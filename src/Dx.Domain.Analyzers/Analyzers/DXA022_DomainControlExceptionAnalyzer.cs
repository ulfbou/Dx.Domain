// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA022_DomainControlExceptionAnalyzer.cs" company="Dx.Domain Team">
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
    /// Enforces Result-based error signaling by prohibiting domain control exceptions in Result-returning methods.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Methods that return Result must communicate failure through Result.Failure, not through exceptions. Throwing for control flow breaks the explicit contract of the Kernel and makes error paths non-observable.
    /// </para>
    /// <para>
    /// Scope behavior: Applies to S1, S2, and S3. S0 may define exception types but must not use them for domain control flow in Result-returning APIs.
    /// </para>
    /// <para>
    /// The analyzer is conservative and classifies intent using the exception intent classifier. Infrastructure exceptions and invariant violations are not flagged.
    /// </para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA022_DomainControlExceptionAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Gets the diagnostic identifier for discouraged domain control exceptions.
        /// </summary>
        public const string DiagnosticId = "DXA022";

        private const string Category = "Domain.ExceptionHandling";
        private static readonly LocalizableString Title =
            "Discouraged Domain Control Exception";
        private static readonly LocalizableString MessageFormat =
            "Use Result.Failure instead of throwing exception in Result-returning method.";
        private static readonly LocalizableString Description =
            "Methods that return Result should use Result.Failure instead of throwing exceptions for domain control flow.";

        /// <summary>
        /// Defines the diagnostic descriptor for domain control exceptions.
        /// </summary>
        /// <remarks>
        /// Severity is Warning. The rule preserves Result as the single error channel for domain operations.
        /// </remarks>
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        /// <inheritdoc />
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

                    AnalyzeThrow(operationContext, services);
                }, OperationKind.Throw);
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

        private static void AnalyzeThrow(OperationAnalysisContext context, AnalyzerServices services)
        {
            var throwOperation = (IThrowOperation)context.Operation;

            // Skip if generated code
            if (throwOperation.Exception?.Type != null &&
                services.Generated.IsGenerated(throwOperation.Exception.Type))
                return;

            // Get the scope - only enforce in S1, S2 (not S0 kernel)
            var scope = services.Scope.ResolveSymbol(context.ContainingSymbol);
            if (scope == Scope.S0)
                return;

            // Check if we're in a method that returns Result
            if (context.ContainingSymbol is not IMethodSymbol method)
                return;

            if (!services.Semantic.IsKernelResultType(method.ReturnType))
                return;

            // Classify the exception intent
            var intent = services.Exceptions.Classify(throwOperation);

            // Allow argument validation and invariant violations
            if (intent == ExceptionIntent.ArgumentValidation ||
                intent == ExceptionIntent.InvariantViolation)
                return;

            // Allow rethrows (throw; with no expression)
            if (throwOperation.Exception == null)
                return;

            // Report diagnostic for domain control or unknown exceptions in Result-returning methods
            if (intent == ExceptionIntent.DomainControl || intent == ExceptionIntent.Unknown)
            {
                var diagnostic = Diagnostic.Create(Rule, throwOperation.Syntax.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
