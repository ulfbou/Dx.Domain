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
using Dx.Domain.Analyzers.Infrastructure.Semantics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers.Analyzers
{
    /// <summary>
    /// Analyzer for DXA011: Public Factory Exposure.
    /// Detects public constructors or public static factory methods on domain types.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA011_PublicFactoryExposureAnalyzer : DiagnosticAnalyzer
    {
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

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

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

            // Skip if generated code
            if (services.Generated.IsGenerated(type))
                return;

            // Only analyze S0 and S1 scopes (kernel and domain)
            var scope = services.Scope.ResolveSymbol(type);
            if (scope != Scope.S0 && scope != Scope.S1)
                return;

            // Skip if not a domain type (basic heuristic: in Dx.Domain namespace or similar)
            if (!IsDomainType(type))
                return;

            // Check public constructors
            foreach (var constructor in type.Constructors)
            {
                if (constructor.DeclaredAccessibility == Accessibility.Public &&
                    !constructor.IsStatic &&
                    constructor.Locations.Any())
                {
                    var location = constructor.Locations.First();
                    context.ReportDiagnostic(Diagnostic.Create(Rule, location));
                }
            }

            // Check public static factory methods
            foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.DeclaredAccessibility == Accessibility.Public &&
                    method.IsStatic &&
                    !method.IsExtensionMethod &&
                    IsFactoryMethod(method, type) &&
                    method.Locations.Any())
                {
                    var location = method.Locations.First();
                    context.ReportDiagnostic(Diagnostic.Create(Rule, location));
                }
            }
        }

        private static bool IsDomainType(INamedTypeSymbol type)
        {
            // Heuristic: check if type is in a domain namespace
            var ns = type.ContainingNamespace?.ToDisplayString();
            if (ns == null)
                return false;

            return ns.Contains("Dx.Domain") || 
                   ns.Contains(".Domain") ||
                   ns.Contains(".Domains");
        }

        private static bool IsFactoryMethod(IMethodSymbol method, INamedTypeSymbol containingType)
        {
            // Factory method returns the containing type or related type
            if (SymbolEqualityComparer.Default.Equals(method.ReturnType, containingType))
                return true;

            // Check if it returns a constructed version of the type
            if (method.ReturnType is INamedTypeSymbol returnType &&
                SymbolEqualityComparer.Default.Equals(returnType.ConstructedFrom, containingType))
                return true;

            // Common factory method names
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
