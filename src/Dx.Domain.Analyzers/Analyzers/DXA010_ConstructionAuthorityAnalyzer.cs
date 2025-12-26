// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA010_ConstructionAuthorityAnalyzer.cs" company="Dx.Domain Team">
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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers.Analyzers
{
    /// <summary>
    /// Analyzer for DXA010: Construction Authority Violation.
    /// Detects direct construction of domain types outside permitted Dx facade factories.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA010_ConstructionAuthorityAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DXA010";
        private const string Category = "Domain.Architecture";

        private static readonly LocalizableString Title =
            "Construction Authority Violation";
        private static readonly LocalizableString MessageFormat =
            "Create domain instances via the Dx facade. Direct construction or public factory use is forbidden outside kernel packages.";
        private static readonly LocalizableString Description =
            "Domain types should be constructed through the Dx facade to centralize invariant enforcement and make creation auditable.";

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
                    AnalyzeObjectCreation(operationContext, services);
                    AnalyzeFactoryInvocation(operationContext, services);
                }, OperationKind.ObjectCreation, OperationKind.Invocation);
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

        private static void AnalyzeObjectCreation(OperationAnalysisContext context, AnalyzerServices services)
        {
            var operation = (IObjectCreationOperation)context.Operation;

            // Skip if generated code
            if (operation.Type != null && services.Generated.IsGenerated(operation.Type))
                return;

            // Skip if not a domain type
            if (!IsDomainType(operation.Type, services))
                return;

            // Get the scope of the call site
            var callSiteScope = services.Scope.ResolveSymbol(context.ContainingSymbol);

            // S0 (kernel) is trusted - allow direct construction
            if (callSiteScope == Scope.S0)
                return;

            // Check if we're inside a type constructor of the type itself
            if (IsWithinTypeConstructor(context.ContainingSymbol, operation.Type))
                return;

            // Report diagnostic
            var diagnostic = Diagnostic.Create(Rule, operation.Syntax.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static void AnalyzeFactoryInvocation(OperationAnalysisContext context, AnalyzerServices services)
        {
            var operation = (IInvocationOperation)context.Operation;

            // Skip if generated code
            if (services.Generated.IsGenerated(operation.TargetMethod))
                return;

            // Only check static factory methods that return domain types
            if (!operation.TargetMethod.IsStatic)
                return;

            var returnType = operation.TargetMethod.ReturnType;
            if (!IsDomainType(returnType, services))
                return;

            // Get the scope of the call site
            var callSiteScope = services.Scope.ResolveSymbol(context.ContainingSymbol);

            // S0 (kernel) is trusted
            if (callSiteScope == Scope.S0)
                return;

            // Check if this is a Dx facade factory method
            if (services.Dx.IsDxFacadeFactory(operation.TargetMethod))
                return;

            // Report diagnostic
            var diagnostic = Diagnostic.Create(Rule, operation.Syntax.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsDomainType(ITypeSymbol? type, AnalyzerServices services)
        {
            if (type == null)
                return false;

            // Check if it's a Result type (domain types return Result)
            if (services.Semantic.IsKernelResultType(type))
                return false;

            // Check if it's in Dx.Domain namespace
            var ns = type.ContainingNamespace?.ToDisplayString();
            return ns != null && ns.StartsWith("Dx.Domain", System.StringComparison.Ordinal);
        }

        private static bool IsWithinTypeConstructor(ISymbol containingSymbol, ITypeSymbol? createdType)
        {
            if (createdType == null)
                return false;
            // Check if we're in a static constructor (.cctor) or instance constructor of the same type
            if (containingSymbol is IMethodSymbol method)
            {
                if (method.MethodKind == MethodKind.Constructor ||
                    method.MethodKind == MethodKind.StaticConstructor)
                {
                    return SymbolEqualityComparer.Default.Equals(method.ContainingType, createdType);
                }
            }

            return false;
        }
    }
}
