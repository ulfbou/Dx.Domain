// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IScopeResolver.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Infrastructure.Scopes
{
    /// <summary>
    /// Defines a contract for resolving architectural scope for assemblies and symbols.
    /// </summary>
    /// <remarks>
    /// This interface is used exclusively by analyzers to enforce layering rules. It carries analysis configuration only and imposes no runtime semantics outside compilation analysis.
    /// </remarks>
    public interface IScopeResolver
    {
        /// <summary>
        /// Resolves the architectural scope for the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly symbol to resolve.</param>
        /// <returns>The <see cref="Scope"/> assigned to the assembly.</returns>
        Scope ResolveAssembly(IAssemblySymbol assembly);

        /// <summary>
        /// Resolves the architectural scope for the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to resolve.</param>
        /// <returns>The <see cref="Scope"/> assigned to the symbol.</returns>
        Scope ResolveSymbol(ISymbol symbol);

        /// <summary>
        /// Determines whether the specified assembly is marked as kernel-internal.
        /// </summary>
        /// <param name="assembly">The assembly symbol to examine.</param>
        /// <returns><see langword="true"/> if the assembly is kernel-internal; otherwise, <see langword="false"/>.</returns>
        bool IsKernelInternal(IAssemblySymbol assembly);
    }
}
