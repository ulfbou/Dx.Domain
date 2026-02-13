// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA040_KernelPublicSurfaceFreezeAnalyzer.cs" company="Dx.Domain Team">
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
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers
{
    /// <summary>
    /// Analyzer for DXA040: Kernel Public Surface Freeze.
    /// Detects new public API additions in kernel assemblies without DPI justification.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA040_KernelPublicSurfaceFreezeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DxRuleIds.DXA040;
        private const string Category = DxCategories.DomainArchitecture;

        private static readonly LocalizableString Title =
            "Kernel Public Surface Freeze";
        private static readonly LocalizableString MessageFormat =
            "New public kernel API detected. Provide DPI justification and confirm it cannot live at the edges.";
        private static readonly LocalizableString Description =
            "Kernel API surface should remain minimal. New public types or members require DPI justification to prevent kernel drift.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
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
                var scopeResolver = new ScopeResolver(startContext.Options.AnalyzerConfigOptionsProvider);
                if (scopeResolver.ResolveAssembly(startContext.Compilation.Assembly) != Scope.S0)
                    return;

                if (IsTestProject(startContext.Options.AnalyzerConfigOptionsProvider))
                    return;

                if (!IsKernelFreezeEnabled(startContext.Options.AnalyzerConfigOptionsProvider))
                    return;

                var services = CreateServices(startContext);

                startContext.RegisterSymbolAction(symbolContext =>
                {
                    AnalyzeSymbol(symbolContext, services);
                }, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Field);
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

        private static void AnalyzeSymbol(SymbolAnalysisContext context, AnalyzerServices services)
        {
            var symbol = context.Symbol;

            // Skip if generated code
            if (services.Generated.IsGenerated(symbol))
                return;

            var scope = services.Scope.ResolveSymbol(symbol);
            if (scope != Scope.S0)
                return;

            // Only care about public symbols
            if (symbol.DeclaredAccessibility != Accessibility.Public)
                return;

            // Check if symbol has DPI justification attribute
            // In a real implementation, this would check for a custom attribute like [DpiJustified]
            // or check for metadata in PR/commit messages
            if (HasDpiJustification(symbol))
                return;

            // For now, we'll do a simple heuristic: flag new public APIs
            // In production, this would integrate with API diff tooling in CI
            if (symbol.Locations.Any())
            {
                var location = symbol.Locations.First();
                context.ReportDiagnostic(Diagnostic.Create(Rule, location));
            }
        }

        private static bool HasDpiJustification(ISymbol symbol)
        {
            var assemblyAttributes = symbol.ContainingAssembly?.GetAttributes() ?? ImmutableArray<AttributeData>.Empty;
            if (HasJustificationAttributes(assemblyAttributes))
                return true;

            if (symbol.ContainingType != null && HasJustificationAttributes(symbol.ContainingType.GetAttributes()))
                return true;

            foreach (var attribute in symbol.GetAttributes())
            {
                var name = attribute.AttributeClass?.Name;
                if (name != "DpiJustifiedAttribute" &&
                    name != "KernelApiAttribute" &&
                    name != "ApprovedKernelApiAttribute")
                {
                    continue;
                }

                if (attribute.ConstructorArguments.Length == 0)
                    return true;

                var arg = attribute.ConstructorArguments[0].Value as string;
                if (!string.IsNullOrWhiteSpace(arg))
                    return true;
            }

            return false;
        }

        private static bool HasJustificationAttributes(ImmutableArray<AttributeData> attributes)
        {
            foreach (var attribute in attributes)
            {
                var name = attribute.AttributeClass?.Name;
                if (name != "DpiJustifiedAttribute" &&
                    name != "KernelApiAttribute" &&
                    name != "ApprovedKernelApiAttribute")
                {
                    continue;
                }

                if (attribute.ConstructorArguments.Length == 0)
                    return true;

                var arg = attribute.ConstructorArguments[0].Value as string;
                if (!string.IsNullOrWhiteSpace(arg))
                    return true;
            }

            return false;
        }

        private static bool IsKernelFreezeEnabled(AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (optionsProvider.GlobalOptions.TryGetValue("build_property.DxKernelApiFreeze", out var raw) &&
                bool.TryParse(raw, out var enabled))
            {
                return enabled;
            }

            if (optionsProvider.GlobalOptions.TryGetValue("dx.kernel.freeze", out raw) &&
                bool.TryParse(raw, out enabled))
            {
                return enabled;
            }

            return false;
        }

        private static bool IsTestProject(AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (optionsProvider.GlobalOptions.TryGetValue("build_property.IsTestProject", out var raw) &&
                bool.TryParse(raw, out var isTest))
            {
                return isTest;
            }

            return false;
        }
    }
}
