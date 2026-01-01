// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IDomainSymbol.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Contracts
{
    /// <summary>
    /// Represents a named symbol within a domain-specific model.
    /// </summary>
    /// <remarks>Implementations of this interface typically provide additional metadata or behavior
    /// associated with the symbol. The interface is intended for use in scenarios where symbols are identified by name,
    /// such as language models, code analysis, or domain-driven design.</remarks>
    public interface IDomainSymbol
    {
        /// <summary>
        /// Gets the name associated with the current instance.
        /// </summary>
        string Name { get; }
    }
}
