// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA080_FacadeInvariantEnforcementAnalyzer.cs" company="Dx.Domain Team">
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

using Dx.Domain.Analyzers.Infrastructure;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers.Analyzers
{

    /// <summary>
    /// Enforces Dx.Domain rule DXA080: Facade Invariant Enforcement Missing.
    /// </summary>
    /// <remarks>
    /// This analyzer enforces invariant checking at domain creation boundaries.
    /// Dx facade factories must not bypass invariant enforcement.
    ///
    /// Scope:
    /// - Applies to S1 and S2.
    ///
    /// Enforcement model:
    /// - Violations produce diagnostics.
    /// - Facade factories must return Result or enforce invariants explicitly.
    ///
    /// DX-first principle:
    /// Construction boundaries must be correctness-preserving.
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA080_FacadeInvariantEnforcementAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Diagnostic contract identifier for DXA080.
        /// </summary>
        public const string DiagnosticId = "DXA080";

        private const string Category = "Domain.Architecture";

        private static readonly LocalizableString Title =
            "Facade Invariant Enforcement Missing";
        private static readonly LocalizableString MessageFormat =
            "Facade factory does not enforce invariants. Ensure invariants are checked and failures return DomainError/Result.";
        private static readonly LocalizableString Description =
            "Dx facade factory methods must enforce invariants to guarantee creation boundary correctness.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
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

                    AnalyzeMethod(symbolContext, services);
                }, SymbolKind.Method);
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

        private static void AnalyzeMethod(SymbolAnalysisContext context, AnalyzerServices services)
        {
            var method = (IMethodSymbol)context.Symbol;

            // Skip if generated code
            if (services.Generated.IsGenerated(method))
                return;

            // Only analyze S1 and S2 scopes (domain and application)
            var scope = services.Scope.ResolveSymbol(method);
            if (scope != Scope.S1 && scope != Scope.S2)
                return;

            // Check if this is a Dx facade factory method
            if (!services.Dx.IsDxFacadeFactory(method))
                return;

            // Check if method returns Result type (invariants should be enforced)
            // Facade method doesn't return Result - should it enforce invariants?
            // This is a potential issue if creating domain types
            if (!services.Semantic.IsKernelResultType(method.ReturnType)
                && services.Semantic.IsDomainType(method.ReturnType)
                && method.Locations.Any())
            {
                var location = method.Locations.First();
                context.ReportDiagnostic(Diagnostic.Create(Rule, location));
            }
        }
    }
}
