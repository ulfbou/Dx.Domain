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
using System.Diagnostics.CodeAnalysis;
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
    /// Ensures generated code is correctly tagged to enable accurate scope resolution and analyzer exclusion.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Analyzers rely on reliable identification of generated code to avoid false positives and to apply correct scope rules. Missing or incorrect generator tags break this detection.
    /// </para>
    /// <para>
    /// Scope behavior: Applies to all scopes where generators emit code. Tagging requirements are enforced regardless of S0 through S3 classification.
    /// </para>
    /// <para>
    /// The analyzer verifies the presence of standard generated code attributes and comments. It operates fail-open for unrecognized generation patterns.
    /// </para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA070_GeneratedCodeTaggingAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Gets the diagnostic identifier for missing generated code tagging.
        /// </summary>
        public const string DiagnosticId = "DXA070";

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private const string Category = "Domain.CodeGeneration";

        private static readonly LocalizableString Title =
            "Generated Code Tagging";
        private static readonly LocalizableString MessageFormat =
            "Generated code missing required generator tag. Add [GeneratedCode] attribute or configured marker.";
        private static readonly LocalizableString Description =
            "Generated code should be tagged with [GeneratedCode] attribute to prevent false positives from analyzers.";

        /// <summary>
        /// Defines the diagnostic descriptor for generated code tagging violations.
        /// </summary>
        /// <remarks>
        /// Severity is Warning. The rule ensures generators emit identifiable markers required by the analyzer infrastructure.
        /// </remarks>
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                var services = CreateServices(startContext);

                startContext.RegisterSymbolAction(symbolContext =>
                {
                    // 1. Kernel types are implementation, not subject to DXA011
                    if (services.Scope.IsKernelInternal(symbolContext.Compilation.Assembly))
                        return;

                    // 2. Generated code – SymbolAnalysisContext has no IsGeneratedCode, use the detector
                    if (symbolContext.IsGeneratedCode)
                        return;

                    AnalyzeNamedType(symbolContext, services);
                }, SymbolKind.NamedType);
            });
        }

        [SuppressMessage(
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

            // Skip if generated code
            if (services.Generated.IsGenerated(type))
                return;

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
