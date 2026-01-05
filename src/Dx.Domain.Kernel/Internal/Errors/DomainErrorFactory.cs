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
    // DPI: Construct immutable DomainError values from passive ErrorCode definitions.
    //      Enforces meta immutability and timestamp rules. No policy, no I/O.
    internal static class DomainErrorFactory
    {
        public static DomainError From(ErrorCode code)
        {
            if (string.IsNullOrWhiteSpace(code.Code))
                throw new ArgumentException(message: "ErrorCode.Code must not be null or whitespace.");

            return DomainError.Create(code.Code, code.ShortDescription);
        }

        public static DomainError From(ErrorCode code, IReadOnlyDictionary<string, object> meta)
        {
            if (string.IsNullOrWhiteSpace(code.Code))
                throw new ArgumentException(message: "ErrorCode.Code must not be null or whitespace.");

            var safeMeta = ToImmutableMeta(meta);
            return DomainError.Create(code.Code, code.ShortDescription, safeMeta.ToImmutableArray());
        }

        public static DomainError From(ErrorCode code, DateTimeOffset timestamp)
        {
            if (string.IsNullOrWhiteSpace(code.Code))
                throw new ArgumentException(message: "ErrorCode.Code must not be null or whitespace.");

            return DomainError.Create(code.Code, code.ShortDescription).WithTimestamp(timestamp);
        }

        public static DomainError From(
            ErrorCode code,
            IReadOnlyDictionary<string, object> meta,
            DateTimeOffset timestamp)
        {
            if (string.IsNullOrWhiteSpace(code.Code))
                throw new ArgumentException(message: "ErrorCode.Code must not be null or whitespace.");

            var safeMeta = ToImmutableMeta(meta);
            return DomainError.Create(code.Code, code.ShortDescription).WithMeta(safeMeta).WithTimestamp(timestamp);
        }

        private static ImmutableDictionary<string, object> ToImmutableMeta(
            IReadOnlyDictionary<string, object> meta)
        {
            if (meta is null || meta.Count == 0)
                return ImmutableDictionary<string, object>.Empty;

            var builder = ImmutableDictionary.CreateBuilder<string, object>();

            foreach (var kvp in meta)
            {
                if (kvp.Key is null)
                    throw new ArgumentException("Meta keys must not be null.", nameof(meta));

                // Allow null values: absence of value can be meaningful.
                var value = kvp.Value;
                if (value is not null && !Dx.Domain.Internal.Diagnostics.DiagnosticHintValidator.IsValidValue(value))
                {
                    throw new ArgumentException(
                        $"Meta value for key '{kvp.Key}' is not a permitted type.",
                        nameof(meta));
                }

                builder[kvp.Key] = value!;
            }

            return builder.ToImmutable();
        }
    }
}
