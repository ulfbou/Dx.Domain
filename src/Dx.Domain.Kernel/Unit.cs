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
    /// Unit type used for Result operations that return no value.
    /// </summary>
    [DebuggerDisplay("Unit")]
    [ApprovedKernelApi("Void-like type for operations with no return value")]
    public readonly record struct Unit
    {
        /// <summary>Gets the singleton value of the Unit type.</summary>
        public static Unit Value => default;
    }
}
