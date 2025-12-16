// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Require.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Invariants
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides functional-style precondition checks that return <see cref="Result{TValue}"/> instead of throwing
    /// exceptions.
    /// </summary>
    /// <remarks>
    /// These helpers are intended for use in domain logic where you want to model failures explicitly via
    /// <see cref="Result{TValue}"/> and <see cref="DomainError"/>, rather than by throwing exceptions.
    /// </remarks>
    internal static partial class Require
    {
        /// <summary>
        /// Ensures that the specified condition is <see langword="true"/>, returning a successful
        /// <see cref="Result{Unit}"/> when it is and a failed result with the provided <see cref="DomainError"/> otherwise.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="error">The error to return if the condition is <see langword="false"/>.</param>
        /// <returns>A successful or failed <see cref="Result{Unit}"/> depending on the condition.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Unit> That(bool condition, DomainError error)
            => condition ? Result<Unit>.Ok(Unit.Value) : Result<Unit>.Failure(error);

        /// <summary>
        /// Ensures that the specified condition is <see langword="true"/>, returning a successful
        /// <see cref="Result{Unit}"/> when it is and a failed result with an error produced by
        /// <paramref name="errorFactory"/> otherwise.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="errorFactory">A factory used to create the <see cref="DomainError"/> when the condition fails.</param>
        /// <returns>A successful or failed <see cref="Result{Unit}"/> depending on the condition.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Unit> That(bool condition, Func<DomainError> errorFactory)
            => condition ? Result<Unit>.Ok(Unit.Value) : Result<Unit>.Failure(errorFactory());

        /// <summary>
        /// Ensures that the specified reference value is not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The reference type of the value.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="error">The error to return when <paramref name="value"/> is <see langword="null"/>.</param>
        /// <returns>
        /// A successful <see cref="Result{TValue}"/> containing <paramref name="value"/> when not null;
        /// otherwise, a failed result with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> NotNull<T>(T? value, DomainError error) where T : class
            => value is null ? Result<T>.Failure(error) : Result<T>.Ok(value);

        /// <summary>
        /// Ensures that the specified reference value is not <see langword="null"/>, using an error factory.
        /// </summary>
        /// <typeparam name="T">The reference type of the value.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="errorFactory">A factory used to create the <see cref="DomainError"/> when the value is null.</param>
        /// <returns>
        /// A successful <see cref="Result{TValue}"/> containing <paramref name="value"/> when not null;
        /// otherwise, a failed result created from <paramref name="errorFactory"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> NotNull<T>(T? value, Func<DomainError> errorFactory) where T : class
            => value is null ? Result<T>.Failure(errorFactory()) : Result<T>.Ok(value);

        /// <summary>
        /// Ensures that a string is not <see langword="null"/> or whitespace.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <param name="error">The error to return when the string is null or whitespace.</param>
        /// <returns>
        /// A successful <see cref="Result{TValue}"/> containing the string when valid; otherwise, a failed result
        /// with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> NotNullOrWhiteSpace(string? value, DomainError error)
            => string.IsNullOrWhiteSpace(value) ? Result<string>.Failure(error) : Result<string>.Ok(value!);

        /// <summary>
        /// Ensures that a string is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <param name="error">The error to return when the string is null or empty.</param>
        /// <returns>
        /// A successful <see cref="Result{TValue}"/> containing the string when valid; otherwise, a failed result
        /// with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> NotNullOrEmpty(string? value, DomainError error)
            => string.IsNullOrEmpty(value) ? Result<string>.Failure(error) : Result<string>.Ok(value!);

        /// <summary>
        /// Ensures that a string matches the specified regular expression pattern.
        /// </summary>
        /// <param name="value">The string to evaluate.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="error">The error to return when the string does not match the pattern or is null.</param>
        /// <param name="options">Optional regular expression options.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when it matches; otherwise, a failed result
        /// with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> Matches(string? value, string pattern, DomainError error, RegexOptions options = RegexOptions.None)
            => value is not null && Regex.IsMatch(value, pattern, options) ? Result<string>.Ok(value) : Result<string>.Failure(error);

        /// <summary>
        /// Ensures that a string's length falls within the specified inclusive range.
        /// </summary>
        /// <param name="value">The string to evaluate.</param>
        /// <param name="minInclusive">The minimum allowed length, inclusive.</param>
        /// <param name="maxInclusive">The maximum allowed length, inclusive.</param>
        /// <param name="error">The error to return when the length is outside the range or the string is null.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when the length is within range; otherwise,
        /// a failed result with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<string> LengthInRange(string? value, int minInclusive, int maxInclusive, DomainError error)
            => value is not null && value.Length >= minInclusive && value.Length <= maxInclusive ? Result<string>.Ok(value) : Result<string>.Failure(error);

        /// <summary>
        /// Ensures that a comparable value lies within the specified inclusive range.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IComparable{T}"/>.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="minInclusive">The minimum allowed value, inclusive.</param>
        /// <param name="maxInclusive">The maximum allowed value, inclusive.</param>
        /// <param name="error">The error to return when the value is outside the range.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when within range; otherwise, a failed result
        /// with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> InRange<T>(T value, T minInclusive, T maxInclusive, DomainError error) where T : IComparable<T>
            => value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) <= 0 ? Result<T>.Ok(value) : Result<T>.Failure(error);

        /// <summary>
        /// Ensures that a comparable value is greater than the specified threshold.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IComparable{T}"/>.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="threshold">The lower bound that <paramref name="value"/> must exceed.</param>
        /// <param name="error">The error to return when the value is not greater than the threshold.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when greater than the threshold; otherwise,
        /// a failed result with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> GreaterThan<T>(T value, T threshold, DomainError error) where T : IComparable<T>
            => value.CompareTo(threshold) > 0 ? Result<T>.Ok(value) : Result<T>.Failure(error);

        /// <summary>
        /// Ensures that a comparable value is less than the specified threshold.
        /// </summary>
        /// <typeparam name="T">The type of the value, which must implement <see cref="IComparable{T}"/>.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="threshold">The upper bound that <paramref name="value"/> must be less than.</param>
        /// <param name="error">The error to return when the value is not less than the threshold.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when less than the threshold; otherwise,
        /// a failed result with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> LessThan<T>(T value, T threshold, DomainError error) where T : IComparable<T>
            => value.CompareTo(threshold) < 0 ? Result<T>.Ok(value) : Result<T>.Failure(error);

        /// <summary>
        /// Ensures that the specified enum value is defined for its enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value to validate.</param>
        /// <param name="error">The error to return when <paramref name="value"/> is not a defined enum value.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when defined; otherwise, a failed result with
        /// <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TEnum> IsDefined<TEnum>(TEnum value, DomainError error) where TEnum : struct, Enum
            => Enum.IsDefined(value) ? Result<TEnum>.Ok(value) : Result<TEnum>.Failure(error);

        /// <summary>
        /// Ensures that a <see cref="Guid"/> value is not <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="value">The GUID to test.</param>
        /// <param name="error">The error to return when <paramref name="value"/> is <see cref="Guid.Empty"/>.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when not empty; otherwise, a failed result with
        /// <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Guid> NotEmpty(Guid value, DomainError error)
            => value != Guid.Empty ? Result<Guid>.Ok(value) : Result<Guid>.Failure(error);

        /// <summary>
        /// Ensures that a <see cref="DateTimeOffset"/> value represents a UTC instant.
        /// </summary>
        /// <param name="value">The date/time value to test.</param>
        /// <param name="error">The error to return when the value is not UTC.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when UTC; otherwise, a failed result with
        /// <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<DateTimeOffset> IsUtc(DateTimeOffset value, DomainError error)
            => value.Offset == TimeSpan.Zero ? Result<DateTimeOffset>.Ok(value) : Result<DateTimeOffset>.Failure(error);

        /// <summary>
        /// Ensures that a <see cref="DateTimeOffset"/> value does not lie in the future relative to
        /// <see cref="DateTimeOffset.UtcNow"/>.
        /// </summary>
        /// <param name="value">The date/time value to test.</param>
        /// <param name="error">The error to return when the value lies in the future.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when not in the future; otherwise, a failed
        /// result with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<DateTimeOffset> NotInFuture(DateTimeOffset value, DomainError error)
            => value <= DateTimeOffset.UtcNow ? Result<DateTimeOffset>.Ok(value) : Result<DateTimeOffset>.Failure(error);

        /// <summary>
        /// Ensures that a <see cref="DateTimeOffset"/> value does not lie in the past relative to
        /// <see cref="DateTimeOffset.UtcNow"/>.
        /// </summary>
        /// <param name="value">The date/time value to test.</param>
        /// <param name="error">The error to return when the value lies in the past.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when not in the past; otherwise, a failed
        /// result with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<DateTimeOffset> NotInPast(DateTimeOffset value, DomainError error)
            => value >= DateTimeOffset.UtcNow ? Result<DateTimeOffset>.Ok(value) : Result<DateTimeOffset>.Failure(error);

        /// <summary>
        /// Ensures that a collection is not <see langword="null"/> and contains at least one element.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="error">The error to return when the collection is null or empty.</param>
        /// <returns>
        /// A successful result containing an array copy of the collection when non-empty; otherwise, a failed result
        /// with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<IReadOnlyCollection<T>> NotEmpty<T>(IEnumerable<T>? collection, DomainError error)
            => collection is not null && System.Linq.Enumerable.Any(collection) ? Result<IReadOnlyCollection<T>>.Ok(System.Linq.Enumerable.ToArray(collection)) : Result<IReadOnlyCollection<T>>.Failure(error);

        /// <summary>
        /// Ensures that a collection is not <see langword="null"/> and does not contain any <see langword="null"/> elements.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="error">The error to return when the collection is null or contains null elements.</param>
        /// <returns>
        /// A successful result containing an array copy of the collection when valid; otherwise, a failed result
        /// with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<IReadOnlyCollection<T>> NoNullElements<T>(IEnumerable<T>? collection, DomainError error)
            => collection is not null && !System.Linq.Enumerable.Any(collection, e => e is null) ? Result<IReadOnlyCollection<T>>.Ok(System.Linq.Enumerable.ToArray(collection)) : Result<IReadOnlyCollection<T>>.Failure(error);

        /// <summary>
        /// Ensures that the specified value satisfies a given predicate.
        /// </summary>
        /// <typeparam name="TValue">The type of the value being tested.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="predicate">The predicate that must return <see langword="true"/> for the value to be accepted.</param>
        /// <param name="error">The error to return when the predicate evaluates to <see langword="false"/>.</param>
        /// <returns>
        /// A successful result containing <paramref name="value"/> when the predicate returns true; otherwise, a
        /// failed result with <paramref name="error"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TValue> Satisfies<TValue>(TValue value, Func<TValue, bool> predicate, DomainError error)
            where TValue : notnull
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return predicate(value) ? Result<TValue>.Ok(value) : Result<TValue>.Failure(error);
        }

        /// <summary>
        /// Attempts to parse an input string into a value using the specified parsing delegate, returning a
        /// <see cref="Result{TValue}"/> that represents success or failure.
        /// </summary>
        /// <typeparam name="TValue">The type of the parsed value.</typeparam>
        /// <param name="input">The input string to parse.</param>
        /// <param name="tryParse">A delegate that performs the parse and returns a success flag and value.</param>
        /// <param name="error">The error to return when the input is null or parsing fails.</param>
        /// <returns>
        /// A successful result containing the parsed value when parsing succeeds; otherwise, a failed result with
        /// <paramref name="error"/>.
        /// </returns>
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
