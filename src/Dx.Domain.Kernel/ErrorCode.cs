// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ErrorCode.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain
{
    /// <summary>
    /// Represents an immutable error code and its associated metadata, including a short description and a rationale
    /// for display or diagnostic purposes.
    /// </summary>
    /// <remarks>Use this type to encapsulate error information in a structured and consistent manner
    /// throughout an application. The record is immutable and suitable for use as a value object.</remarks>
    /// <param name="Code">The unique string identifier for the error code. This value is typically used for programmatic checks or
    /// logging.</param>
    /// <param name="ShortDescription">A brief, human-readable description of the error. Intended to summarize the nature of the error for display or
    /// reporting.</param>
    /// <param name="DpiRationale">A rationale or explanation for the error, intended to provide additional context for diagnostics,
    /// troubleshooting, or user guidance.</param>
    public readonly record struct ErrorCode(string Code, string ShortDescription, string DpiRationale)
    {
        public const string InvalidArgument = "DX.INVALID.ARGUMENT";
        public const string InvalidActorId = "DX.INVALID.ACTOR_ID";
    }

    /// <summary>
    /// Provides a centralized set of canonical error codes used to represent common kernel-level domain errors.
    /// </summary>
    /// <remarks>Use these error codes to ensure consistency when reporting or handling standard domain
    /// invariants and mapping errors within the kernel. The codes are intended for reuse across the application to
    /// promote uniform error handling and diagnostics.</remarks>
    public static class KernelErrorCodes
    {
        public static readonly ErrorCode InvariantNotEmpty =
            new("DX.DOMAIN.INVARIANT.NOT_EMPTY", "Invariant: value must not be empty", "Enforces non-empty invariant for identity values");

        public static readonly ErrorCode TimeNonMonotonic =
            new("DX.DOMAIN.TIME.NON_MONOTONIC", "Time must be strictly increasing", "Enforces monotonic time invariant for DomainTime");

        public static readonly ErrorCode ResultMappingError =
            new("DX.DOMAIN.RESULT.MAPPING_ERROR", "Result mapping failed", "Canonicalizes mapping failures into DomainError");
    }
}
