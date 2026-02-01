// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dxk006_DomainFactBoundaryAnalyzer.cs" company="Dx.Domain Team">
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
using Dx.Domain.Analyzers.Infrastructure.Boundaries;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Roles;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class Dxk006_DomainFactBoundaryAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Dxk.DXK006);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(startContext =>
            {
                var role = RoleResolver.Resolve(startContext.Compilation);
                if (role is null or DxAssemblyRole.Contracts or DxAssemblyRole.Shared)
                    return;

                var resolver = new DomainFactResolver(startContext.Compilation);
                if (resolver.DomainFactType == null)
                    return;

                var boundaryDetector = new OutboxBoundaryDetector();
                var generated = new GeneratedCodeDetector(startContext.Options.AnalyzerConfigOptionsProvider);

                startContext.RegisterSymbolAction(
                    ctx => AnalyzeMethod(ctx, resolver, boundaryDetector, generated),
                    SymbolKind.Method);
            });
        }

        private static void AnalyzeMethod(
            SymbolAnalysisContext context,
            IDomainFactResolver resolver,
            OutboxBoundaryDetector boundaryDetector,
            GeneratedCodeDetector generated)
        {
            var method = (IMethodSymbol)context.Symbol;

            if (method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet)
                return;

            if (method.IsImplicitlyDeclared || !method.Locations.Any(l => l.IsInSource))
                return;

            if (generated.IsGenerated(method))
                return;

            if (!OutboxBoundaryDetector.IsBoundary(method))
                return;

            foreach (var parameter in method.Parameters)
            {
                if (IsDomainFactCarrier(parameter.Type, resolver))
                    continue;

                context.ReportDiagnostic(Diagnostic.Create(Dxk.DXK006, parameter.Locations.FirstOrDefault()));
            }

            if (method.ReturnsVoid)
                return;

            if (IsDomainFactCarrier(method.ReturnType, resolver))
                return;

            context.ReportDiagnostic(Diagnostic.Create(Dxk.DXK006, method.Locations.FirstOrDefault()));
        }

        private static bool IsDomainFactCarrier(ITypeSymbol type, IDomainFactResolver resolver)
        {
            if (resolver.IsDomainFact(type))
                return true;

            if (type is IArrayTypeSymbol arrayType)
                return IsDomainFactCarrier(arrayType.ElementType, resolver);

            if (type is INamedTypeSymbol named)
            {
                if (named.IsGenericType)
                    return named.TypeArguments.Any(t => IsDomainFactCarrier(t, resolver));

                if (named.MetadataName == "Nullable`1")
                    return IsDomainFactCarrier(named.TypeArguments[0], resolver);
            }

            return false;
        }
    }
}
