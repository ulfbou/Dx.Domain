// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright>
//   Copyright (c) 2025 Dx.Domain Team.
// </copyright>
// <license>
//   This software is licensed under the MIT License.
//   See the project's root <c>LICENSE</c> file for details.
//   Contributions are welcome, subject to the terms of the project's license.
//   See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;

using Dx.Domain;

namespace Dx.Domain.Primitives
{
    /// <summary>
    /// Represents a strongly typed identifier for an actor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Canonical string format is <c>"N"</c> (32 hexadecimal characters, no separators).
    /// </para>
    /// <para>
    /// <see cref="ActorId"/> does not permit empty values.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("{Value,nq}")]
    public readonly struct ActorId :
        IIdentity,
        IEquatable<ActorId>,
        IParsable<ActorId>,
        ISpanFormattable
    {
        /// <summary>Gets the underlying GUID value of this actor identifier.</summary>
        public Guid Value { get; }

        private ActorId(Guid value) => Value = value;

        /// <summary>
        /// Creates a new random <see cref="ActorId"/> instance.
        /// </summary>
        /// <returns>A new <see cref="ActorId"/> with a randomly generated GUID value.</returns>
        public static ActorId New() => new(Guid.NewGuid());

        /// <summary>
        /// Creates an <see cref="ActorId"/> from a non-empty GUID.
        /// </summary>
        /// <param name="value">The GUID value. Must not be <see cref="Guid.Empty"/>.</param>
        /// <returns>A new <see cref="ActorId"/> instance wrapping the specified GUID.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.</exception>
        public static ActorId FromGuid(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("ActorId cannot be empty.", nameof(value));

            return new ActorId(value);
        }

        /// <inheritdoc />
        public static ActorId Parse(string s, IFormatProvider? provider)
            => FromGuid(Guid.ParseExact(s ?? throw new ArgumentNullException(nameof(s)), "N"));

        /// <inheritdoc />
        public static bool TryParse(string? s, IFormatProvider? provider, out ActorId result)
        {
            if (Guid.TryParseExact(s, "N", out var g) && g != Guid.Empty)
            {
                result = new ActorId(g);
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// General-purpose span formatting implementation used by composite formatting APIs.
        /// </summary>
        /// <remarks>
        /// If <paramref name="format"/> is null or empty, defaults to canonical format (<c>"N"</c>).
        /// If <paramref name="provider"/> is null, uses <see cref="CultureInfo.InvariantCulture"/>.
        /// </remarks>
        /// <param name="destination">The span to write the formatted value into.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters written to the destination.</param>
        /// <param name="format">The format specifier. If null or empty, defaults to <c>"N"</c>.</param>
        /// <param name="provider">The format provider. If null, uses <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <returns><see langword="true"/> if formatting succeeded; otherwise, <see langword="false"/>.</returns>
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            var normalizedFormat = format.IsEmpty ? "N" : format;
            var normalizedProvider = provider ?? CultureInfo.InvariantCulture;
            return ((ISpanFormattable)Value).TryFormat(destination, out charsWritten, normalizedFormat, normalizedProvider);
        }

        /// <inheritdoc />
        public bool Equals(ActorId other)
            => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is ActorId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString()
            => Value.ToString("N", CultureInfo.InvariantCulture);

        /// <summary>
        /// Formats this identifier as a string using the specified format and culture.
        /// </summary>
        /// <remarks>
        /// If <paramref name="format"/> is null or empty, defaults to canonical format (<c>"N"</c>).
        /// If <paramref name="formatProvider"/> is null, uses <see cref="CultureInfo.InvariantCulture"/>.
        /// </remarks>
        /// <param name="format">The format specifier. If null or empty, defaults to <c>"N"</c>.</param>
        /// <param name="formatProvider">The format provider. If null, uses <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <returns>A string representation of this actor identifier.</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(string.IsNullOrEmpty(format) ? "N" : format!, formatProvider ?? CultureInfo.InvariantCulture);

        /// <summary>
        /// Determines whether two <see cref="ActorId"/> values are equal.
        /// </summary>
        /// <param name="left">The first actor identifier to compare.</param>
        /// <param name="right">The second actor identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(ActorId left, ActorId right)
            => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="ActorId"/> values are not equal.
        /// </summary>
        /// <param name="left">The first actor identifier to compare.</param>
        /// <param name="right">The second actor identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(ActorId left, ActorId right)
            => !(left == right);
    }
}
