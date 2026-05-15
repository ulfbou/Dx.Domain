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

using Dx.Domain.Analyzers.Infrastructure;
using Dx.Domain.Analyzers.Infrastructure.Exceptions;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Flow;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

using System.Collections.Immutable;

namespace Dx.Domain.Analyzers.Analyzers;

/// <summary>
/// Enforces Dx.Domain rule DXA010: Construction Authority Violation.
/// </summary>
/// <remarks>
/// This analyzer enforces a compile-time architectural invariant:
/// domain types must be constructed exclusively through approved Dx facade factories.
///
/// Scope:
/// - Applies to S1–S3 (shared, domain, application).
/// - Explicitly ignored in S0 (kernel).
///
/// Enforcement model:
/// - Violations produce diagnostics.
/// - No automatic code fixes are applied.
/// - Program behavior is not modified.
///
/// DX-first principle:
/// Construction authority is enforced at compile time to prevent invariant bypass.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DXA010_ConstructionAuthorityAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Diagnostic contract identifier for DXA010.
    /// This identifier is part of the public governance contract and
    /// must remain stable and never be reused or repurposed.
    /// </summary>
    public const string DiagnosticId = "DXA010";

    private const string Category = "Domain.Architecture";

    private static readonly LocalizableString Title =
        "Construction Authority Violation";

    private static readonly LocalizableString MessageFormat =
        "Create domain instances via the Dx facade. Direct construction or public factory use is forbidden outside kernel packages.";

    private static readonly LocalizableString Description =
        "Domain types should be constructed through the Dx facade to centralize invariant enforcement and make creation auditable.";

    /// <summary>
    /// Diagnostic descriptor defining the stable contract for DXA010.
    /// </summary>
    /// <remarks>
    /// Rule identifiers are part of the public governance contract and
    /// must remain stable over time.
    /// </remarks>
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

            startContext.RegisterOperationAction(operationContext =>
            {
                // 1. Kernel boundary – must be first
                if (services.Scope.IsKernelInternal(operationContext.Compilation.Assembly))
                    return;

                // 2. Generated code – use the built-in flag, not IsGenerated(SyntaxTree)
                if (operationContext.IsGeneratedCode)
                    return;

                // 3. Safe dispatch
                switch (operationContext.Operation)
                {
                    case IObjectCreationOperation:
                        AnalyzeObjectCreation(operationContext, services);
                        break;
                    case IInvocationOperation:
                        AnalyzeFactoryInvocation(operationContext, services);
                        break;
                }
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
        return AnalyzerServicesFactory.Create(context.Compilation, config);
    }

    private static void AnalyzeObjectCreation(OperationAnalysisContext context, AnalyzerServices services)
    {
        var operation = (IObjectCreationOperation)context.Operation;

        // Skip if generated code
        if (operation.Type!= null && services.Generated.IsGenerated(operation.Type))
            return;

        // Skip if not a domain type
        if (operation.Type == null ||!services.Semantic.IsDomainType(operation.Type))
            return;

        // Get the scope of the call site
        var callSiteScope = services.Scope.ResolveSymbol(context.ContainingSymbol);

        // S0 (kernel) is trusted - allow direct construction
        if (callSiteScope == Scope.S0)
            return;

        // Check if we're inside a type constructor of the type itself
        if (IsWithinTypeConstructor(context.ContainingSymbol, operation.Type))
            return;

        // Check if we're inside a facade method
        if (IsWithinFacadeMethod(context.ContainingSymbol, services))
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
        if (!services.Semantic.IsDomainType(returnType))
            return;

        // Get the scope of the call site
        var callSiteScope = services.Scope.ResolveSymbol(context.ContainingSymbol);

        // S0 (kernel) is trusted
        if (callSiteScope == Scope.S0)
            return;

        // Check if this is a Dx facade factory method
        if (services.Dx.IsDxFacadeFactory(operation.TargetMethod))
            return;

        // Check if we're inside a facade method
        if (IsWithinFacadeMethod(context.ContainingSymbol, services))
            return;

        // Report diagnostic
        var diagnostic = Diagnostic.Create(Rule, operation.Syntax.GetLocation());
        context.ReportDiagnostic(diagnostic);
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

    private static bool IsWithinFacadeMethod(ISymbol containingSymbol, AnalyzerServices services)
    {
        // Check if the containing method is a facade factory
        if (containingSymbol is IMethodSymbol method)
        {
            return services.Dx.IsDxFacadeFactory(method);
        }

        return false;
    }
}
