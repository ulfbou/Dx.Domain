// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FileName.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Contracts;

namespace Dx.Domain.Kernel.Errors
{
    /// <summary>
    /// Represents detailed diagnostic information for a violated invariant.
    /// </summary>
    /// <param name="Code">The unique error code identifying the invariant violation.</param>
    /// <param name="Message">A descriptive message detailing the invariant violation.</param>
    // DPI: This record is used internally to encapsulate invariant violation details,
    internal sealed record InvariantError(
        string Code,
        string Message
    ) : IInvariantError;
}
