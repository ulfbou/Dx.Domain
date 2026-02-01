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
using System.Linq;

using Dx.Domain.Annotations;
using Dx.Domain.Analyzers.Infrastructure;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers
{
    /// <summary>
    /// Analyzer for DXA010: Construction Authority Violation.
    /// Detects direct construction of domain types outside permitted Dx facade factories.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA010_ConstructionAuthorityAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DxRuleIds.DXA010;
        private const string Category = DxCategories.DomainArchitecture;

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
                var assemblyName = startContext.Compilation.AssemblyName;
                var scopeResolver = new ScopeResolver(startContext.Options.AnalyzerConfigOptionsProvider);
                var scope = scopeResolver.ResolveAssembly(startContext.Compilation.Assembly);
                if (IsKernelLikeLayer(startContext.Options.AnalyzerConfigOptionsProvider) ||
                    scope != Scope.S3 ||
                    IsKernelLikeAssembly(assemblyName) ||
                    (assemblyName != null && assemblyName.StartsWith("Dx.Domain.", System.StringComparison.OrdinalIgnoreCase)) ||
                    IsKernelLikeCompilation(startContext.Compilation))
                    return;

                var services = CreateServices(startContext);

                startContext.RegisterOperationAction(operationContext =>
                {
                    AnalyzeObjectCreation(operationContext, services);
                }, OperationKind.ObjectCreation);

                startContext.RegisterOperationAction(operationContext =>
                {
                    AnalyzeFactoryInvocation(operationContext, services);
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

        private static void AnalyzeObjectCreation(OperationAnalysisContext context, AnalyzerServices services)
        {
            if (context.Operation is not IObjectCreationOperation operation)
                return;

            var syntax = operation.Syntax;
            if (syntax == null)
                return;

            if (context.ContainingSymbol is IAssemblySymbol)
                return;

            if (context.ContainingSymbol.ContainingAssembly == null)
                return;

            if (context.ContainingSymbol.ContainingAssembly.Name.StartsWith("Dx.Domain.", System.StringComparison.OrdinalIgnoreCase))
                return;

            if (IsKernelLikeAssembly(context.ContainingSymbol.ContainingAssembly?.Name))
                return;

            if (context.ContainingSymbol.ContainingAssembly?.Name?.StartsWith("Dx.Domain.", System.StringComparison.OrdinalIgnoreCase) == true)
                return;

            if (IsKernelLikeLocation(context.ContainingSymbol))
                return;

            if (IsKernelLikePath(syntax.SyntaxTree?.FilePath))
                return;

            if (IsAssemblyInfoFile(syntax.SyntaxTree?.FilePath))
                return;

            if (IsKernelAssembly(context.ContainingSymbol.ContainingAssembly))
                return;

            if (IsAttributeOperation(operation))
                return;

            if (operation.Type?.Name.EndsWith("Attribute", System.StringComparison.Ordinal) == true)
                return;

            // Skip if generated code
            if (operation.Type != null && services.Generated.IsGenerated(operation.Type))
                return;

            // Skip if not a domain type
            if (!IsDomainType(operation.Type, services))
                return;

            // Get the scope of the call site
            var callSiteScope = services.Scope.ResolveSymbol(context.ContainingSymbol);

            if (!IsConsumerScope(callSiteScope))
                return;

            // Check if we're inside a type constructor of the type itself
            if (IsWithinTypeConstructor(context.ContainingSymbol, operation.Type))
                return;

            // Report diagnostic
            var diagnostic = Diagnostic.Create(Rule, syntax.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static void AnalyzeFactoryInvocation(OperationAnalysisContext context, AnalyzerServices services)
        {
            if (context.Operation is not IInvocationOperation operation)
                return;

            var syntax = operation.Syntax;
            if (syntax == null)
                return;

            if (context.ContainingSymbol is IAssemblySymbol)
                return;

            if (IsKernelLikeAssembly(context.ContainingSymbol.ContainingAssembly?.Name))
                return;

            if (context.ContainingSymbol.ContainingAssembly?.Name?.StartsWith("Dx.Domain.", System.StringComparison.OrdinalIgnoreCase) == true)
                return;

            if (IsKernelLikeLocation(context.ContainingSymbol))
                return;

            if (IsKernelLikePath(syntax.SyntaxTree?.FilePath))
                return;

            if (IsAssemblyInfoFile(syntax.SyntaxTree?.FilePath))
                return;

            if (IsKernelAssembly(context.ContainingSymbol.ContainingAssembly))
                return;

            if (IsAttributeOperation(operation))
                return;

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

            if (!IsConsumerScope(callSiteScope))
                return;

            // Check if this is a Dx facade factory method
            if (services.Dx.IsDxFacadeFactory(operation.TargetMethod))
                return;

            // Report diagnostic
            var diagnostic = Diagnostic.Create(Rule, syntax.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsDomainType(ITypeSymbol? type, AnalyzerServices services)
        {
            if (type == null)
                return false;

            if (type.ContainingNamespace?.ToDisplayString() == "Dx.Domain.Annotations" && type.Name.EndsWith("Attribute", System.StringComparison.Ordinal))
                return false;

            if (type.Name == "DxAssemblyRoleAttribute")
                return false;

            if (type is INamedTypeSymbol named && InheritsFromException(named))
                return false;

            if (services.Semantic.IsKernelResultType(type) ||
                services.Semantic.IsDomainErrorType(type) ||
                services.Semantic.IsInvariantException(type))
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

        private static bool IsAttributeOperation(IOperation operation)
        {
            return operation.Syntax is AttributeSyntax ||
                   operation.Syntax is AttributeListSyntax ||
                   operation.Syntax?.AncestorsAndSelf().OfType<AttributeSyntax>().Any() == true;
        }

        private static bool IsConsumerScope(Scope scope) => scope == Scope.S3;

        private static bool IsKernelLikeAssembly(string? name)
        {
            return string.Equals(name, "Dx.Domain.Kernel", System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(name, "Dx.Domain.Primitives", System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(name, "Dx.Domain.Annotations", System.StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKernelLikePath(string? path)
        {
            return path != null &&
                   (path.IndexOf("Dx.Domain.Kernel", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    path.IndexOf("Dx.Domain.Primitives", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    path.IndexOf("Dx.Domain.Annotations", System.StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static bool IsAssemblyInfoFile(string? path)
        {
            return path != null && path.EndsWith("AssemblyInfo.cs", System.StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKernelLikeCompilation(Compilation compilation)
        {
            return compilation.SyntaxTrees.Any(tree => IsKernelLikePath(tree.FilePath));
        }

        private static bool IsKernelLikeLayer(AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!optionsProvider.GlobalOptions.TryGetValue("build_property.DxLayer", out var layer))
            {
                optionsProvider.GlobalOptions.TryGetValue("dx.layer", out layer);
            }

            if (string.IsNullOrWhiteSpace(layer))
                return false;

            return string.Equals(layer, "Kernel", System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(layer, "Primitives", System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(layer, "Annotations", System.StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKernelLikeLocation(ISymbol symbol)
        {
            return symbol.Locations.Any(location =>
                IsKernelLikePath(location.SourceTree?.FilePath));
        }

        private static bool InheritsFromException(INamedTypeSymbol type)
        {
            for (var current = type; current != null; current = current.BaseType)
            {
                if (current.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Exception")
                    return true;
            }

            return false;
        }

        private static bool IsKernelAssembly(IAssemblySymbol? assembly)
        {
            var name = assembly?.Name;
            return name != null &&
                   name.IndexOf("Dx.Domain.Kernel", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
