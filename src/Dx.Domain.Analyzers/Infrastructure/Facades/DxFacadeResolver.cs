// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxFacadeResolver.cs" company="Dx.Domain Team">
// Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
// This software is licensed under the MIT License.
// See the project's root <c>LICENSE</c> file for details.
// Contributions are welcome, subject to the terms of the project's license.
// See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using System.Linq;

namespace Dx.Domain.Analyzers.Infrastructure.Facades
{
    /// <summary>
    /// Default implementation that reflects over the <c>Dx</c> root facade in <c>Dx.Domain</c>
    /// (or a configured alternative) and collects public static factory methods.
    /// Also supports types marked with [DxFacade] attribute per AC3.
    /// </summary>
    public sealed class DxFacadeResolver : IDxFacadeResolver
    {
        private readonly HashSet<IMethodSymbol> _methods =
            new(SymbolEqualityComparer.Default);

        public DxFacadeResolver(Compilation compilation, AnalyzerConfigOptionsProvider config)
        {
            // 1. Configured root facade (AC3 - config path)
            var rootTypeName = GetRootFacadeTypeName(config) ?? "Dx.Dx";
            var dx = compilation.GetTypeByMetadataName(rootTypeName);
            dx ??= compilation.GetTypeByMetadataName("Dx");

            if (dx != null)
            {
                AddMethodsFromRoot(dx);
            }

            // 2. Attribute-based facades (AC3 - attribute path)
            AddMethodsFromAttributedTypes(compilation);
        }

        public IReadOnlyCollection<IMethodSymbol> FacadeFactories => _methods;

        public bool IsDxFacadeFactory(IMethodSymbol method) =>
            _methods.Contains(method);

        public IMethodSymbol? FindFacadeFactoryForType(ITypeSymbol type) =>
            _methods.FirstOrDefault(m =>
                SymbolEqualityComparer.Default.Equals(m.ReturnType, type));

        private void AddMethodsFromRoot(INamedTypeSymbol root)
        {
            foreach (var nested in root.GetTypeMembers())
            {
                if (nested.DeclaredAccessibility != Accessibility.Public)
                    continue;

                AddPublicStaticMethods(nested);
            }
        }

        private void AddMethodsFromAttributedTypes(Compilation compilation)
        {
            // Correct namespace from Dx.Domain.Annotations
            var facadeAttr = compilation.GetTypeByMetadataName(
                "Dx.Domain.Annotations.DxFacadeAttribute");

            if (facadeAttr == null)
                return;

            foreach (var tree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(tree);
                var types = tree.GetRoot().DescendantNodes()
                   .Where(n => n is Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax)
                   .Select(n => model.GetDeclaredSymbol(n) as INamedTypeSymbol)
                   .Where(s => s != null);

                foreach (var type in types!)
                {
                    if (type!.GetAttributes().Any(a =>
                        SymbolEqualityComparer.Default.Equals(a.AttributeClass, facadeAttr)))
                    {
                        AddPublicStaticMethods(type);
                    }
                }
            }
        }

        private void AddPublicStaticMethods(INamedTypeSymbol type)
        {
            foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.DeclaredAccessibility == Accessibility.Public && method.IsStatic)
                {
                    _methods.Add(method);
                }
            }
        }

        private static string? GetRootFacadeTypeName(AnalyzerConfigOptionsProvider config)
        {
            var global = config.GlobalOptions;
            return global.TryGetValue("dx_facade_root", out var value) && !string.IsNullOrWhiteSpace(value)
               ? value.Trim()
                : null;
        }
    }
}
