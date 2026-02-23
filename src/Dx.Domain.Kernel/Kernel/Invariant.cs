// ============================================================================
// Dx.Domain.Internal — Kernel Internal Helpers
// DPI: These helpers are mechanical Kernel implementation details only.
// They:
//   - enforce invariants
//   - construct immutable values
//   - normalize results
//   - check time/diagnostics constraints
// They do NOT:
//   - expose public APIs
//   - provide convenience to consumers
//   - encode policy, logging, or integration behavior
// ============================================================================

using System;
using System.Collections.Generic;

namespace Dx.Domain.Kernel
{
    /// <summary>
    /// Provides static methods for enforcing invariants and throwing an InvariantViolationException when a specified
    /// condition is not met.
    /// </summary>
    // DPI: Evaluates invariants and throws InvariantViolationException on failure.
    internal static class Invariant
    {
        /// <summary>
        /// Checks that the specified condition is met and throws an exception if the invariant is violated.
        /// </summary>
        /// <remarks>Use this method to enforce invariants within your code. If the condition is not met,
        /// an exception is thrown containing the provided rule identifier and error code.</remarks>
        /// <param name="condition">The condition to evaluate. If <see langword="false"/>, an invariant violation is detected.</param>
        /// <param name="code">The error code to associate with the invariant violation.</param>
        /// <param name="message">The error message to include in the exception if the invariant is violated.</param>
        /// <exception cref="InvariantViolationException">Thrown if the specified condition is <see langword="false"/>, indicating that the invariant has been
        /// violated.</exception>
        public static void That(
                bool condition,
                string code,
                string message)
        {
            if (!condition)
                throw InvariantViolationException.Create(code, message);
        }
    }
}
