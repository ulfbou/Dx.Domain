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

namespace Dx.Domain.Analyzers.Infrastructure.Facades
{
    /// <summary>
    /// Resolves the canonical <c>Dx</c> facade factories that are allowed to construct domain
    /// types in accordance with the analyzer contracts and rules.
    /// </summary>
    /// <remarks>
    /// This resolver is the single source of truth for DXA010/DXA011 and related rules.
    /// It discovers facade factory methods on the configured root facade type (defaults
    /// to <c>Dx.Dx</c>) and exposes them for analyzers and code fixes.
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
}
