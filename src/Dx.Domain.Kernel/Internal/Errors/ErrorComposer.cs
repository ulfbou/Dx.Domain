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

namespace Dx.Domain.Internal.Errors
{
    // DPI: Pure composition of immutable error values; no semantic expansion or side effects.
    internal static class ErrorComposer
    {
        public static DomainError Wrap(DomainError outer, DomainError inner)
        {
            // Outer error remains the primary semantic identity.
            // Inner error is preserved as meta for post-mortem inspection.
            var metaBuilder = ImmutableDictionary.CreateBuilder<string, object>();

            if (outer.Meta is not null)
            {
                foreach (var kvp in outer.Meta)
                    metaBuilder[kvp.Key] = kvp.Value!;
            }

            // Store inner error under a stable key.
            metaBuilder["inner"] = inner;

            return new DomainError
            {
                Code = outer.Code,
                Message = outer.Message,
                Meta = metaBuilder.ToImmutable(),
                Timestamp = outer.Timestamp ?? inner.Timestamp
            };
        }

        public static DomainError Enrich(DomainError error, string key, object value)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (value is not null &&
                !Dx.Domain.Internal.Diagnostics.DiagnosticHintValidator.IsValidValue(value))
            {
                throw new ArgumentException(
                    $"Meta value for key '{key}' is not a permitted type.",
                    nameof(value));
            }

            var metaBuilder = ImmutableDictionary.CreateBuilder<string, object>();

            if (error.Meta is not null)
            {
                foreach (var kvp in error.Meta)
                    metaBuilder[kvp.Key] = kvp.Value!;
            }

            metaBuilder[key] = value!;
            return new DomainError
            {
                Code = error.Code,
                Message = error.Message,
                Meta = metaBuilder.ToImmutable(),
                Timestamp = error.Timestamp
            };
        }

        public static IReadOnlyDictionary<string, object> MergeMeta(
            IReadOnlyDictionary<string, object> a,
            IReadOnlyDictionary<string, object> b)
        {
            if ((a is null || a.Count == 0) && (b is null || b.Count == 0))
                return ImmutableDictionary<string, object>.Empty;

            var builder = ImmutableDictionary.CreateBuilder<string, object>();

            if (a is not null)
            {
                foreach (var kvp in a)
                    builder[kvp.Key] = kvp.Value!;
            }

            if (b is not null)
            {
                foreach (var kvp in b)
                    builder[kvp.Key] = kvp.Value!;
            }

            return builder.ToImmutable();
        }
    }

