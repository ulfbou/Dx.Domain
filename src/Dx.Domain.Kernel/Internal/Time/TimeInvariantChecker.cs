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

using System.Collections.Generic;

namespace Dx.Domain.Internal.Time
{
    using Dx.Domain.Errors;
    using Dx.Domain.Internal.Invariants;

    using System;

    // DPI: Enforces monotonic time invariants; no drift correction, no policy.
    internal static class TimeInvariantChecker
    {
        // NOTE: This helper assumes there is an ErrorCode defined for non-monotonic time,
        // e.g., KernelErrorCodes.TimeNonMonotonic in Dx.Domain.Abstractions.Errors.
        // That static registry is not shown here.
        public static InvariantError? EnsureMonotonic(
            DateTimeOffset previous,
            DateTimeOffset next,
            ErrorCode nonMonotonicErrorCode,
            string ruleId)
        {
            // Monotonic means strictly increasing.
            var isMonotonic = next > previous;

            if (isMonotonic)
                return null;

            var hints = new Dictionary<string, object>
            {
                ["previous"] = previous,
                ["next"] = next
            };

            return InvariantBuilder.Require(
                condition: false,
                ruleId: ruleId,
                code: nonMonotonicErrorCode,
                hints: hints);
        }
    }
}
