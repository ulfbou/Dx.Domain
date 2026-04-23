// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA060_ForbiddenVocabularyAnalyzer.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Analyzers.Analyzers
{
    /// <summary>
    /// Enforces mechanical vocabulary in the Kernel by prohibiting semantic or pattern-specific terminology.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Kernel provides primitives, not domain patterns. Use of forbidden vocabulary indicates semantic leakage into the mechanical layer and compromises neutrality.
    /// </para>
    /// <para>
    /// Scope behavior: Applies exclusively to S0. Vocabulary restrictions preserve the Kernel as a stable, pattern-agnostic foundation.
    /// </para>
    /// <para>
    /// The analyzer evaluates identifiers and type names against the configured forbidden vocabulary list.
    /// </para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA060_ForbiddenVocabularyAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Gets the diagnostic identifier for forbidden vocabulary in the Kernel.
        /// </summary>
        public const string DiagnosticId = "DXA060";

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private const string Category = "Domain.Architecture";

        private static readonly LocalizableString Title =
            "Forbidden Vocabulary in Kernel";
        private static readonly LocalizableString MessageFormat =
            "Forbidden vocabulary '{0}' used in kernel. Move to adapter or rename to mechanical term.";
        private static readonly LocalizableString Description =
            "Kernel code should use mechanical terminology. Pattern-based terms like 'Repository', 'Saga', 'Apply' belong in adapters.";

        /// <summary>
        /// Defines the diagnostic descriptor for forbidden vocabulary.
        /// </summary>
        /// <remarks>
        /// Severity is Warning. The rule maintains Kernel neutrality by restricting terminology to mechanical concepts.
        /// </remarks>
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        // Forbidden vocabulary from the Dx.Domain manifesto
        private static readonly ImmutableHashSet<string> ForbiddenTerms = ImmutableHashSet.Create(
            "AggregateRoot",
            "Repository",
            "Saga",
            "Apply",
            "Handle",
            "TransitionTo",
            "Emit",
            "Publish",
            "Subscribe",
            "Command",
            "Query",
            "Event",
            "Projection",
            "ReadModel",
            "WriteModel",
            "EventStore",
            "Snapshot"
        );

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

                    AnalyzeSymbol(symbolContext, services);
                }, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);
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

        private static void AnalyzeSymbol(SymbolAnalysisContext context, AnalyzerServices services)
        {
            var symbol = context.Symbol;

            // Skip if generated code
            if (services.Generated.IsGenerated(symbol))
                return;

            // Only analyze S0 and S1 scopes (kernel and domain)
            var scope = services.Scope.ResolveSymbol(symbol);
            if (scope != Scope.S0 && scope != Scope.S1)
                return;

            // Check if symbol name contains forbidden vocabulary
            var symbolName = symbol.Name;
            var forbiddenTerm = ForbiddenTerms.FirstOrDefault(term => symbolName.Contains(term));
            if (forbiddenTerm != null)
            {
                if (symbol.Locations.Any())
                {
                    var location = symbol.Locations.First();
                    var diagnostic = Diagnostic.Create(Rule, location, forbiddenTerm);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
