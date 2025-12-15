using System.Text.RegularExpressions;

namespace Dx.Domain
{
    public static partial class Invariant
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void That(
            bool condition,
            InvariantError error,
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            if (!condition)
            {
                throw InvariantViolationException.From(error);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void That(bool condition, Func<DomainError> errorFactory, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (!condition)
            {
                var domainError = errorFactory();
                var invariantError = InvariantError.Create(
                    domainError,
                    messageOverride: null,
                    correlationId: default,
                    traceId: default,
                    spanId: default,
                    member: member,
                    file: file,
                    line: line);

                throw InvariantViolationException.From(invariantError);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(T? value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T : class
            => That(value is not null, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(T? value, Func<DomainError> errorFactory, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T : class
            => That(value is not null, errorFactory, member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrWhiteSpace(string? value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(!string.IsNullOrWhiteSpace(value), InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrEmpty(string? value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(!string.IsNullOrEmpty(value), InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Matches(string? value, string pattern, DomainError error, RegexOptions options = RegexOptions.None, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value is not null && Regex.IsMatch(value, pattern, options), InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LengthInRange(string? value, int minInclusive, int maxInclusive, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value is not null && value.Length >= minInclusive && value.Length <= maxInclusive, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InRange<T>(T value, T minInclusive, T maxInclusive, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where T : IComparable<T>
            => That(value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) <= 0, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GreaterThan<T>(T value, T threshold, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where T : IComparable<T>
            => That(value.CompareTo(threshold) > 0, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GreaterThanOrEqual<T>(T value, T threshold, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where T : IComparable<T>
            => That(value.CompareTo(threshold) >= 0, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessThan<T>(T value, T threshold, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where T : IComparable<T>
            => That(value.CompareTo(threshold) < 0, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessThanOrEqual<T>(T value, T threshold, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where T : IComparable<T>
            => That(value.CompareTo(threshold) <= 0, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsDefinedEnum<TEnum>(TEnum value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            where TEnum : struct, Enum
            => That(Enum.IsDefined(value), InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEmpty(Guid value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value != Guid.Empty, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsUtc(DateTimeOffset value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value.Offset == TimeSpan.Zero, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotInFuture(DateTimeOffset value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value <= DateTimeOffset.UtcNow, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotInPast(DateTimeOffset value, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(value >= DateTimeOffset.UtcNow, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEmpty<T>(IEnumerable<T>? collection, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(collection is not null && collection.GetEnumerator().MoveNext(), InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NoNullElements<T>(IEnumerable<T>? collection, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(collection is not null && !System.Linq.Enumerable.Any(collection, e => e is null), InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MaxCount<T>(IEnumerable<T>? collection, int maxInclusive, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
            => That(collection is not null && System.Linq.Enumerable.Count(collection) <= maxInclusive, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Satisfies<T>(T value, Func<T, bool> predicate, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            That(predicate(value), InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);
        }

        // Parse helpers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CanParse<T>(string? input, Func<string, (bool ok, T value)> tryParse, DomainError error, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            ArgumentNullException.ThrowIfNull(tryParse);
            That(input is not null && tryParse(input).ok, InvariantError.Create(error, null, default, default, default, member, file, line), member, file, line);
        }
    }
}
