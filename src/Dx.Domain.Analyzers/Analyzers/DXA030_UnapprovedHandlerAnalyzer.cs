// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA030_UnapprovedHandlerAnalyzer.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
    /// Analyzer for DXA030: Unapproved Handler Usage.
    /// Detects Result values being passed to non-approved handlers.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA030_UnapprovedHandlerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DXA030";
        private const string Category = "Domain.ResultHandling";

        private static readonly LocalizableString Title =
            "Unapproved Handler Usage";
        private static readonly LocalizableString MessageFormat =
            "Result passed to an unapproved handler. Register the handler in EditorConfig or use a known adapter.";
        private static readonly LocalizableString Description =
            "Result values should only be passed to approved handlers to ensure explicit and analyzable result handling.";

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
                var approvedHandlers = LoadApprovedHandlers(startContext.Options.AnalyzerConfigOptionsProvider);

                startContext.RegisterOperationAction(operationContext =>
                {
                    AnalyzeInvocation(operationContext, services, approvedHandlers);
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

        private static ImmutableHashSet<string> LoadApprovedHandlers(AnalyzerConfigOptionsProvider config)
        {
            var options = config.GlobalOptions;
            if (!options.TryGetValue("dx_result_handlers", out var raw))
                return ImmutableHashSet<string>.Empty;

            return raw.Split(';')
                      .Select(s => s.Trim())
                      .Where(s => s.Length > 0)
                      .ToImmutableHashSet();
        }

        private static void AnalyzeInvocation(
            OperationAnalysisContext context,
            AnalyzerServices services,
            ImmutableHashSet<string> approvedHandlers)
        {
            var invocation = (IInvocationOperation)context.Operation;

            // Skip if generated code
            if (invocation.TargetMethod != null && services.Generated.IsGenerated(invocation.TargetMethod))
                return;

            // Only analyze S0, S1, S2 scopes
            if (invocation.TargetMethod == null)
                return;
            
            var scope = services.Scope.ResolveSymbol(invocation.TargetMethod);
            if (scope == Scope.S3)
                return;

            // Check if any argument is a Result type
            foreach (var argument in invocation.Arguments)
            {
                if (argument.Value.Type != null && services.Semantic.IsKernelResultType(argument.Value.Type))
                {
                    // Check if the target method is approved
                    var methodName = invocation.TargetMethod.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    
                    if (!IsApprovedHandler(methodName, approvedHandlers, invocation.TargetMethod))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.Syntax.GetLocation()));
                        return;
                    }
                }
            }
        }

        private static bool IsApprovedHandler(
            string methodName,
            ImmutableHashSet<string> approvedHandlers,
            IMethodSymbol method)
        {
            // Check if in approved list
            if (approvedHandlers.Contains(methodName))
                return true;

            // Check for common Result extension methods
            var name = method.Name;
            if (name == "Match" || name == "Map" || name == "Bind" || 
                name == "OnSuccess" || name == "OnFailure" || name == "Tap" ||
                name == "Ensure" || name == "ThenAsync" || name == "Finally")
                return true;

            // If it's an extension method on Result, it's likely approved
            if (method.IsExtensionMethod && method.Parameters.Length > 0)
            {
                var firstParam = method.Parameters[0];
                if (firstParam.Type.ToDisplayString().Contains("Result"))
                    return true;
            }

            return false;
        }
    }
}
