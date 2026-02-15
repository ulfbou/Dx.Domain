// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA090_SuppressionDetectionAnalyzer.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Immutable;
using System.Linq;

using Dx.Domain.Annotations;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers
{
    /// <summary>
    /// Analyzer for DXA090: Forbidden Analyzer Suppression.
    /// Detects attempts to suppress DX diagnostics in consumer scope.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA090_SuppressionDetectionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DxRuleIds.DXA090;
        private const string Category = DxCategories.DomainArchitecture;

        private static readonly LocalizableString Title =
            "Forbidden Analyzer Suppression";
        private static readonly LocalizableString MessageFormat =
            "DX diagnostics must not be suppressed in consumer scope. Remove the suppression and fix the underlying issue.";
        private static readonly LocalizableString Description =
            "Consumer projects cannot disable or suppress DX analyzers. Fix the reported diagnostics instead of suppressing them.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                var scopeResolver = new ScopeResolver(startContext.Options.AnalyzerConfigOptionsProvider);
                if (scopeResolver.ResolveAssembly(startContext.Compilation.Assembly) != Scope.S3)
                    return;

                startContext.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
                startContext.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
            });
        }

        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            var pragmas = root.DescendantTrivia()
                .Where(t => t.IsKind(SyntaxKind.PragmaWarningDirectiveTrivia))
                .Select(t => t.GetStructure())
                .OfType<PragmaWarningDirectiveTriviaSyntax>();

            foreach (var pragma in pragmas)
            {
                if (!pragma.DisableOrRestoreKeyword.IsKind(SyntaxKind.DisableKeyword))
                    continue;

                if (pragma.ErrorCodes.Count == 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, pragma.GetLocation()));
                    continue;
                }

                foreach (var code in pragma.ErrorCodes)
                {
                    var raw = code.ToString().Trim();
                    if (!IsDxDiagnosticId(raw))
                        continue;

                    context.ReportDiagnostic(Diagnostic.Create(Rule, code.GetLocation()));
                }
            }
        }

        private static void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;
            var type = context.SemanticModel.GetTypeInfo(attribute, context.CancellationToken).Type;
            if (type == null)
                return;

            if (!IsSuppressMessageAttribute(type))
                return;

            if (attribute.ArgumentList == null)
                return;

            foreach (var argument in attribute.ArgumentList.Arguments)
            {
                var constant = context.SemanticModel.GetConstantValue(argument.Expression, context.CancellationToken);
                if (!constant.HasValue || constant.Value is not string text)
                    continue;

                if (ContainsDxDiagnosticId(text))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, argument.GetLocation()));
                    return;
                }
            }
        }

        private static bool IsSuppressMessageAttribute(ITypeSymbol type)
        {
            if (type.Name == "SuppressMessageAttribute")
                return true;

            return type.ToDisplayString() == "System.Diagnostics.CodeAnalysis.SuppressMessageAttribute";
        }

        private static bool ContainsDxDiagnosticId(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var tokens = text.Split(':');
            return tokens.Any(IsDxDiagnosticId);
        }

        private static bool IsDxDiagnosticId(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return text.StartsWith("DXA", System.StringComparison.OrdinalIgnoreCase) ||
                   text.StartsWith("DXK", System.StringComparison.OrdinalIgnoreCase) ||
                   text.StartsWith("DXT", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
