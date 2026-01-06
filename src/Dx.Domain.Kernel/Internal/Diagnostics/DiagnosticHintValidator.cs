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

namespace Dx.Domain.Internal.Diagnostics
{
    // DPI: Ensures diagnostic hint values are serializable and permitted.
    //      This is a mechanical guard, analyzers enforce the broader contract.
    internal static class DiagnosticHintValidator
    {
        public static bool IsValidValue(object value)
        {
            if (value is null)
                return true; // Null is allowed; absence is meaningful.

            var type = value.GetType();

            if (type.IsPrimitive)
                return true;

            if (type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(Guid))
                return true;

            if (type.IsEnum)
                return true;

            // Reject delegates, streams, and common runtime handles.
            if (typeof(Delegate).IsAssignableFrom(type))
                return false;

            if (type.FullName is not null &&
                (type.FullName.Contains("System.IO.", StringComparison.Ordinal) ||
                 type.FullName.Contains("System.Threading.", StringComparison.Ordinal)))
            {
                return false;
            }

            // For now, be conservative: allow other types, but analyzers should enforce DTO constraints.
            return true;
        }

        public static void ThrowIfInvalid(object value, string key)
        {
            if (!IsValidValue(value))
                throw new ArgumentException(
                    $"Diagnostic hint value for key '{key}' is not a permitted type.",
                    nameof(value));
        }
    }
}
