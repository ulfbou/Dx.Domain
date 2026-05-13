// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Scope.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Analyzers.Infrastructure.Scopes
{
    /// <summary>
    /// Defines architectural scope classification vocabulary for analyzers.
    /// </summary>
    /// <remarks>
    /// This enumeration imposes no runtime semantics. It is pure analyzer metadata interpreted exclusively by
    /// Dx.Domain analyzers. Values are part of the public analyzer contract and must remain stable.
    /// </remarks>
    public enum Scope
    {
        /// <summary>
        /// Represents the kernel layer (S0).
        /// </summary>
        S0 = 0,

        /// <summary>
        /// Represents the shared layer (S1).
        /// </summary>
        S1 = 1,

        /// <summary>
        /// Represents the domain layer (S2).
        /// </summary>
        S2 = 2,

        /// <summary>
        /// Represents the application layer (S3).
        /// </summary>
        S3 = 3
    }
}
