namespace Dx.Domain 
{
    public static partial class Invariant
    {
        // Basic truth
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void That(bool condition, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (!condition) throw new InvariantViolationException(error, member, file, line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void That(bool condition, Func<DomainError> errorFactory, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (!condition) throw new InvariantViolationException(errorFactory(), member, file, line);
        }

        // Null checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(T? value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T : class
            => That(value is not null, error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(T? value, Func<DomainError> errorFactory, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T : class
            => That(value is not null, errorFactory, member, file, line);

        // String checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrWhiteSpace(string? value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(!string.IsNullOrWhiteSpace(value), error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrEmpty(string? value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(!string.IsNullOrEmpty(value), error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Matches(string? value, string pattern, DomainError error, RegexOptions options = RegexOptions.None, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value is not null && Regex.IsMatch(value, pattern, options), error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LengthInRange(string? value, int minInclusive, int maxInclusive, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value is not null && value.Length >= minInclusive && value.Length <= maxInclusive, error, member, file, line);

        // Numeric range checks (IComparable)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InRange<T>(T value, T minInclusive, T maxInclusive, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where T : IComparable<T>
            => That(value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) <= 0, error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GreaterThan<T>(T value, T threshold, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where T : IComparable<T>
            => That(value.CompareTo(threshold) > 0, error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GreaterThanOrEqual<T>(T value, T threshold, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where T : IComparable<T>
            => That(value.CompareTo(threshold) >= 0, error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessThan<T>(T value, T threshold, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where T : IComparable<T>
            => That(value.CompareTo(threshold) < 0, error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessThanOrEqual<T>(T value, T threshold, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where T : IComparable<T>
            => That(value.CompareTo(threshold) <= 0, error, member, file, line);

        // Enum checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsDefinedEnum<TEnum>(TEnum value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where TEnum : struct, Enum
            => That(Enum.IsDefined(typeof(TEnum), value), error, member, file, line);

        // Guid checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEmpty(Guid value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value != Guid.Empty, error, member, file, line);

        // DateTime checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsUtc(DateTimeOffset value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value.Offset == TimeSpan.Zero, error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotInFuture(DateTimeOffset value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value <= DateTimeOffset.UtcNow, error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotInPast(DateTimeOffset value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value >= DateTimeOffset.UtcNow, error, member, file, line);

        // Collections
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEmpty<T>(IEnumerable<T>? collection, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(collection is not null && collection.GetEnumerator().MoveNext(), error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NoNullElements<T>(IEnumerable<T>? collection, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(collection is not null && !System.Linq.Enumerable.Any(collection, e => e is null), error, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MaxCount<T>(IEnumerable<T>? collection, int maxInclusive, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(collection is not null && System.Linq.Enumerable.Count(collection) <= maxInclusive, error, member, file, line);

        // Predicate
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Satisfies<T>(T value, Func<T, bool> predicate, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            That(predicate(value), error, member, file, line);
        }

        // Parse helpers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CanParse<T>(string? input, Func<string, (bool ok, T value)> tryParse, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (tryParse is null) throw new ArgumentNullException(nameof(tryParse));
            That(input is not null && tryParse(input).ok, error, member, file, line);
        }
    }
}
