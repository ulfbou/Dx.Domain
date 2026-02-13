// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dxk004_PrimitiveObsessionAnalyzer.cs" company="Dx.Domain Team">
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
using Dx.Domain.Analyzers.Infrastructure.Primitives;
using Dx.Domain.Analyzers.Infrastructure.Scopes;
using Dx.Domain.Analyzers.Roles;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class Dxk004_PrimitiveObsessionAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Dxk.DXK004);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(startContext =>
            {
                var scopeResolver = new ScopeResolver(startContext.Options.AnalyzerConfigOptionsProvider);
                if (scopeResolver.ResolveAssembly(startContext.Compilation.Assembly) != Scope.S3)
                    return;

                var role = RoleResolver.Resolve(startContext.Compilation, startContext.Options.AnalyzerConfigOptionsProvider);
                if (role is null or DxAssemblyRole.Contracts or DxAssemblyRole.Shared)
                    return;

                var catalog = PrimitiveCatalog.Create(startContext.Compilation);
                var generated = new GeneratedCodeDetector(startContext.Options.AnalyzerConfigOptionsProvider);

                startContext.RegisterSymbolAction(ctx => AnalyzeMethod(ctx, catalog, generated), SymbolKind.Method);
                startContext.RegisterSymbolAction(ctx => AnalyzeProperty(ctx, catalog, generated), SymbolKind.Property);
                startContext.RegisterSymbolAction(ctx => AnalyzeField(ctx, catalog, generated), SymbolKind.Field);
            });
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context, PrimitiveCatalog catalog, GeneratedCodeDetector generated)
        {
            var method = (IMethodSymbol)context.Symbol;

            if (method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet)
                return;

            if (method.IsImplicitlyDeclared || !method.Locations.Any(l => l.IsInSource))
                return;

            if (generated.IsGenerated(method))
                return;

            if (method.ReturnsVoid == false)
            {
                ReportIfPrimitive(context, catalog, method.Name, method.ReturnType, method.Locations.FirstOrDefault());
            }

            foreach (var parameter in method.Parameters)
            {
                ReportIfPrimitive(context, catalog, parameter.Name, parameter.Type, parameter.Locations.FirstOrDefault());
            }
        }

        private static void AnalyzeProperty(SymbolAnalysisContext context, PrimitiveCatalog catalog, GeneratedCodeDetector generated)
        {
            var property = (IPropertySymbol)context.Symbol;

            if (property.IsImplicitlyDeclared || !property.Locations.Any(l => l.IsInSource))
                return;

            if (generated.IsGenerated(property))
                return;

            ReportIfPrimitive(context, catalog, property.Name, property.Type, property.Locations.FirstOrDefault());
        }

        private static void AnalyzeField(SymbolAnalysisContext context, PrimitiveCatalog catalog, GeneratedCodeDetector generated)
        {
            var field = (IFieldSymbol)context.Symbol;

            if (field.IsImplicitlyDeclared || !field.Locations.Any(l => l.IsInSource))
                return;

            if (generated.IsGenerated(field))
                return;

            ReportIfPrimitive(context, catalog, field.Name, field.Type, field.Locations.FirstOrDefault());
        }

        private static void ReportIfPrimitive(
            SymbolAnalysisContext context,
            PrimitiveCatalog catalog,
            string symbolName,
            ITypeSymbol type,
            Location? location)
        {
            if (location == null)
                return;

            if (!catalog.TryGetReplacement(type, symbolName, out var replacement))
                return;

            var diagnostic = Diagnostic.Create(
                Dxk.DXK004,
                location,
                replacement.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));

            context.ReportDiagnostic(diagnostic);
        }
    }
}
