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


namespace Dx.Domain.Primitives
{
    /// <summary>
    /// Represents a 64-bit opaque span identifier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Canonical string format is 16 lowercase hexadecimal characters (<c>"x16"</c>) using invariant culture,
    /// aligned with W3C Trace Context standards.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("{ToString(),nq}")]
    public readonly struct SpanId :
        IIdentity,
        IEquatable<SpanId>,
        IParsable<SpanId>,
        ISpanFormattable
    {
        /// <summary>An empty span identifier with a value of zero.</summary>
        public static readonly SpanId Empty = new(0);

        /// <summary>Gets the underlying 64-bit unsigned integer value of this span identifier.</summary>
        public ulong Value { get; }

        private SpanId(ulong value) => Value = value;

        /// <summary>
        /// Creates a new random <see cref="SpanId"/> instance.
        /// </summary>
        /// <returns>A new <see cref="SpanId"/> whose <see cref="Value"/> is non-zero with high probability.</returns>
        public static SpanId New()
            => new(BitConverter.ToUInt64(Guid.NewGuid().ToByteArray()));

        /// <summary>
        /// Creates a <see cref="SpanId"/> from a 64-bit unsigned integer value.
        /// </summary>
        /// <param name="value">The underlying 64-bit unsigned integer value.</param>
        /// <returns>A new <see cref="SpanId"/> instance wrapping the specified value.</returns>
        public static SpanId FromUInt64(ulong value) => new(value);

        /// <summary>
        /// Gets a value indicating whether this span identifier is empty (has a value of zero).
        /// </summary>
        public bool IsEmpty => Value == 0UL;

        /// <summary>
        /// Parses a canonical 16-character hexadecimal string into a <see cref="SpanId"/>.
        /// </summary>
        /// <param name="s">The input string to parse. Must be exactly 16 hex characters.</param>
        /// <param name="provider">Ignored. Parsing is culture-invariant.</param>
        /// <returns>The parsed <see cref="SpanId"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="s"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">Thrown when <paramref name="s"/> is not exactly 16 hex characters.</exception>
        public static SpanId Parse(string s, IFormatProvider? provider)
        {
            ArgumentNullException.ThrowIfNull(s);

            if (s.Length != 16)
            {
                throw new FormatException("SpanId must be exactly 16 hexadecimal characters.");
            }

            return new SpanId(ulong.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Attempts to parse a canonical 16-character hexadecimal string into a <see cref="SpanId"/>.
        /// </summary>
        /// <param name="s">The input string to parse (may be <see langword="null"/>).</param>
        /// <param name="provider">Optional format provider; if <see langword="null"/>, uses invariant culture.</param>
        /// <param name="result">The parsed <see cref="SpanId"/> if successful; otherwise the default value.</param>
        /// <returns><see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.</returns>
        public static bool TryParse(string? s, IFormatProvider? provider, out SpanId result)
        {
            if (s is not null && s.Length == 16 &&
                ulong.TryParse(s, NumberStyles.HexNumber, provider ?? CultureInfo.InvariantCulture, out var value))
            {
                result = new SpanId(value);
                return true;
            }

            result = default;
            return false;
        }

        /// <inheritdoc />
        public bool Equals(SpanId other) => Value == other.Value;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is SpanId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Formats this identifier as a 16-character lowercase hexadecimal string.
        /// </summary>
        /// <param name="destination">The span to write the formatted value into.</param>
        /// <param name="charsWritten">The number of characters written to the destination.</param>
        /// <returns><see langword="true"/> if formatting succeeded; otherwise, <see langword="false"/>.</returns>
        public bool TryFormat(Span<char> destination, out int charsWritten)
            => Value.TryFormat(destination, out charsWritten, "x16", CultureInfo.InvariantCulture);

        /// <summary>
        /// Tries to format this identifier into the destination span.
        /// </summary>
        /// <param name="destination">The span to write the formatted value into.</param>
        /// <param name="charsWritten">The number of characters written to the destination.</param>
        /// <param name="format">The format specifier. If <see langword="null"/> or empty, defaults to <c>"x16"</c>.</param>
        /// <param name="provider">The format provider. If <see langword="null"/>, uses <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <returns><see langword="true"/> if formatting succeeded; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// If <paramref name="format"/> is <see langword="null"/> or empty, defaults to canonical format (<c>"x16"</c>).
        /// If <paramref name="provider"/> is <see langword="null"/>, uses <see cref="CultureInfo.InvariantCulture"/>.
        /// </remarks>
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            var normalizedFormat = format.IsEmpty ? "x16" : format;
            var normalizedProvider = provider ?? CultureInfo.InvariantCulture;
            return ((ISpanFormattable)Value).TryFormat(destination, out charsWritten, normalizedFormat, normalizedProvider);
        }

        /// <inheritdoc />
        public override string ToString()
            => IsEmpty ? "SpanId.Empty" : Value.ToString("x16", CultureInfo.InvariantCulture);

        /// <summary>
        /// Formats this identifier as a string using the specified format and culture.
        /// </summary>
        /// <remarks>
        /// If <paramref name="format"/> is <see langword="null"/> or empty, defaults to canonical format (<c>"x16"</c>).
        /// If <paramref name="formatProvider"/> is <see langword="null"/>, uses <see cref="CultureInfo.InvariantCulture"/>.
        /// </remarks>
        /// <param name="format">The format specifier. If <see langword="null"/> or empty, defaults to <c>"x16"</c>.</param>
        /// <param name="formatProvider">
        /// The format provider. If <see langword="null"/>, uses <see cref="CultureInfo.InvariantCulture"/>.
        /// </param>
        /// <returns>A formatted string representation of this identifier.</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(string.IsNullOrEmpty(format) ? "x16" : format!, formatProvider ?? CultureInfo.InvariantCulture);

        /// <summary>
        /// Determines whether two <see cref="SpanId"/> values are equal.
        /// </summary>
        /// <param name="left">The first span identifier to compare.</param>
        /// <param name="right">The second span identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(SpanId left, SpanId right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="SpanId"/> values are not equal.
        /// </summary>
        /// <param name="left">The first span identifier to compare.</param>
        /// <param name="right">The second span identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(SpanId left, SpanId right) => !left.Equals(right);
    }
}
