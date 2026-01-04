// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxDomain.Require.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Errors;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Dx.Domain
{
    public static partial class DxDomain
    {
        internal static partial class Kernel
        {
            /// <summary>
            /// Result-based precondition checks used by kernel primitives. Explicitly non-ergonomic.
            /// </summary>
            internal static partial class Require
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<Unit> That(bool condition, DomainError error)
                    => condition ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<Unit> That(bool condition, Func<DomainError> errorFactory)
                {
                    Invariant.That(errorFactory is not null, DomainError.Create(Codes.Invariant.Violation, "The error factory function cannot be null."));
                    return condition ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(errorFactory());
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<T> NotNull<T>(T? value, DomainError error) where T : class
                    => value is null ? Result<T>.Failure(error) : Result<T>.Success(value);
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<T> NotNull<T>(T? value, Func<DomainError> errorFactory) where T : class
                {
                    Invariant.That(errorFactory is not null, DomainError.Create(Codes.Invariant.Violation, "The error factory function cannot be null."));
                    return value is null ? Result<T>.Failure(errorFactory()) : Result<T>.Success(value);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<string> NotNullOrWhiteSpace(string? value, DomainError error)
                    => string.IsNullOrWhiteSpace(value) ? Result<string>.Failure(error) : Result<string>.Success(value!);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<string> NotNullOrEmpty(string? value, DomainError error)
                    => string.IsNullOrEmpty(value) ? Result<string>.Failure(error) : Result<string>.Success(value!);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<string> Matches(string? value, string pattern, DomainError error, RegexOptions options = RegexOptions.None)
                    => value is not null && Regex.IsMatch(value, pattern, options) ? Result<string>.Success(value) : Result<string>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<string> LengthInRange(string? value, int minInclusive, int maxInclusive, DomainError error)
                    => value is not null && value.Length >= minInclusive && value.Length <= maxInclusive ? Result<string>.Success(value) : Result<string>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<T> InRange<T>(T value, T minInclusive, T maxInclusive, DomainError error) where T : IComparable<T>
                    => value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) <= 0 ? Result<T>.Success(value) : Result<T>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<T> GreaterThan<T>(T value, T threshold, DomainError error) where T : IComparable<T>
                    => value.CompareTo(threshold) > 0 ? Result<T>.Success(value) : Result<T>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<T> LessThan<T>(T value, T threshold, DomainError error) where T : IComparable<T>
                    => value.CompareTo(threshold) < 0 ? Result<T>.Success(value) : Result<T>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<TEnum> IsDefined<TEnum>(TEnum value, DomainError error) where TEnum : struct, Enum
                    => Enum.IsDefined(value) ? Result<TEnum>.Success(value) : Result<TEnum>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<Guid> NotEmpty(Guid value, DomainError error)
                    => value != Guid.Empty ? Result<Guid>.Success(value) : Result<Guid>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<DateTimeOffset> IsUtc(DateTimeOffset value, DomainError error)
                    => value.Offset == TimeSpan.Zero ? Result<DateTimeOffset>.Success(value) : Result<DateTimeOffset>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<DateTimeOffset> NotInFuture(DateTimeOffset value, DomainError error)
                    => value <= DateTimeOffset.UtcNow ? Result<DateTimeOffset>.Success(value) : Result<DateTimeOffset>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<DateTimeOffset> NotInPast(DateTimeOffset value, DomainError error)
                    => value >= DateTimeOffset.UtcNow ? Result<DateTimeOffset>.Success(value) : Result<DateTimeOffset>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<IReadOnlyCollection<T>> NotEmpty<T>(IEnumerable<T>? collection, DomainError error)
                    => collection is not null && Enumerable.Any(collection) ? Result<IReadOnlyCollection<T>>.Success((IReadOnlyCollection<T>)Enumerable.ToArray(collection)) : Result<IReadOnlyCollection<T>>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<IReadOnlyCollection<T>> NoNullElements<T>(IEnumerable<T>? collection, DomainError error)
                    => collection is not null && !Enumerable.Any(collection, e => e is null)
                    ? Result<IReadOnlyCollection<T>>.Success((IReadOnlyCollection<T>)Enumerable.ToArray(collection))
                    : Result<IReadOnlyCollection<T>>.Failure(error);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Result<TValue> Satisfies<TValue>(TValue value, Func<TValue, bool> predicate, DomainError error)
                    where TValue : notnull
                {
                    Invariant.That(predicate is not null, Kernel.Faults.Guard.ParameterCannotBeNull(nameof(predicate)));

                    return predicate(value) ? Result<TValue>.Success(value) : Result<TValue>.Failure(error);
                }
            }
        }
    }
}
