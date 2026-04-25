// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ResultState.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Defines the lifecycle states tracked for Result values during flow analysis.
    /// </summary>
    public enum ResultState
    {
        /// <summary>Result value has been created but not yet observed.</summary>
        Created = 0,

        /// <summary>Result value has been inspected via IsSuccess, IsFailure, or pattern matching.</summary>
        Checked = 1,

        /// <summary>Result value has been propagated to another operation or returned.</summary>
        Propagated = 2,

        /// <summary>Result value has been consumed by a terminal handler.</summary>
        Terminated = 3,

        /// <summary>Result value was created and never observed.</summary>
        Ignored = 4
    }
}
