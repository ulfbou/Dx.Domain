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
using System.Linq;

using Dx.Domain.Annotations;
using Dx.Domain.Analyzers.Infrastructure;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Flow;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers
{
    /// <summary>
    /// Analyzer for DXA022: Discouraged Domain Control Exception.
    /// Detects methods that return Result&lt;T&gt; but throw domain control exceptions instead of returning Result.Failure.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA022_DomainControlExceptionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DxRuleIds.DXA022;
        private const string Category = DxCategories.DomainExceptionHandling;

        private static readonly LocalizableString Title =
            "Discouraged Domain Control Exception";
        private static readonly LocalizableString MessageFormat =
            "Use Result.Failure instead of throwing exception in Result-returning method.";
        private static readonly LocalizableString Description =
            "Methods that return Result should use Result.Failure instead of throwing exceptions for domain control flow.";

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
                    scope != Scope.S3 || IsKernelLikeAssembly(assemblyName) || IsKernelLikeCompilation(startContext.Compilation))
                    return;

                var services = CreateServices(startContext);

                startContext.RegisterOperationAction(operationContext =>
                {
                    AnalyzeThrow(operationContext, services);
                }, OperationKind.Throw);
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

        private static void AnalyzeThrow(OperationAnalysisContext context, AnalyzerServices services)
        {
            var throwOperation = (IThrowOperation)context.Operation;

            if (IsKernelLikeAssembly(context.ContainingSymbol.ContainingAssembly?.Name))
                return;

            if (IsKernelLikeLocation(context.ContainingSymbol))
                return;

            var syntax = throwOperation.Syntax;
            if (syntax == null)
                return;

            if (IsKernelLikePath(syntax.SyntaxTree?.FilePath))
                return;

            // Skip if generated code
            if (throwOperation.Exception?.Type != null &&
                services.Generated.IsGenerated(throwOperation.Exception.Type))
                return;

            var scope = services.Scope.ResolveSymbol(context.ContainingSymbol);
            if (scope != Scope.S3)
                return;

            // Check if we're in a method that returns Result
            if (context.ContainingSymbol is not IMethodSymbol method)
                return;

            if (!services.Semantic.IsKernelResultType(method.ReturnType))
                return;

            if (!IsContractFacing(method))
                return;

            // Classify the exception intent
            var intent = services.Exceptions.Classify(throwOperation);

            // Allow argument validation and invariant violations
            if (intent == ExceptionIntent.ArgumentValidation ||
                intent == ExceptionIntent.InvariantViolation ||
                intent == ExceptionIntent.ControlFlow)
                return;

            // Allow rethrows (throw; with no expression)
            if (throwOperation.Exception == null)
                return;

            // Report diagnostic for domain control or unknown exceptions in Result-returning methods
            if (intent == ExceptionIntent.DomainControl)
            {
                var diagnostic = Diagnostic.Create(Rule, syntax.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

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

        private static bool IsContractFacing(IMethodSymbol method)
        {
            return method.DeclaredAccessibility is Accessibility.Public or
                   Accessibility.Protected or
                   Accessibility.ProtectedOrInternal;
        }
    }
}
