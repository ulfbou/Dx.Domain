// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA020_ResultIgnoredAnalyzer.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;

using Dx.Domain.Analyzers.Infrastructure;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Flow;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers
{
    /// <summary>
    /// Analyzer for DXA020: Result Ignored.
    /// Detects when a Result&lt;T&gt; is created but not explicitly handled, returned, or checked.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA020_ResultIgnoredAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DXA020";
        private const string Category = "Domain.ResultHandling";

        private static readonly LocalizableString Title =
            "Result Ignored";
        private static readonly LocalizableString MessageFormat =
            "Result value is produced and ignored. Either handle, return, or explicitly discard with intent.";
        private static readonly LocalizableString Description =
            "Result instances must be explicitly handled to prevent silent failures and lost domain errors.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                var assemblyName = startContext.Compilation.AssemblyName;
                if (assemblyName != null &&
                    (assemblyName.IndexOf("Dx.Domain.Kernel", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     assemblyName.IndexOf("Dx.Domain.Primitives", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return;
                }

                var services = CreateServices(startContext);

                startContext.RegisterSymbolAction(symbolContext =>
                {
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
                new ResultFlowEngineWrapper(),
                new GeneratedCodeDetector(config));
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context, AnalyzerServices services)
        {
            var method = (IMethodSymbol)context.Symbol;

            if (method.IsImplicitlyDeclared || method.DeclaringSyntaxReferences.Length == 0)
                return;

            if (services.Generated.IsGenerated(method))
                return;

            var scope = services.Scope.ResolveSymbol(method);
            if (scope == Scope.S0)
                return;

            var optionsProvider = context.Options.AnalyzerConfigOptionsProvider;
            var options = method.Locations.FirstOrDefault()?.SourceTree is { } tree
                ? optionsProvider.GetOptions(tree)
                : optionsProvider.GlobalOptions;

            var graph = services.Flow.Analyze(method, context.Compilation, options, context.CancellationToken);
            if (!graph.IsValid)
                return;

            foreach (var node in graph.ResultNodes)
            {
                if (!graph.NodeStates.TryGetValue(node, out var state))
                    continue;

                if (state != ResultFlow.ResultState.Ignored)
                    continue;

                if (node.Producer.Syntax == null)
                    continue;

                context.ReportDiagnostic(Diagnostic.Create(Rule, node.Producer.Syntax.GetLocation()));
            }
        }
    }
}
