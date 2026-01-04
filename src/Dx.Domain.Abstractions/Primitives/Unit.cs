// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Unit.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Diagnostics;

namespace Dx.Domain
{
    /// <summary>
    /// Represents the absence of a value.
    /// Used to unify generic and non-generic results.
    /// </summary>
    public readonly struct Unit
    {
        /// <summary>
        /// The single valid value of <see cref="Unit"/>.
        /// </summary>
        public static readonly Unit Value = default;
    }
}
