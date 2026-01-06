using Dx.Domain.Errors;
using Dx.Domain.Internal.Invariants;

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Dx.Domain.Internal.Errors
{
    // DPI: Pure composition of immutable error values; no semantic expansion or side effects.
    internal static class ErrorComposer
    {
        public static DomainError Wrap(DomainError outer, DomainError inner)
        {
            // If neither error has meta, avoid allocations and return a simple cloned error
            if (outer.Meta is null && inner.Meta is null)
            {
                return DomainError.Create(outer.Code, outer.Message);
            }

            var mergedMeta = MergeMeta(outer.Meta, inner.Meta);
            return DomainError.Create(outer.Code, outer.Message, mergedMeta);
        }

        public static DomainError Enrich(DomainError error, string key, object value)
        {
            Invariant.That(!string.IsNullOrWhiteSpace(key),
                "Dx.Domain.Internal.Errors.ErrorComposer.Enrich",
                _ =>
                    (new ErrorCode(ErrorCode.InvalidArgument, "Meta key must be a non-empty string.", DpiRationale: "Meta keys are used to identify metadata entries."),
                    new Dictionary<string, object>
                    {
                        { "ParameterName", nameof(key) },
                        { "ParameterValue", key ?? "null" }
                    }));

            if (value is not null &&
                !Dx.Domain.Internal.Diagnostics.DiagnosticHintValidator.IsValidValue(value))
            {
                throw InvariantViolationException.Create(
                    "Dx.Domain.Internal.Errors.ErrorComposer.InvalidMetaValue",
                    "Meta value is not a permitted type.",
                    nameof(Enrich));
            }

            // Fast path: no existing meta, create a single-entry array
            if (error.Meta is null || error.Meta.Count == 0)
            {
                return DomainError.Create(
                    error.Code,
                    error.Message,
                    ImmutableArray.Create(new KeyValuePair<string, object>(key, value!)));
            }

            var metaBuilder = ImmutableDictionary.CreateBuilder<string, object>();

            foreach (var kvp in error.Meta)
            {
                metaBuilder[kvp.Key] = kvp.Value!;
            }

            metaBuilder[key] = value!;
            return DomainError.Create(error.Code, error.Message, metaBuilder.ToImmutableArray());
        }

        private static ImmutableArray<KeyValuePair<string, object>> MergeMeta(
            IReadOnlyDictionary<string, object>? a,
            IReadOnlyDictionary<string, object>? b)
        {
            var hasA = a is not null && a.Count > 0;
            var hasB = b is not null && b.Count > 0;

            if (!hasA && !hasB)
                return ImmutableArray<KeyValuePair<string, object>>.Empty;

            if (hasA && !hasB)
            {
                // Clone into an immutable array with a single pass
                var builder = ImmutableArray.CreateBuilder<KeyValuePair<string, object>>(a.Count);
                foreach (var kvp in a)
                {
                    builder.Add(new KeyValuePair<string, object>(kvp.Key, kvp.Value!));
                }

                return builder.MoveToImmutable();
            }

            if (!hasA && hasB)
            {
                var builder = ImmutableArray.CreateBuilder<KeyValuePair<string, object>>(b.Count);
                foreach (var kvp in b)
                {
                    builder.Add(new KeyValuePair<string, object>(kvp.Key, kvp.Value!));
                }

                return builder.MoveToImmutable();
            }

            // Both have values – merge with B overriding A on key collisions
            var merged = ImmutableDictionary.CreateBuilder<string, object>();

            foreach (var kvp in a!)
            {
                merged[kvp.Key] = kvp.Value!;
            }

            foreach (var kvp in b!)
            {
                merged[kvp.Key] = kvp.Value!;
            }

            return merged.ToImmutableArray();
        }
    }
}
