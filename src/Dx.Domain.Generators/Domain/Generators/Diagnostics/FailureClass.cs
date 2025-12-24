// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FailureClass.cs" company="Dx.Domain Team">
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
    /// Defines the mandatory failure classification taxonomy (Section 2 of specification).
    /// All failures must be classified into exactly one failure class.
    /// </summary>
    public enum FailureClass
    {
        /// <summary>
        /// DX2xxx: Authored intent violates required invariants or domain rules.
        /// </summary>
        IntentViolation,

        /// <summary>
        /// DX3xxx: Intent is valid but disallowed by applied policy.
        /// </summary>
        PolicyViolation,

        /// <summary>
        /// DX4xxx: Generator cannot deterministically infer required contract.
        /// </summary>
        InferenceFailure,

        /// <summary>
        /// DX5xxx: Generated change would violate public API or semantic versioning rules.
        /// </summary>
        CompatibilityFailure,

        /// <summary>
        /// DX6xxx: Infrastructure, IO, cache corruption, or internal generator failure.
        /// </summary>
        SystemFailure,

        /// <summary>
        /// DX7xxx: Cache-related violations (undeclared external input, non-deterministic cacheable stage).
        /// </summary>
        CacheViolation,

        /// <summary>
        /// DX8xxx: Trust boundary and supply chain violations (signature failure, derived artifact misuse).
        /// </summary>
        TrustViolation,
        DependencyViolation,
        InternalError
    }
}
