namespace Dx.Domain
{
    using System.Text.RegularExpressions;

    public static partial class Require
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Unit> That(bool condition, DomainError error)
            => condition ? Result<Unit>.Ok(Unit.Value) : Result<Unit>.Failure(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Unit> That(bool condition, Func<DomainError> errorFactory)
            => condition ? Result<Unit>.Ok(Unit.Value) : Result<Unit>.Failure(errorFactory());

        // Null checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> NotNull<T>(T? value, DomainError error) where T : class
            => value is null ? Result<T>.Failure(error) : Result<T>.Ok(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> NotNull<T>(T? value, Func<DomainError> errorFactory) where T : class
            => value is null ? Result<T>.Failure(errorFactory()) : Result<T>.Ok(value);

        // String checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> NotNullOrWhiteSpace(string? value, DomainError error)
            => string.IsNullOrWhiteSpace(value) ? Result<string>.Failure(error) : Result<string>.Ok(value!);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> NotNullOrEmpty(string? value, DomainError error)
            => string.IsNullOrEmpty(value) ? Result<string>.Failure(error) : Result<string>.Ok(value!);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> Matches(string? value, string pattern, DomainError error, RegexOptions options = RegexOptions.None)
            => value is not null && Regex.IsMatch(value, pattern, options) ? Result<string>.Ok(value) : Result<string>.Failure(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> LengthInRange(string? value, int minInclusive, int maxInclusive, DomainError error)
            => value is not null && value.Length >= minInclusive && value.Length <= maxInclusive ? Result<string>.Ok(value) : Result<string>.Failure(error);

        // Numeric range checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> InRange<T>(T value, T minInclusive, T maxInclusive, DomainError error) where T : IComparable<T>
            => value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) <= 0 ? Result<T>.Ok(value) : Result<T>.Failure(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> GreaterThan<T>(T value, T threshold, DomainError error) where T : IComparable<T>
            => value.CompareTo(threshold) > 0 ? Result<T>.Ok(value) : Result<T>.Failure(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> LessThan<T>(T value, T threshold, DomainError error) where T : IComparable<T>
            => value.CompareTo(threshold) < 0 ? Result<T>.Ok(value) : Result<T>.Failure(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TEnum> IsDefinedEnum<TEnum>(TEnum value, DomainError error) where TEnum : struct, Enum
            => Enum.IsDefined(value) ? Result<TEnum>.Ok(value) : Result<TEnum>.Failure(error);

        // Guid
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Guid> NotEmpty(Guid value, DomainError error)
            => value != Guid.Empty ? Result<Guid>.Ok(value) : Result<Guid>.Failure(error);

        // DateTime
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<DateTimeOffset> IsUtc(DateTimeOffset value, DomainError error)
            => value.Offset == TimeSpan.Zero ? Result<DateTimeOffset>.Ok(value) : Result<DateTimeOffset>.Failure(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<DateTimeOffset> NotInFuture(DateTimeOffset value, DomainError error)
            => value <= DateTimeOffset.UtcNow ? Result<DateTimeOffset>.Ok(value) : Result<DateTimeOffset>.Failure(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<DateTimeOffset> NotInPast(DateTimeOffset value, DomainError error)
            => value >= DateTimeOffset.UtcNow ? Result<DateTimeOffset>.Ok(value) : Result<DateTimeOffset>.Failure(error);

        // Collections
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<IReadOnlyCollection<T>> NotEmpty<T>(IEnumerable<T>? collection, DomainError error)
            => collection is not null && System.Linq.Enumerable.Any(collection) ? Result<IReadOnlyCollection<T>>.Ok(System.Linq.Enumerable.ToArray(collection)) : Result<IReadOnlyCollection<T>>.Failure(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<IReadOnlyCollection<T>> NoNullElements<T>(IEnumerable<T>? collection, DomainError error)
            => collection is not null && !System.Linq.Enumerable.Any(collection, e => e is null) ? Result<IReadOnlyCollection<T>>.Ok(System.Linq.Enumerable.ToArray(collection)) : Result<IReadOnlyCollection<T>>.Failure(error);

        // Predicate
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TValue> Satisfies<TValue>(TValue value, Func<TValue, bool> predicate, DomainError error)
            where TValue : notnull
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return predicate(value) ? Result<TValue>.Ok(value) : Result<TValue>.Failure(error);
        }

        // Parse helpers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TValue> TryParse<TValue>(string? input, Func<string, (bool ok, TValue value)> tryParse, DomainError error)
            where TValue : notnull
        {
            ArgumentNullException.ThrowIfNull(tryParse);
            if (input is null)
            {
                return Result<TValue>.Failure(error);
            }

            var (ok, value) = tryParse(input);
            return ok ? Result<TValue>.Ok(value) : Result<TValue>.Failure(error);
        }
    }
}
