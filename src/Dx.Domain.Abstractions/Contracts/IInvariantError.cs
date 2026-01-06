// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IInvariantError.cs" company="Dx.Domain Team">
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
    /// Defines the contract for representing invariant errors within the domain.
    /// </summary>
    // DPI: This interface is intended to abstract the details of invariant errors,
    // allowing different implementations to represent invariant violations consistently.
    public interface IInvariantError
    {
        /// <summary>Gets the unique error code associated with the invariant violation.</summary>
        string Code { get; }

        /// <summary>Gets the descriptive message detailing the invariant violation.</summary>
        string Message { get; }
    }
}
