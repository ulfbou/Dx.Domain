// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ISemanticClassifier.cs" company="Dx.Domain Team">
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

using System;
using System.Linq;

namespace Dx.Domain.Analyzers.Infrastructure.Facades
{
    /// <summary>
    /// Defines a contract for classifying semantic types during analysis.
    /// </summary>
    /// <remarks>
    /// This interface imposes no runtime semantics. It is interpreted exclusively by analyzers to identify kernel and domain types.
    /// </remarks>
    public interface ISemanticClassifier
    {
        /// <summary>
        /// Determines whether the specified type represents a kernel Result type.
        /// </summary>
        /// <param name="type">The type symbol to examine. Must not be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the type is a kernel Result type; otherwise, <see langword="false"/>.</returns>
        bool IsKernelResultType(ITypeSymbol type);

        /// <summary>
        /// Determines whether the specified type represents a domain error type.
        /// </summary>
        /// <param name="type">The type symbol to examine. Must not be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the type is a domain error type; otherwise, <see langword="false"/>.</returns>
        bool IsDomainErrorType(ITypeSymbol type);

        /// <summary>
        /// Determines whether the specified type represents an invariant exception.
        /// </summary>
        /// <param name="type">The type symbol to examine. Must not be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the type is an invariant exception; otherwise, <see langword="false"/>.</returns>
        bool IsInvariantException(ITypeSymbol type);

        /// <summary>
        /// Determines whether the specified type represents a domain type.
        /// </summary>
        /// <param name="type">The type symbol to examine. Must not be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the type is a domain type; otherwise, <see langword="false"/>.</returns>
        bool IsDomainType(ITypeSymbol type);
    }
}
