namespace Dx.Domain 
{
    public static partial class Require
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Unit> That(bool condition, DomainError error)
            => condition ? Result<Unit>.Ok(Unit.Value) : Result<Unit>.Fail(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Unit> That(bool condition, Func<DomainError> errorFactory)
            => condition ? Result<Unit>.Ok(Unit.Value) : Result<Unit>.Fail(errorFactory());

        // Null checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> NotNull<T>(T? value, DomainError error) where T : class
            => value is null ? Result<T>.Fail(error) : Result<T>.Ok(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> NotNull<T>(T? value, Func<DomainError> errorFactory) where T : class
            => value is null ? Result<T>.Fail(errorFactory()) : Result<T>.Ok(value);

        // String checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> NotNullOrWhiteSpace(string? value, DomainError error)
            => string.IsNullOrWhiteSpace(value) ? Result<string>.Fail(error) : Result<string>.Ok(value!);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> NotNullOrEmpty(string? value, DomainError error)
            => string.IsNullOrEmpty(value) ? Result<string>.Fail(error) : Result<string>.Ok(value!);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> Matches(string? value, string pattern, DomainError error, RegexOptions options = RegexOptions.None)
            => value is not null && Regex.IsMatch(value, pattern, options) ? Result<string>.Ok(value) : Result<string>.Fail(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> LengthInRange(string? value, int minInclusive, int maxInclusive, DomainError error)
            => value is not null && value.Length >= minInclusive && value.Length <= maxInclusive ? Result<string>.Ok(value) : Result<string>.Fail(error);

        // Numeric range checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> InRange<T>(T value, T minInclusive, T maxInclusive, DomainError error) where T : IComparable<T>
            => value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) <= 0 ? Result<T>.Ok(value) : Result<T>.Fail(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> GreaterThan<T>(T value, T threshold, DomainError error) where T : IComparable<T>
            => value.CompareTo(threshold) > 0 ? Result<T>.Ok(value) : Result<T>.Fail(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> LessThan<T>(T value, T threshold, DomainError error) where T : IComparable<T>
            => value.CompareTo(threshold) < 0 ? Result<T>.Ok(value) : Result<T>.Fail(error);

        // Enum
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TEnum> IsDefinedEnum<TEnum>(TEnum value, DomainError error) where TEnum : struct, Enum
            => Enum.IsDefined(typeof(TEnum), value) ? Result<TEnum>.Ok(value) : Result<TEnum>.Fail(error);

        // Guid
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Guid> NotEmpty(Guid value, DomainError error)
            => value != Guid.Empty ? Result<Guid>.Ok(value) : Result<Guid>.Fail(error);

        // DateTime
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<DateTimeOffset> IsUtc(DateTimeOffset value, DomainError error)
            => value.Offset == TimeSpan.Zero ? Result<DateTimeOffset>.Ok(value) : Result<DateTimeOffset>.Fail(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<DateTimeOffset> NotInFuture(DateTimeOffset value, DomainError error)
            => value <= DateTimeOffset.UtcNow ? Result<DateTimeOffset>.Ok(value) : Result<DateTimeOffset>.Fail(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<DateTimeOffset> NotInPast(DateTimeOffset value, DomainError error)
            => value >= DateTimeOffset.UtcNow ? Result<DateTimeOffset>.Ok(value) : Result<DateTimeOffset>.Fail(error);

        // Collections
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<IReadOnlyCollection<T>> NotEmpty<T>(IEnumerable<T>? collection, DomainError error)
            => collection is not null && System.Linq.Enumerable.Any(collection) ? Result<IReadOnlyCollection<T>>.Ok(System.Linq.Enumerable.ToArray(collection)) : Result<IReadOnlyCollection<T>>.Fail(error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<IReadOnlyCollection<T>> NoNullElements<T>(IEnumerable<T>? collection, DomainError error)
            => collection is not null && !System.Linq.Enumerable.Any(collection, e => e is null) ? Result<IReadOnlyCollection<T>>.Ok(System.Linq.Enumerable.ToArray(collection)) : Result<IReadOnlyCollection<T>>.Fail(error);

        // Predicate
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Satisfies<T>(T value, Func<T, bool> predicate, DomainError error)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return predicate(value) ? Result<T>.Ok(value) : Result<T>.Fail(error);
        }

        // Parse helpers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> TryParse<T>(string? input, Func<string, (bool ok, T value)> tryParse, DomainError error)
        {
            if (tryParse is null) throw new ArgumentNullException(nameof(tryParse));
            if (input is null) return Result<T>.Fail(error);
            var (ok, value) = tryParse(input);
            return ok ? Result<T>.Ok(value) : Result<T>.Fail(error);
        }
    }
}
