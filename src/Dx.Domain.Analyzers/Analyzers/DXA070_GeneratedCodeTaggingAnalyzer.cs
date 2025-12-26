// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA070_GeneratedCodeTaggingAnalyzer.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Linq;

using Dx.Domain.Analyzers.Infrastructure;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers.Analyzers
{
    /// <summary>
    /// Analyzer for DXA070: Generated Code Tagging.
    /// Detects generated code missing required generator tags.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA070_GeneratedCodeTaggingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DXA070";
        private const string Category = "Domain.CodeGeneration";

        private static readonly LocalizableString Title =
            "Generated Code Tagging";
        private static readonly LocalizableString MessageFormat =
            "Generated code missing required generator tag. Add [GeneratedCode] attribute or configured marker.";
        private static readonly LocalizableString Description =
            "Generated code should be tagged with [GeneratedCode] attribute to prevent false positives from analyzers.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                var services = CreateServices(startContext);

                startContext.RegisterSymbolAction(symbolContext =>
                {
                    AnalyzeNamedType(symbolContext, services);
                }, SymbolKind.NamedType);
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "MicrosoftCodeAnalysisCorrectness",
            "RS1012:Start action has no registered actions",
            Justification = "Actions are registered in the enclosing lambda; this helper only builds services.")]
        private static AnalyzerServices CreateServices(CompilationStartAnalysisContext context)
        {
            var config = context.Options.AnalyzerConfigOptionsProvider;
            return new AnalyzerServices(
                new ScopeResolver(config),
                new DxFacadeResolver(context.Compilation, config),
                new SemanticClassifier(context.Compilation),
                new Infrastructure.Exceptions.ExceptionIntentClassifier(context.Compilation, config),
                new Infrastructure.Flow.ResultFlowEngineWrapper(),
                new GeneratedCodeDetector(config));
        }

        private static void AnalyzeNamedType(SymbolAnalysisContext context, AnalyzerServices services)
        {
            var type = (INamedTypeSymbol)context.Symbol;

            // Only analyze S1 and S2 scopes (domain and application)
            var scope = services.Scope.ResolveSymbol(type);
            if (scope != Scope.S1 && scope != Scope.S2)
                return;

            // Check if type looks like generated code (heuristic)
            if (!LooksLikeGeneratedCode(type))
                return;

            // Check if already tagged
            if (HasGeneratedCodeAttribute(type))
                return;

            // Check if in namespace marked as generated
            if (services.Generated.IsGenerated(type))
                return;

            // Report diagnostic
            if (type.Locations.Any())
            {
                var location = type.Locations.First();
                context.ReportDiagnostic(Diagnostic.Create(Rule, location));
            }
        }

        private static bool LooksLikeGeneratedCode(INamedTypeSymbol type)
        {
            var name = type.Name;

            // Check for common generated code patterns
            if (name.Contains("_g") || name.Contains("__") ||
                name.EndsWith("Generated", System.StringComparison.Ordinal) || name.EndsWith("_Generated", System.StringComparison.Ordinal) ||
                name.EndsWith("Proxy", System.StringComparison.Ordinal) || name.EndsWith("_Proxy", System.StringComparison.Ordinal))
                return true;

            // Check if file path contains "Generated" or "obj"
            if (type.Locations.Any())
            {
                var location = type.Locations.First();
                if (location.SourceTree != null)
                {
                    var path = location.SourceTree.FilePath;
                    if (path.Contains("Generated") || path.Contains("\\obj\\") || path.Contains("/obj/"))
                        return true;
                }
            }

            // Check for namespace hints
            var ns = type.ContainingNamespace?.ToDisplayString();
            if (ns != null && (ns.Contains("Generated") || ns.Contains(".g.")))
                return true;

            return false;
        }

        private static bool HasGeneratedCodeAttribute(INamedTypeSymbol type)
        {
            return type.GetAttributes().Any(a =>
                a.AttributeClass?.ToDisplayString() == typeof(GeneratedCodeAttribute).FullName ||
                a.AttributeClass?.Name == "GeneratedCodeAttribute" ||
                a.AttributeClass?.Name == "DxGeneratedAttribute" ||
                a.AttributeClass?.Name == "CompilerGeneratedAttribute");
        }
    }
}
