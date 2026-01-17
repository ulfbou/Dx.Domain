// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="TraceId.cs" company="Dx.Domain Team">
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
using System.Runtime.CompilerServices;

namespace Dx.Domain.Primitives
{
    /// <summary>
    /// Represents a 128-bit trace identifier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Canonical string format is <c>"hi:lo"</c>, both unsigned decimals.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("{ToString(),nq}")]
    public readonly struct TraceId :
        IEquatable<TraceId>,
        IParsable<TraceId>,
        ISpanFormattable
    {
        /// <summary>An empty trace identifier.</summary>
        public static readonly TraceId Empty = new(0, 0);

        private readonly ulong _hi;
        private readonly ulong _lo;

        private TraceId(ulong hi, ulong lo)
        {
            _hi = hi;
            _lo = lo;
        }

        /// <summary>
        /// Creates a new random <see cref="TraceId"/> instance.
        /// </summary>
        /// <returns>A new <see cref="TraceId"/> with a uniformly random 128-bit value.</returns>
        public static TraceId New()
        {
            var g = Guid.NewGuid().ToByteArray();
            return new TraceId(
                BitConverter.ToUInt64(g, 0),
                BitConverter.ToUInt64(g, 8));
        }

        /// <summary>Creates a <see cref="TraceId"/> from its parts.</summary>
        /// <param name="hi">The high 64 bits.</param>
        /// <param name="lo">The low 64 bits.</param>
        /// <returns>A new <see cref="TraceId"/> instance.</returns>
        public static TraceId FromParts(ulong hi, ulong lo) => new(hi, lo);

        /// <inheritdoc />
        public static TraceId Parse(string s, IFormatProvider? provider)
        {
            ArgumentNullException.ThrowIfNull(s);

            var parts = s.Split(':');

            if (parts.Length != 2)
                throw new FormatException("TraceId must be in 'hi:lo' format.");

            return new TraceId(
                ulong.Parse(parts[0], CultureInfo.InvariantCulture),
                ulong.Parse(parts[1], CultureInfo.InvariantCulture));
        }

        /// <inheritdoc />
        public static bool TryParse(string? s, IFormatProvider? provider, out TraceId result)
        {
            result = default;

            if (s is null)
                return false;

            var parts = s.Split(':');

            if (parts.Length != 2)
                return false;

            if (!ulong.TryParse(parts[0], out var hi))
                return false;
            if (!ulong.TryParse(parts[1], out var lo))
                return false;

            result = new TraceId(hi, lo);
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this identifier is empty.
        /// </summary>
        public bool IsEmpty => _hi == 0UL && _lo == 0UL;

        /// <inheritdoc />
        public bool Equals(TraceId other) => _hi == other._hi && _lo == other._lo;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is TraceId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(_hi, _lo);

        /// <summary>
        /// Determines whether two <see cref="TraceId"/> values are equal.
        /// </summary>
        /// <param name="left">The first identifier to compare.</param>
        /// <param name="right">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(TraceId left, TraceId right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="TraceId"/> values are not equal.
        /// </summary>
        /// <param name="left">The first identifier to compare.</param>
        /// <param name="right">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(TraceId left, TraceId right) => !left.Equals(right);

        /// <summary>
        /// Attempts to format the identifier as a canonical 32-character hexadecimal string without separators.
        /// </summary>
        public bool TryFormat(Span<char> destination, out int charsWritten)
        {
            // 32 hex characters are required for the combined 128-bit value.
            if (destination.Length < 32)
            {
                charsWritten = 0;
                return false;
            }

            // Format as hi then lo in big-endian style for readability and determinism.
            if (!_hi.TryFormat(destination.Slice(0, 16), out var hiChars, "x16", CultureInfo.InvariantCulture))
            {
                charsWritten = 0;
                return false;
            }

            if (!_lo.TryFormat(destination.Slice(16, 16), out var loChars, "x16", CultureInfo.InvariantCulture))
            {
                charsWritten = 0;
                return false;
            }

            charsWritten = hiChars + loChars;
            return true;
        }

        /// <summary>
        /// General-purpose span formatting implementation used by composite formatting APIs.
        /// </summary>
        /// <remarks>
        /// The <paramref name="format"/> is currently ignored and the identifier is always rendered as 32 hex characters
        /// using invariant culture.
        /// </remarks>
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => TryFormat(destination, out charsWritten);

        /// <inheritdoc />
        public override string ToString()
        {
            Span<char> buffer = stackalloc char[32];

            return TryFormat(buffer, out var charsWritten)
                ? new string(buffer.Slice(0, charsWritten))
                : string.Empty;
        }

        /// <inheritdoc />
        public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => IsEmpty ? "TraceId.Empty" : ToString();
    }
}
