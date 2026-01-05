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

using Dx.Domain.Errors;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Dx.Domain.Internal.Invariants
{
    // DPI: Centralizes invariant evaluation; builds InvariantError for failed conditions.
    internal static class InvariantBuilder
    {
        public static InvariantError? Require(
            bool condition,
            string ruleId,
            ErrorCode code)
        {
            if (condition)
                return null;

            return InvariantErrorBuilder.Create(
                ruleId,
                code,
                hints: ImmutableDictionary<string, object>.Empty);
        }

        public static InvariantError? Require(
            bool condition,
            string ruleId,
            ErrorCode code,
            IReadOnlyDictionary<string, object> hints)
        {
            if (condition)
                return null;

            return InvariantErrorBuilder.Create(
                ruleId,
                code,
                hints);
        }
    }
    // DPI: Composes invariant checks without adding semantics; pure logical composition.
    internal static class GuardComposer
    {
        public static InvariantError? And(params Func<InvariantError?>[] guards)
        {
            ArgumentNullException.ThrowIfNull(guards);

            foreach (var guard in guards)
            {
                if (guard is null)
                    continue;

                var result = guard();
                if (result is not null)
                    return result;
            }

            return null;
        }

        public static InvariantError? Or(params Func<InvariantError?>[] guards)
        {
            ArgumentNullException.ThrowIfNull(guards);

            InvariantError? lastError = null;

            foreach (var guard in guards)
            {
                if (guard is null)
                    continue;

                var result = guard();
                if (result is null)
                    return null; // At least one guard passed.

                lastError = result;
            }

            // All failed; report the last error (or null if nothing ran).
            return lastError;
        }
    }
}
