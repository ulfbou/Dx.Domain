// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IDxFacadeResolver.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers.Infrastructure.Facades
{
    /// <summary>
    /// Resolves the canonical <c>Dx</c> facade factories that are allowed to construct domain
    /// types in accordance with the analyzer contracts and rules.
    /// </summary>
    /// <remarks>
    /// This resolver is the single source of truth for DXA010/DXA011 and related rules.
    /// It discovers facade factory methods on the configured root facade type (defaults
    /// to <c>Dx</c>) and exposes them for analyzers and code fixes.
    /// </remarks>
    public interface IDxFacadeResolver
    {
        /// <summary>Gets the set of all discovered facade factory methods.</summary>
        IReadOnlyCollection<IMethodSymbol> FacadeFactories { get; }

        /// <summary>Returns <see langword="true"/> if the specified method is a known facade factory.</summary>
        bool IsDxFacadeFactory(IMethodSymbol method);

        /// <summary>
        /// Attempts to find a facade factory that produces the specified domain type.
        /// </summary>
        /// <param name="type">The candidate result type.</param>
        /// <returns>The first matching factory method, or <see langword="null"/> if none is found.</returns>
        IMethodSymbol? FindFacadeFactoryForType(ITypeSymbol type);
    }

    /// <summary>
    /// Default implementation that reflects over the <c>Dx</c> root facade in <c>Dx.Domain</c>
    /// (or a configured alternative) and collects public static factory methods.
    /// </summary>
    public sealed class DxFacadeResolver : IDxFacadeResolver
    {
        private readonly HashSet<IMethodSymbol> _methods =
            new(SymbolEqualityComparer.Default);

        /// <summary>
        /// Initializes a new instance of the <see cref="DxFacadeResolver"/> class.
        /// </summary>
        /// <param name="compilation">The compilation used to resolve the facade type.</param>
        /// <param name="config">Analyzer configuration options used to locate the root facade.</param>
        public DxFacadeResolver(Compilation compilation, AnalyzerConfigOptionsProvider config)
        {
            // Allow the root facade type to be overridden via EditorConfig, falling back to "Dx".
            var rootTypeName = GetRootFacadeTypeName(config) ?? "Dx";

            var dx = compilation.GetTypeByMetadataName(rootTypeName);
            if (dx == null)
                return;

            // We treat all public nested types of the root facade as logical namespaces
            // containing public static factory methods.
            foreach (var nested in dx.GetTypeMembers())
            {
                if (nested.DeclaredAccessibility != Accessibility.Public)
                    continue;

                foreach (var method in nested.GetMembers().OfType<IMethodSymbol>())
                {
                    if (method.DeclaredAccessibility != Accessibility.Public ||
                        !method.IsStatic)
                    {
                        continue;
                    }

                    // Future extension point: filter to methods returning Dx.Domain kernel types
                    // or Result<T>. For now, record all public static methods and let
                    // individual rules apply additional constraints.
                    _methods.Add(method);
                }
            }
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IMethodSymbol> FacadeFactories => _methods;

        /// <inheritdoc />
        public bool IsDxFacadeFactory(IMethodSymbol method) =>
            _methods.Contains(method);

        /// <inheritdoc />
        public IMethodSymbol? FindFacadeFactoryForType(ITypeSymbol type) =>
            _methods.FirstOrDefault(m =>
                SymbolEqualityComparer.Default.Equals(m.ReturnType, type));

        private static string? GetRootFacadeTypeName(AnalyzerConfigOptionsProvider config)
        {
            // Config key aligned with docs: "dx_facade_root". When not present, callers fall
            // back to the conventional root type name "Dx".
            var global = config.GlobalOptions;
            return global.TryGetValue("dx_facade_root", out var value) && !string.IsNullOrWhiteSpace(value)
                ? value.Trim()
                : null;
        }
    }
}
