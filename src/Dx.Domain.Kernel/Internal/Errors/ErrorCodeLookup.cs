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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
// DPI: Provides internal lookup of passive ErrorCode definitions; ensures codes are valid and unique.
internal static class ErrorCodeLookup
{
    // Implementation note:
    // In a real codebase, this would be generated or backed by a static registry
    // in Dx.Domain.Abstractions (e.g., KernelErrorCodes). Here we keep it simple
    // and mechanical, assuming a static array of known codes.
    private static readonly ImmutableDictionary<string, ErrorCode> _byCode;

    static ErrorCodeLookup()
    {
        // This assumes a static class Dx.Domain.Abstractions.Errors.KernelErrorCodes
        // exposing readonly ErrorCode fields. That class is not shown here.
        var builder = ImmutableDictionary.CreateBuilder<string, ErrorCode>(StringComparer.Ordinal);

        // Example only; in the real repository this is populated from KernelErrorCodes via reflection or source generation.
        // builder["DX.DOMAIN.TIME.NON_MONOTONIC"] = KernelErrorCodes.TimeNonMonotonic;
        // builder["DX.DOMAIN.INVARIANT.NOT_EMPTY"] = KernelErrorCodes.InvariantNotEmpty;
        // ...

        _byCode = builder.ToImmutable();
    }

    public static bool TryGet(string code, out ErrorCode errorCode)
    {
        if (code is null)
        {
            errorCode = default;
            return false;
        }

        return _byCode.TryGetValue(code, out errorCode);
    }

    public static IEnumerable<ErrorCode> All() => _byCode.Values;
}
