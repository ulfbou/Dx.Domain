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

using System.Collections.Immutable;

using Dx.Domain.Analyzers.Infrastructure;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Flow;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers.Analyzers
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
                var services = CreateServices(startContext);

                startContext.RegisterOperationAction(operationContext =>
                {
                    AnalyzeResultUsage(operationContext, services);
                }, OperationKind.Invocation, OperationKind.ObjectCreation, OperationKind.SimpleAssignment, OperationKind.Return);
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

        private static void AnalyzeResultUsage(OperationAnalysisContext context, AnalyzerServices services)
        {
            var expression = context.Operation;

            // Skip if generated code
            if (expression.Type != null && services.Generated.IsGenerated(expression.Type))
                return;

            // Check if expression type is a Result type
            if (expression.Type == null || !services.Semantic.IsKernelResultType(expression.Type))
                return;

            // Get the scope - only enforce in S1, S2 (not S0 kernel)
            var scope = services.Scope.ResolveSymbol(context.ContainingSymbol);
            if (scope == Scope.S0)
                return;

            // If this is an expression statement that produces a Result, it's being ignored
            // (unless it's assigned to a variable, which would be a different operation kind)

            var diagnostic = Diagnostic.Create(Rule, expression.Syntax.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
