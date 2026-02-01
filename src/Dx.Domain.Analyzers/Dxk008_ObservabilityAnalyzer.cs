// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dxk008_ObservabilityAnalyzer.cs" company="Dx.Domain Team">
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

using Dx.Domain.Analyzers.Diagnostics;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Observability;
using Dx.Domain.Analyzers.Roles;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class Dxk008_ObservabilityAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Dxk.DXK008);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(startContext =>
            {
                var role = RoleResolver.Resolve(startContext.Compilation);
                if (role != DxAssemblyRole.Host)
                    return;

                var correlationIdType = startContext.Compilation.GetTypeByMetadataName("Dx.Domain.Primitives.CorrelationId")
                                        ?? startContext.Compilation.GetTypeByMetadataName("Dx.Domain.CorrelationId");

                if (correlationIdType == null)
                    return;

                var generated = new GeneratedCodeDetector(startContext.Options.AnalyzerConfigOptionsProvider);
                var checker = new CorrelationIdPropagationChecker(correlationIdType);

                startContext.RegisterOperationAction(
                    ctx => AnalyzeInvocation(ctx, checker, generated),
                    OperationKind.Invocation);
            });
        }

        private static void AnalyzeInvocation(
            OperationAnalysisContext context,
            CorrelationIdPropagationChecker checker,
            GeneratedCodeDetector generated)
        {
            var invocation = (IInvocationOperation)context.Operation;

            if (invocation.TargetMethod == null)
                return;

            if (generated.IsGenerated(invocation.TargetMethod))
                return;

            if (context.ContainingSymbol is IMethodSymbol method && generated.IsGenerated(method))
                return;

            if (!checker.ShouldReport(invocation, context.ContainingSymbol as IMethodSymbol))
                return;

            context.ReportDiagnostic(Diagnostic.Create(Dxk.DXK008, invocation.Syntax.GetLocation()));
        }
    }
}
