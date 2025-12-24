// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="TrustZone.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Generators.Trust
{
    /// <summary>
    /// Defines trust zones for artifacts in the generator system (Section 5 of specification).
    /// </summary>
    public enum TrustZone
    {
        /// <summary>
        /// Authoritative: Signed generators, policies, CI keys.
        /// Must be signed.
        /// </summary>
        Authoritative,

        /// <summary>
        /// Declarative: Source intent, manifests.
        /// Must be hashed into InputFingerprint.
        /// </summary>
        Declarative,

        /// <summary>
        /// Derived: Generated artifacts, golden tests.
        /// Must not be used as generator inputs.
        /// </summary>
        Derived
    }
}
