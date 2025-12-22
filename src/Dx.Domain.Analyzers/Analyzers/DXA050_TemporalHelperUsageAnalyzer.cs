// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA050_TemporalHelperUsageAnalyzer.cs" company="Dx.Domain Team">
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
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;
using Dx.Domain.Analyzers.Infrastructure.Semantics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers.Analyzers
{
    /// <summary>
    /// Analyzer for DXA050: Temporal Helper Usage in Kernel.
    /// Detects use of temporal/policy helpers in kernel code.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA050_TemporalHelperUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DXA050";
        private const string Category = "Domain.Architecture";

        private static readonly LocalizableString Title =
            "Temporal Helper Usage in Kernel";
        private static readonly LocalizableString MessageFormat =
            "Temporal or policy-sensitive helper used in kernel. Move to edge package or justify via DPI.";
        private static readonly LocalizableString Description =
            "Kernel code should remain mechanical, not prescriptive. Temporal helpers encode business time semantics and belong at the edges.";

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
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                var services = CreateServices(startContext);

                startContext.RegisterOperationAction(operationContext =>
                {
                    AnalyzeInvocation(operationContext, services);
                }, OperationKind.Invocation);
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

        private static void AnalyzeInvocation(OperationAnalysisContext context, AnalyzerServices services)
        {
            var invocation = (IInvocationOperation)context.Operation;

            // Skip if generated code
            if (invocation.TargetMethod != null && services.Generated.IsGenerated(invocation.TargetMethod))
                return;

            // Only analyze S0 scope (kernel) - also flag in S1 when used in kernel contexts
            var scope = services.Scope.ResolveSymbol(context.ContainingSymbol);
            if (scope != Scope.S0 && scope != Scope.S1)
                return;

            // Check if it's a temporal helper method
            if (invocation.TargetMethod == null || !IsTemporalHelper(invocation.TargetMethod))
                return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.Syntax.GetLocation()));
        }

        private static bool IsTemporalHelper(IMethodSymbol method)
        {
            var containingType = method.ContainingType?.ToDisplayString();
            var methodName = method.Name;

            // Check for Require.Temporal helpers
            if (containingType != null &&
                containingType.Contains("Require") &&
                (methodName == "NotInFuture" || methodName == "NotInPast" ||
                 methodName == "InRange" || methodName == "BeforeNow" ||
                 methodName == "AfterNow" || methodName == "Between"))
            {
                return true;
            }

            // Check for DateTime/DateTimeOffset policy methods
            if (methodName.Contains("ValidateTime") || methodName.Contains("EnsureTime") ||
                methodName.Contains("CheckTime") || methodName.Contains("AssertTime"))
                return true;

            return false;
        }
    }
}
