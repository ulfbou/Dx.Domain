// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DiagnosticCodes.cs" company="Dx.Domain Team">
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
    /// Defines all diagnostic codes as specified in the formal specification.
    /// </summary>
    public static class DiagnosticCodes
    {
        // Generator Invariants (DX1xxx)
        /// <summary>
        /// DX1001: Referential Transparency violation.
        /// </summary>
        public const string ReferentialTransparency = "DX1001";

        /// <summary>
        /// DX1002: Monotonic Knowledge violation.
        /// </summary>
        public const string MonotonicKnowledge = "DX1002";

        /// <summary>
        /// DX1003: No Semantic Guessing violation.
        /// </summary>
        public const string NoSemanticGuessing = "DX1003";

        /// <summary>
        /// DX1004: No Hidden Coupling violation.
        /// </summary>
        public const string NoHiddenCoupling = "DX1004";

        // Intent Violations (DX2xxx)
        /// <summary>
        /// DX2001: Generic intent violation.
        /// </summary>
        public const string IntentViolation = "DX2001";

        // Policy Violations (DX3xxx)
        /// <summary>
        /// DX3001: Generic policy violation.
        /// </summary>
        public const string PolicyViolation = "DX3001";

        // Inference Failures (DX4xxx)
        /// <summary>
        /// DX4001: Generic inference failure.
        /// </summary>
        public const string InferenceFailure = "DX4001";

        // Compatibility Failures (DX5xxx)
        /// <summary>
        /// DX5001: Generic compatibility failure.
        /// </summary>
        public const string CompatibilityFailure = "DX5001";

        // System Failures (DX6xxx)
        /// <summary>
        /// DX6001: Generic system failure.
        /// </summary>
        public const string SystemFailure = "DX6001";

        // Cache Violations (DX7xxx)
        /// <summary>
        /// DX7001: Undeclared external input in cacheable stage.
        /// </summary>
        public const string UndeclaredExternalInput = "DX7001";

        /// <summary>
        /// DX7002: Non-deterministic cacheable stage.
        /// </summary>
        public const string NonDeterministicCacheableStage = "DX7002";

        // Trust Violations (DX8xxx)
        /// <summary>
        /// DX8001: Signature failure.
        /// </summary>
        public const string SignatureFailure = "DX8001";

        /// <summary>
        /// DX8002: Derived artifact misuse.
        /// </summary>
        public const string DerivedArtifactMisuse = "DX8002";
    }
}
