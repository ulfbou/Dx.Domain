// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IDeterministicIdentity.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Provides deterministic identity generation for generator stages.
    /// Replaces Guid.NewGuid() to ensure referential transparency (DX-001).
    /// </summary>
    public interface IDeterministicIdentity
    {
        /// <summary>
        /// Generates a deterministic GUID based on the input fingerprint and a seed.
        /// For the same fingerprint and seed, this always returns the same GUID.
        /// </summary>
        /// <param name="seed">A seed value to differentiate multiple IDs in the same context.</param>
        /// <returns>A deterministically generated GUID.</returns>
        Guid NewGuid(string seed);
    }
}
