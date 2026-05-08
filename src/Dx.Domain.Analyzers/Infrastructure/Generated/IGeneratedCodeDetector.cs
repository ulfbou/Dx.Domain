// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IGeneratedCodeDetector.cs" company="Dx.Domain Team">
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
using System.Collections.Generic;
using System.Linq;

namespace Dx.Domain.Analyzers.Infrastructure.Generated
{
    /// <summary>
    /// Defines a contract for detecting symbols that originate from generated code.
    /// </summary>
    /// <remarks>
    /// This interface is used exclusively by analyzers to suppress diagnostics on generated sources.
    /// It carries analysis configuration only and imposes no runtime semantics outside compilation analysis.
    /// </remarks>
    public interface IGeneratedCodeDetector
    {
        /// <summary>
        /// Determines whether the specified symbol originates from generated code.
        /// </summary>
        /// <param name="symbol">The symbol to examine.</param>
        /// <returns><see langword="true"/> if the symbol is marked with a generated code attribute or matches a configured namespace marker; otherwise, <see langword="false"/>.</returns>
        bool IsGenerated(ISymbol symbol);
    }
}
