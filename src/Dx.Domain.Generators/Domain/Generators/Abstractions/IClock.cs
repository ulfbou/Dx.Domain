// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IClock.cs" company="Dx.Domain Team">
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
    /// Provides deterministic time access for generator stages.
    /// Replaces DateTime.Now to ensure referential transparency (DX-001).
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Gets the current UTC time in a deterministic manner.
        /// This value is derived from the InputFingerprint or build timestamp,
        /// ensuring identical outputs for identical inputs.
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }
}
