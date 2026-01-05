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
namespace Dx.Domain.Internal.Results
{
    // DPI: Canonicalizes Result<T> construction and mapping; no hidden control flow.
    internal static class ResultNormalizer
    {
        public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);

        public static Result<T> Fail<T>(DomainError error)
        {
            if (string.IsNullOrEmpty(error.Code))
                throw new ArgumentException("DomainError.Code must not be null or empty.", nameof(error));

            return Result<T>.Failure(error);
        }

        public static Result<U> Map<T, U>(Result<T> result, Func<T, U> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);

            if (result.IsSuccess)
            {
                var mapped = mapper(result.Value);
                return Result<U>.Success(mapped);
            }

            return Result<U>.Failure(result.Error);
        }
    }
}
