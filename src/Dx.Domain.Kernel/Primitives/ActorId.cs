// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ActorId.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Contracts;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

using static Dx.DxDomain;

namespace Dx.Domain.Primitives
{
    /// <summary>
    /// Represents a strongly typed identifier for an actor, backed by a <see cref="Guid"/> value.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="ActorId"/> type to uniquely identify actors within a distributed system or application.
    /// <see cref="ActorId"/> provides value-based equality and can be compared, serialized, or used as a key in collections.
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct ActorId : IIdentity, IParsable<ActorId>, ISpanFormattable, IComparable<ActorId>, IEquatable<ActorId>
    {
        /// <summary>
        /// Represents an uninitialized or default value of the ActorId type.
        /// </summary>
        /// <remarks>Use this field to represent a scenario where no valid actor identifier is available
        /// or assigned. The value of Empty is equivalent to an ActorId constructed with Guid.Empty.</remarks>
        public static readonly ActorId Empty = new ActorId(Guid.Empty);

        /// <summary>
        /// Gets the underlying <see cref="Guid"/> value for this actor.
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Gets a value indicating whether the current value is empty.
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value == Guid.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the ActorId struct using the specified <see cref="Guid"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value that uniquely identifies the actor.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ActorId(Guid value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="ActorId"/> with a freshly generated <see cref="Guid"/> value.
        /// </summary>
        /// <returns>A new unique <see cref="ActorId"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ActorId InternalNew() => new(Guid.NewGuid());

        /// <summary>
        /// Creates a new ActorId instance from the specified GUID value.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value to use for the ActorId. Must not be Guid.Empty.</param>
        /// <returns>An <see cref="ActorId"/> that represents the specified <see cref="Guid"/> value.</returns>
        /// <exception cref="InvariantViolationException">Thrown if the provided <see cref="Guid"/> value is <see cref="Guid.Empty"/>.</exception>
        /// <remarks>The method enforces the invariant that the provided <see cref="Guid"/> value is not
        /// <see cref="Guid.Empty"/>. If the value is <see cref="Guid.Empty"/>, an invariant violation is raised.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ActorId InternalFrom(Guid value)
        {
            // Validates "Monotonic Knowledge" and "Invariant Enforcement"
            Invariant.That(value != Guid.Empty, Faults.FactoryBypass("ActorId cannot be default."));
            return new ActorId(value);
        }

        /// <summary>
        /// Attempts to format the value as a 32-digit hexadecimal string without hyphens into the provided character span.
        /// </summary>
        /// <param name="destination">The span to write the formatted string to.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters written to the span.</param>
        /// <returns><see langword="true"/> if the formatting was successful; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFormat(Span<char> destination, out int charsWritten)
            => Value.TryFormat(destination, out charsWritten, "N");

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ActorId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is ActorId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ActorId left, ActorId right) => left.Equals(right);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ActorId left, ActorId right) => !left.Equals(right);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => Value.ToString("N", CultureInfo.InvariantCulture);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(format ?? "N", formatProvider ?? CultureInfo.InvariantCulture);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(ActorId other) => Value.CompareTo(other.Value);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object? obj)
        {
            if (obj is null || obj is not ActorId other)
            {
                return 1;
            }

            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// General-purpose span formatting implementation used by composite formatting APIs.
        /// </summary>
        /// <remarks>
        /// <see cref="Guid"/>'s span-based formatting does not accept an <see cref="IFormatProvider"/>, so the provider parameter is ignored.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => Value.TryFormat(destination, out charsWritten, format.IsEmpty ? "N" : format);

        /// <inheritdoc />
        public static ActorId Parse(string s, IFormatProvider? provider)
        {
            Invariant.That(s is not null, Faults.FactoryBypass("Null parse input."));
            return InternalFrom(Guid.Parse(s, provider ?? CultureInfo.InvariantCulture));
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ActorId result)
        {
            if (s is not null && Guid.TryParse(s, provider ?? CultureInfo.InvariantCulture, out var guid) && guid != Guid.Empty)
            {
                result = new ActorId(guid);
                return true;
            }
            result = default;
            return false;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(ActorId left, ActorId right) => left.CompareTo(right) < 0;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(ActorId left, ActorId right) => left.CompareTo(right) <= 0;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(ActorId left, ActorId right) => left.CompareTo(right) > 0;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(ActorId left, ActorId right) => left.CompareTo(right) >= 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"ActorId={Value.ToString("N")}";
    }
}
