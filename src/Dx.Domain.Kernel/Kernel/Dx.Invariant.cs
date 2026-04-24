// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dx.Invariant.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

// ============================================================================
// Dx.Domain.Internal — Kernel Internal Helpers
// DPI: These helpers are mechanical Kernel implementation details only.
// They:
//   - enforce invariants
//   - construct immutable values
//   - normalize results
//   - check time/diagnostics constraints
// They do NOT:
//   - expose public convenience APIs
//   - provide flow-oriented validation
//   - encode policy, logging, or integration behavior
// ============================================================================

using System;

namespace Dx.Domain
{
    /// <summary>
    /// Provides assertion‑style invariant enforcement.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Invariants represent <em>structural truths</em> of the domain model.
    /// Violating an invariant indicates either a programmer error or a corrupt
    /// internal state.
    /// </para>
    /// <para>
    /// <strong>Invariant methods throw exceptions on failure.</strong>
    /// They are intended for kernel, infrastructure, or otherwise trusted code
    /// paths where recovery is neither meaningful nor expected.
    /// </para>
    /// <para>
    /// For recoverable, flow‑oriented validation that should not throw,
    /// use <see cref="Dx.Require"/> instead.
    /// </para>
    /// </remarks>
    /// <seealso cref="Dx.Require"/>
    // DPI: Evaluates invariants and throws InvariantViolationException on failure.
    internal static class Invariant
    {
        /// <summary>
        /// Asserts that the specified condition holds.
        /// </summary>
        /// <param name="condition">
        /// The condition to evaluate. If <see langword="false"/>, the invariant is violated.
        /// </param>
        /// <param name="code">
        /// A stable, machine‑readable error code identifying the invariant.
        /// </param>
        /// <param name="message">
        /// A human‑readable description of the invariant violation.
        /// </param>
        /// <exception cref="InvariantViolationException">
        /// Thrown when <paramref name="condition"/> evaluates to <see langword="false"/>.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This overload is appropriate when the invariant cost is trivial and
        /// no contextual data must be computed lazily.
        /// </para>
        /// <para>
        /// If error construction is expensive or context‑dependent, use the
        /// factory‑based overload instead.
        /// </para>
        /// </remarks>
        public static void That(
            bool condition,
            string code,
            string message)
        {
            if (!condition)
                throw InvariantViolationException.Create(code, message);
        }

        /// <summary>
        /// Asserts that the specified condition holds, using lazy error construction.
        /// </summary>
        /// <param name="condition">
        /// The condition to evaluate. If <see langword="false"/>, the invariant is violated.
        /// </param>
        /// <param name="errorFactory">
        /// A factory that produces the invariant error code and message.
        /// The factory is invoked only if the condition fails.
        /// </param>
        /// <exception cref="InvariantViolationException">
        /// Thrown when <paramref name="condition"/> evaluates to <see langword="false"/>.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This overload avoids allocating error data on successful code paths
        /// and enables richer context capture at the point of failure.
        /// </para>
        /// </remarks>
        public static void That(
            bool condition,
            Func<(string Code, string Message)> errorFactory)
        {
            if (!condition)
            {
                var error = errorFactory();
                throw InvariantViolationException.Create(error.Code, error.Message);
            }
        }
    }
}
