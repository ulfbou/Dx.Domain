// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="CorrelationId.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;


namespace Dx.Domain.Primitives
{
    /// <summary>
    /// Correlates related operations across system boundaries.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Empty values are permitted to represent an uncorrelated context.
    /// </para>
    /// <para>
    /// Canonical string format is <c>"N"</c>.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("{Value,nq}")]
    public readonly struct CorrelationId :
        IIdentity,
        IEquatable<CorrelationId>,
        IParsable<CorrelationId>,
        ISpanFormattable
    {
        /// <summary>An empty correlation identifier representing an uncorrelated context.</summary>
        public static readonly CorrelationId Empty = new(Guid.Empty);

        /// <summary>Gets the underlying GUID value of this correlation identifier.</summary>
        public Guid Value { get; }

        private CorrelationId(Guid value) => Value = value;

        /// <summary>
        /// Creates a new random <see cref="CorrelationId"/> instance.
        /// </summary>
        /// <returns>A new <see cref="CorrelationId"/> with a randomly generated GUID value.</returns>
        public static CorrelationId New() => new(Guid.NewGuid());

        /// <summary>
        /// Creates a <see cref="CorrelationId"/> from a GUID value.
        /// </summary>
        /// <param name="value">The GUID value. May be <see cref="Guid.Empty"/> to represent an uncorrelated context.</param>
        /// <returns>A new <see cref="CorrelationId"/> instance wrapping the specified GUID.</returns>
        public static CorrelationId FromGuid(Guid value) => new(value);

        /// <inheritdoc />
        public static CorrelationId Parse(string s, IFormatProvider? provider)
            => new(Guid.ParseExact(s ?? throw new ArgumentNullException(nameof(s)), "N"));

        /// <inheritdoc />
        public static bool TryParse(string? s, IFormatProvider? provider, out CorrelationId result)
        {
            if (Guid.TryParseExact(s, "N", out var g))
            {
                result = new CorrelationId(g);
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
        public bool Equals(CorrelationId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is CorrelationId other && Equals(other);

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
        /// <returns>A string representation of this correlation identifier.</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(string.IsNullOrEmpty(format) ? "N" : format!, formatProvider ?? CultureInfo.InvariantCulture);

        /// <summary>
        /// Determines whether two specified CorrelationId instances are equal.
        /// </summary>
        /// <param name="left">The first identifier to compare.</param>
        /// <param name="right">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(CorrelationId left, CorrelationId right)
            => left.Equals(right);

        /// <summary>
        /// Determines whether two specified CorrelationId instances are not equal.
        /// </summary>
        /// <param name="left">The first identifier to compare.</param>
        /// <param name="right">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(CorrelationId left, CorrelationId right)
            => !(left == right);
    }
}
