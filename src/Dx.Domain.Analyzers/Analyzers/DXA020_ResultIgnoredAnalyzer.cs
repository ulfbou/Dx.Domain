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
using System.Diagnostics.CodeAnalysis;

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
    /// Requires explicit handling of Result values to preserve explicit control flow.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A Result represents a domain outcome that must be observed. Ignoring a Result silently discards failure information and violates the explicitness principle of the Kernel.
    /// </para>
    /// <para>
    /// Scope behavior: Applies to S1, S2, and S3. S0 is exempt for internal Result construction.
    /// </para>
    /// <para>
    /// The analyzer tracks Result creation, return, and inspection via IsSuccess, IsFailure, Match, Map, and Bind. Values passed to approved handlers are considered handled. Analysis is fail-open.
    /// </para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA020_ResultIgnoredAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Gets the diagnostic identifier for ignored Result values.
        /// </summary>
        public const string DiagnosticId = "DXA020";

        private const string Category = "Domain.ResultHandling";
        private static readonly LocalizableString Title =
            "Result Ignored";
        private static readonly LocalizableString MessageFormat =
            "Result value is produced and ignored. Either handle, return, or explicitly discard with intent.";
        private static readonly LocalizableString Description =
            "Result instances must be explicitly handled to prevent silent failures and lost domain errors.";

        /// <summary>
        /// Defines the diagnostic descriptor for ignored Result values.
        /// </summary>
        /// <remarks>
        /// Severity is Warning. The rule enforces that every Result is either returned, inspected, or passed to an approved handler.
        /// </remarks>
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        /// <summary>
        /// Defines the diagnostic descriptor for ignored Result values.
        /// </summary>
        /// <remarks>
        /// Severity is Warning. The rule enforces that every Result is either returned, inspected, or passed to an approved handler.
        /// </remarks>
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

                startContext.RegisterOperationAction(operationContext =>
                {
                    // 1. Kernel types are implementation, not subject to DXA011
                    if (services.Scope.IsKernelInternal(operationContext.Compilation.Assembly))
                        return;

                    // 2. Generated code – SymbolAnalysisContext has no IsGeneratedCode, use the detector
                    if (operationContext.IsGeneratedCode)
                        return;

                    // Only flag standalone expression statements
                    if (operationContext.Operation.Parent is not IExpressionStatementOperation)
                        return;

                    AnalyzeResultUsage(operationContext, services);
                }, OperationKind.Invocation, OperationKind.ObjectCreation);

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
