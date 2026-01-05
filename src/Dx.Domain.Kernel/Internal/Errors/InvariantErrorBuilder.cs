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

using Dx.Domain;
using Dx.Domain.Errors;
using Dx.Domain.Internal.Errors;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

using static Dx.Domain.DxDomain.Kernel;

namespace Dx.Domain.Internal.Errors
{
    // DPI: Creates InvariantError values with rule identifiers and structured hints.
    //      No policy or behavior; these are pure truth-bearing values.
    internal static class InvariantErrorBuilder
    {
        public static InvariantError Create(
            string ruleId,
            ErrorCode code,
            IReadOnlyDictionary<string, object> hints)
        {
            if (string.IsNullOrWhiteSpace(ruleId))
                throw new ArgumentException("RuleId must not be null or whitespace.", nameof(ruleId));

            var safeHints = ToImmutableHints(hints);
            var error = DomainErrorFactory.From(code);

            return new InvariantError
            {
                RuleId = ruleId,
                Error = error,
                Hints = safeHints,
                Timestamp = null
            };
        }

        public static InvariantError Create(
            string ruleId,
            ErrorCode code,
            IReadOnlyDictionary<string, object> hints,
            DateTimeOffset timestamp)
        {
            if (string.IsNullOrWhiteSpace(ruleId))
                throw new ArgumentException("RuleId must not be null or whitespace.", nameof(ruleId));

            var safeHints = ToImmutableHints(hints);
            var error = DomainErrorFactory.From(code, timestamp);

            return new InvariantError
            {
                RuleId = ruleId,
                Error = error,
                Hints = safeHints,
                Timestamp = timestamp
            };
        }

        private static ImmutableDictionary<string, object> ToImmutableHints(
            IReadOnlyDictionary<string, object> hints)
        {
            if (hints is null || hints.Count == 0)
                return ImmutableDictionary<string, object>.Empty;

            var builder = ImmutableDictionary.CreateBuilder<string, object>();

            foreach (var kvp in hints)
            {
                if (kvp.Key is null)
                    throw new ArgumentException("Hint keys must not be null.", nameof(hints));

                var value = kvp.Value;
                if (value is not null && !Dx.Domain.Internal.Diagnostics.DiagnosticHintValidator.IsValidValue(value))
                {
                    throw new ArgumentException(
                        $"Hint value for key '{kvp.Key}' is not a permitted type.",
                        nameof(hints));
                }

                builder[kvp.Key] = value!;
            }

            return builder.ToImmutable();
        }
    }

