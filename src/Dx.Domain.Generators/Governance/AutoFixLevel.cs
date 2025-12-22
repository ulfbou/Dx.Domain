// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AutoFixLevel.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Generators.Governance
{
    /// <summary>
    /// Defines automation levels for fix application (Section 6 of specification).
    /// </summary>
    public enum AutoFixLevel
    {
        /// <summary>
        /// No automatic fixes - only report diagnostics.
        /// </summary>
        None,

        /// <summary>
        /// Suggest fixes but do not apply them.
        /// </summary>
        Suggest,

        /// <summary>
        /// Automatically apply only safe fixes (no behavior change).
        /// </summary>
        Safe,

        /// <summary>
        /// Automatically apply all fixes including behavioral changes.
        /// </summary>
        Apply
    }
}
