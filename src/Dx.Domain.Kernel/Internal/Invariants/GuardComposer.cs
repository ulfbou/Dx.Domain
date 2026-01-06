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

using Dx.Domain.Diagnostics;
using Dx.Domain.Errors;
using Dx.Domain.Internal.Errors;

using System;
using System.Collections.Generic;

namespace Dx.Domain.Internal.Invariants
{
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

                try
                {
                    var result = guard();
                    if (result is not null)
                        return result;
                }
                catch (Exception ex)
                {
                    return InvariantErrorBuilder.Create(
                        ruleId: DxRuleIds.GuardComposerAndException,
                        code: new ErrorCode("DX.INVARIANT.GUARD_COMPOSITION.EXCEPTION"),
                        hints: new Dictionary<string, object>
                        {
                            { "ExceptionType", ex.GetType().FullName ?? "Unknown" },
                            { "ExceptionMessage", ex.Message }
                        });
                }
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

                try
                {
                    var result = guard();
                    if (result is null)
                        return null;
                    lastError = result;
                }
                catch
                {
                    // Swallow exceptions in Or composition
                }
            }

            return lastError;
        }
    }
}
