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

namespace Dx.Domain.Internal.Invariants
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
        /// <param name="ruleId">The identifier of the rule associated with the invariant. Cannot be null.</param>
        /// <param name="code">The error code to associate with the invariant violation.</param>
        /// <exception cref="InvariantViolationException">Thrown if the specified condition is <see langword="false"/>, indicating that the invariant has been
        /// violated.</exception>
        public static void That(
            bool condition,
            string ruleId,
            ErrorCode code)
        {
            var error = InvariantBuilder.Require(condition, ruleId, code);

            if (error is not null)
                throw new InvariantViolationException(error);
        }

        /// <summary>
        /// Checks that the specified condition is met and throws an exception if the invariant is violated.
        /// </summary>
        /// <remarks>Use this method to enforce invariants in your code and provide detailed error
        /// information when an invariant is not satisfied. The <paramref name="ruleId"/>, <paramref name="code"/>, and
        /// <paramref name="hints"/> parameters are included in the exception to aid in diagnostics.</remarks>
        /// <param name="condition">The condition to evaluate. If <see langword="false"/>, an <see cref="InvariantViolationException"/> is
        /// thrown.</param>
        /// <param name="ruleId">The identifier of the rule or invariant being checked. Cannot be null.</param>
        /// <param name="code">The error code to associate with the invariant violation.</param>
        /// <param name="hints">A read-only dictionary containing additional context or hints related to the invariant check. Can be null or
        /// empty if no hints are needed.</param>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="condition"/> is <see langword="false"/>, indicating that the invariant has been
        /// violated.</exception>
        public static void That(
            bool condition,
            string ruleId,
            ErrorCode code,
            IReadOnlyDictionary<string, object> hints)
        {
            var error = InvariantBuilder.Require(condition, ruleId, code, hints);
            if (error is not null)
                throw new InvariantViolationException(error);
        }

        /// <summary>
        /// Asserts that a specified condition is true and triggers an invariant violation if the condition is false.
        /// </summary>
        /// <remarks>Use this method to enforce invariants in code by specifying a rule identifier and
        /// error details to be used if the condition fails. No action is taken if the condition is true.</remarks>
        /// <param name="condition">The condition to evaluate. If <see langword="true"/>, no action is taken; if <see langword="false"/>, an
        /// invariant violation is triggered.</param>
        /// <param name="ruleId">The identifier for the rule associated with the invariant violation. Used to categorize or reference the
        /// specific rule being enforced.</param>
        /// <param name="value">A function that provides the error code and additional hints for the invariant violation. The function is
        /// invoked only if the condition is false.</param>
        public static void That(bool condition, string ruleId, Func<object, (ErrorCode code, IReadOnlyDictionary<string, object> hints)> value)
        {
            if (condition)
                return;

            try
            {
                (ErrorCode code, IReadOnlyDictionary<string, object> hints) = value.Invoke(null!);
                var error = InvariantBuilder.Require(false, ruleId, code, hints);
            }
            catch { }
        }
    }
}
