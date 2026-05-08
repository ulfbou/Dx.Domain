// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA065_UnresolvedXmlDocReferenceAnalyzer.cs" company="Dx.Domain Team">
// Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
// This software is licensed under the MIT License.
// See the project's root <c>LICENSE</c> file for details.
// Contributions are welcome, subject to the terms of the project's license.
// See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dx.Domain.Analyzers.Analyzers;

/// <summary>
/// Enforces DXA065: XML documentation should use see-cref references for Dx.Domain types.
/// </summary>
/// <remarks>
/// This analyzer supports the Manifesto principle "compiler over runtime" by ensuring
/// documentation references are rename-safe and verified at compile time.
///
/// Scope: Applies to public APIs in S0 (Kernel, Primitives, Facts) for alpha release.
/// Severity is Warning to avoid blocking first release.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DXA065_UnresolvedXmlDocReferenceAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Diagnostic contract identifier for DXA065.
    /// </summary>
    public const string DiagnosticId = "DXA065";

    private const string Category = "Documentation";

    private static readonly LocalizableString Title = "Unresolved XML documentation reference";

    private static readonly LocalizableString MessageFormat = "Use <see cref=\"{0}\"/> instead of plain type name '{0}' in XML documentation";

    private static readonly LocalizableString Description = "Documentation references to Dx.Domain types should use <see cref=\"T:System.Object\"/> to ensure compile-time verification and rename safety.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/ulfbou/dx.domain/blob/main/docs/diagnostics/DXA065.md");

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    // Core types frozen for alpha
    private static readonly ImmutableHashSet<string> CoreTypeNames = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "Result",
        "DomainError",
        "InvariantError",
        "Unit",
        "UserId",
        "ActorId",
        "CorrelationId",
        "FactId",
        "TraceId",
        "SpanId",
        "CausationId",
        "Causation",
        "Fact",
        "FactType",
        "FactTypeOf",
        "IDomainFact",
        "InvariantViolationException",
        "TransitionResult"
    );

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(startContext =>
        {
            var scopeResolver = new ScopeResolver(startContext.Options.AnalyzerConfigOptionsProvider);

            if (!IsKernelAssembly(startContext.Compilation.Assembly, scopeResolver, startContext.Compilation))
                return;

            startContext.RegisterSyntaxNodeAction(
                ctx => AnalyzeDocumentation(ctx),
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxKind.MultiLineDocumentationCommentTrivia);
        });
    }

    private static bool IsKernelAssembly(IAssemblySymbol assembly, ScopeResolver resolver, Compilation compilation)
    {
        if (resolver.IsKernelInternal(assembly))
            return true;

        // Test support: check for [assembly: DxLayer("Kernel")]
        foreach (var attr in assembly.GetAttributes())
        {
            var name = attr.AttributeClass?.ToDisplayString();
            if (name == "Dx.Domain.Annotations.DxLayerAttribute" || name?.EndsWith("DxLayerAttribute") == true)
            {
                if (attr.ConstructorArguments.Length > 0 &&
                    attr.ConstructorArguments[0].Value?.ToString() == "Kernel")
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static void AnalyzeDocumentation(SyntaxNodeAnalysisContext context)
    {
        var trivia = (DocumentationCommentTriviaSyntax)context.Node;
        var parent = trivia.ParentTrivia.Token.Parent;

        if (!IsPublicApi(parent))
            return;

        foreach (var xmlText in trivia.DescendantNodes().OfType<XmlTextSyntax>())
        {
            // Skip <see>, <c>, <code>, etc.
            if (xmlText.Ancestors().Any(a => a is XmlElementSyntax e &&
                e.StartTag.Name.LocalName.ValueText is "see" or "c" or "code" or "paramref" or "typeparamref"))
                continue;

            var text = xmlText.ToString();
            if (string.IsNullOrWhiteSpace(text))
                continue;

            foreach (var typeName in CoreTypeNames)
            {
                if (!text.Contains(typeName, StringComparison.Ordinal))
                    continue;

                var pattern = $@"\b{Regex.Escape(typeName)}\b";
                foreach (Match match in Regex.Matches(text, pattern))
                {
                    var start = xmlText.SpanStart + match.Index;
                    var location = Location.Create(context.Node.SyntaxTree, new TextSpan(start, match.Length));
                    context.ReportDiagnostic(Diagnostic.Create(Rule, location, typeName));
                }
            }
        }
    }

    private static bool IsPublicApi(SyntaxNode? node)
    {
        return node switch
        {
            BaseTypeDeclarationSyntax type => type.Modifiers.Any(SyntaxKind.PublicKeyword),
            MethodDeclarationSyntax method => method.Modifiers.Any(SyntaxKind.PublicKeyword),
            PropertyDeclarationSyntax prop => prop.Modifiers.Any(SyntaxKind.PublicKeyword),
            FieldDeclarationSyntax field => field.Modifiers.Any(SyntaxKind.PublicKeyword),
            _ => false
        };
    }
}
