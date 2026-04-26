// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA011_PublicFactoryExposureAnalyzer.cs" company="Dx.Domain Team">
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

using Dx.Domain.Analyzers.Infrastructure;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers.Analyzers
{

    /// <summary>
    /// Enforces Dx.Domain rule DXA011: Public Factory Exposure.
    /// </summary>
    /// <remarks>
    /// This analyzer enforces a compile-time architectural invariant:
    /// domain types must not expose public constructors or public static factory methods.
    ///
    /// Scope:
    /// - Applies to S0 and S1 domain types.
    /// - Generated code is ignored.
    ///
    /// Enforcement model:
    /// - Violations produce diagnostics.
    /// - No automatic fixes are applied.
    ///
    /// DX-first principle:
    /// Construction surfaces are restricted to preserve creation authority.
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA011_PublicFactoryExposureAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Diagnostic contract identifier for DXA011.
        /// </summary>
        public const string DiagnosticId = "DXA011";
        private const string Category = "Domain.Architecture";

        private static readonly LocalizableString Title =
            "Public Factory Exposure";
        private static readonly LocalizableString MessageFormat =
            "Public construction surface on domain type detected. Make constructor/factory internal and expose creation via Dx facade.";
        private static readonly LocalizableString Description =
            "Domain types should not expose public constructors or factories to prevent consumers from bypassing the Dx facade.";

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

                startContext.RegisterSymbolAction(symbolContext =>
                {
                    AnalyzeNamedType(symbolContext, services);
                }, SymbolKind.NamedType);
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

        private static void AnalyzeNamedType(SymbolAnalysisContext context, AnalyzerServices services)
        {
            var type = (INamedTypeSymbol)context.Symbol;

            // 1. Kernel types are implementation, not subject to DXA011
            if (services.Scope.IsKernelInternal(context.Compilation.Assembly))
                return;

            // 2. Generated code – SymbolAnalysisContext has no IsGeneratedCode, use the detector
            if (services.Generated.IsGenerated(type))
                return;

            // Only analyze S0 and S1 scopes (kernel and domain)
            // S0 (kernel) is WHERE factories are typically defined and should be checked
            // S1 (domain) also needs to be checked for domain types
            var scope = services.Scope.ResolveSymbol(type);
            if (scope != Scope.S0 && scope != Scope.S1)
                return;

            // Use centralized semantic classifier for domain type detection
            if (!services.Semantic.IsDomainType(type))
                return;

            bool hasPublicConstructor = false;
            bool hasPublicFactory = false;

            // Check public constructors
            foreach (var constructor in type.Constructors)
            {
                // Skip static constructors (.cctor)
                if (constructor.IsStatic)
                    continue;

                if (constructor.DeclaredAccessibility == Accessibility.Public)
                {
                    hasPublicConstructor = true;
                    if (constructor.Locations.Any())
                    {
                        var location = constructor.Locations.First();
                        context.ReportDiagnostic(Diagnostic.Create(Rule, location));
                    }
                }
            }

            // Check public static factory methods
            foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.DeclaredAccessibility == Accessibility.Public &&
                    method.IsStatic &&
                    !method.IsExtensionMethod &&
                    IsFactoryMethod(method, type))
                {
                    hasPublicFactory = true;
                    if (method.Locations.Any())
                    {
                        var location = method.Locations.First();
                        context.ReportDiagnostic(Diagnostic.Create(Rule, location));
                    }
                }
            }

            // Check for orphaned domain types (no public creation path and no facade)
            if (!hasPublicConstructor && !hasPublicFactory)
            {
                var hasFacade = services.Dx.FindFacadeFactoryForType(type) != null;
                if (!hasFacade && type.Locations.Any())
                {
                    var location = type.Locations.First();
                    context.ReportDiagnostic(Diagnostic.Create(Rule, location));
                }
            }
        }

        private static bool IsFactoryMethod(IMethodSymbol method, INamedTypeSymbol containingType)
        {
            // Factory method returns the containing type or related type
            if (SymbolEqualityComparer.Default.Equals(method.ReturnType, containingType))
                return true;

            // Check if it returns a constructed version of the type (e.g., generic type)
            if (method.ReturnType is INamedTypeSymbol returnType &&
                SymbolEqualityComparer.Default.Equals(returnType.ConstructedFrom, containingType))
                return true;

            // Check if it returns Result<T> where T is the containing type
            // This catches factories that return Result-wrapped domain objects
            if (method.ReturnType is INamedTypeSymbol namedReturn &&
                namedReturn.IsGenericType &&
                namedReturn.TypeArguments.Length > 0)
            {
                var firstArg = namedReturn.TypeArguments[0];
                if (SymbolEqualityComparer.Default.Equals(firstArg, containingType))
                    return true;
            }

            // Common factory method names - these are strong indicators
            var methodName = method.Name;
            return methodName.StartsWith("Create", System.StringComparison.Ordinal) ||
                   methodName.StartsWith("From", System.StringComparison.Ordinal) ||
                   methodName.StartsWith("Parse", System.StringComparison.Ordinal) ||
                   methodName.StartsWith("TryParse", System.StringComparison.Ordinal) ||
                   methodName == "New" ||
                   methodName == "Of";
        }
    }
}
