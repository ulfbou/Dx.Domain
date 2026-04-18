// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Dx.Domain.Errors;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Dx.Domain.Kernel.Tests")]

namespace Dx.Domain;

/// <summary>
/// Internal kernel facade providing mechanical support for canonical patterns.
/// </summary>
/// <remarks>
/// <para>
/// <b>Purpose:</b> Centralize mechanical, non-semantic kernel utilities:
/// <list type="bullet">
/// <item>Canonical error code constants</item>
/// <item>Error construction helpers</item>
/// <item>Invariant hook centralization</item>
/// <item>Internal factory funnels (if needed)</item>
/// </list>
/// </para>
/// <para>
/// <b>Constraints:</b>
/// <list type="number">
/// <item>Internal only (never public)</item>
/// <item>No ambient context (no static state, ThreadLocal, HttpContext)</item>
/// <item>No business semantics</item>
/// <item>No time/policy opinions</item>
/// <item>Mechanical support only</item>
/// </list>
/// </para>
/// <para>
/// This facade is consistent with "kernel forbids semantic expansion, not mechanical support."
/// It does NOT violate the kernel minimization principle because it provides infrastructure
/// for the structural spine, not domain-specific behavior.
/// </para>
/// </remarks>
[DpiJustified("Internal mechanical facade for kernel consistency; no semantic expansion.")]
internal static class DxK
{
    /// <summary>
    /// Canonical error code constants.
    /// </summary>
    /// <remarks>
    /// Error codes are:
    /// <list type="bullet">
    /// <item>Namespaced with "dx.kernel.*" prefix</item>
    /// <item>Never reused for unrelated semantics</item>
    /// <item>Append-only (never remove or change meaning)</item>
    /// </list>
    /// </remarks>
    internal static class Codes
    {
        /// <summary>Domain transition violated invariants.</summary>
        public const string Domain_InvalidTransition = "dx.kernel.domain.invalid_transition";

        /// <summary>Fact payload was null when required.</summary>
        public const string Domain_FactPayloadNull = "dx.kernel.domain.fact_payload_null";

        /// <summary>Identity value format is invalid.</summary>
        public const string Identity_InvalidFormat = "dx.kernel.identity.invalid_format";

        /// <summary>Identity value is empty when required.</summary>
        public const string Identity_Empty = "dx.kernel.identity.empty";

        /// <summary>Result operation attempted on failed result.</summary>
        public const string Result_OperationOnFailure = "dx.kernel.result.operation_on_failure";

        /// <summary>Invariant violation (structural constraint broken).</summary>
        public const string Invariant_Violation = "dx.kernel.invariant.violation";

        // NOTE: Expand minimally as structural needs arise.
        // Each constant must have clear, single-purpose meaning.
        // Never reuse identifiers for different semantics.
    }

    /// <summary>
    /// Canonical error constructor (mechanical only, no side effects).
    /// </summary>
    /// <param name="code">Error code from <see cref="Codes"/>.</param>
    /// <param name="message">Human-readable error message.</param>
    /// <returns>A <see cref="DomainError"/> instance.</returns>
    /// <remarks>
    /// This is a convenience helper to ensure consistent error construction.
    /// It has no ambient context, no logging, no side effects.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static DomainError Err(string code, string message)
    {
        return DomainError.Create(code, message);
    }

    // NOTE: Additional mechanical helpers can be added here if they:
    // 1. Have no business semantics
    // 2. Are pure (referentially transparent)
    // 3. Support structural kernel operations
    // 4. Do not access ambient context
    //
    // Examples of ALLOWED additions:
    // - Invariant funnel methods (if centralizing diagnostic context)
    // - Internal factory coordination (if needed for type construction)
    // - Structural validation helpers (format checks, bounds validation)
    //
    // Examples of FORBIDDEN additions:
    // - Time providers
    // - Logging
    // - Retry logic
    // - Policy decisions
    // - Database/IO access
    // - Service locators
}
