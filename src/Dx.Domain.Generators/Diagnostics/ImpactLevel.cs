// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ImpactLevel.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Generators.Diagnostics
{
    /// <summary>
    /// Defines the impact level of a failure or remediation.
    /// </summary>
    public enum ImpactLevel
    {
        /// <summary>
        /// No behavior change - safe to auto-apply.
        /// </summary>
        Safe,

        /// <summary>
        /// Behavior change that is reversible.
        /// </summary>
        Behavioral,

        /// <summary>
        /// Public API or semantic breaking change.
        /// </summary>
        Breaking
    }
}
