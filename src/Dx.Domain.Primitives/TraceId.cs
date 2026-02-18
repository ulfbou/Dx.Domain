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

using Dx.Domain;
using System.Runtime.CompilerServices;

namespace Dx.Domain.Primitives
{
    /// <summary>
    /// Represents a 128-bit trace identifier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Canonical string format is 32 lowercase hexadecimal characters (16 for high part + 16 for low part),
    /// aligned with W3C Trace Context standards.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("{ToString(),nq}")]
    public readonly struct TraceId :
        IIdentity,
        IEquatable<TraceId>,
        IParsable<TraceId>,
        ISpanFormattable
    {
        /// <summary>An empty trace identifier with both high and low parts set to zero.</summary>
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

        /// <summary>
        /// Creates a <see cref="TraceId"/> from its high and low 64-bit parts.
        /// </summary>
        /// <param name="hi">The high 64 bits of the trace identifier.</param>
        /// <param name="lo">The low 64 bits of the trace identifier.</param>
        /// <returns>A new <see cref="TraceId"/> instance composed of the specified parts.</returns>
        public static TraceId FromParts(ulong hi, ulong lo) => new(hi, lo);

        /// <inheritdoc />
        public static TraceId Parse(string s, IFormatProvider? provider)
        {
            ArgumentNullException.ThrowIfNull(s);

            if (s.Length != 32)
                throw new FormatException("TraceId must be exactly 32 hexadecimal characters.");

            return new TraceId(
                ulong.Parse(s.AsSpan(0, 16), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                ulong.Parse(s.AsSpan(16, 16), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
        }

        /// <inheritdoc />
        public static bool TryParse(string? s, IFormatProvider? provider, out TraceId result)
        {
            result = default;

            if (s is null || s.Length != 32)
                return false;

            if (!ulong.TryParse(s.AsSpan(0, 16), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hi))
                return false;
            if (!ulong.TryParse(s.AsSpan(16, 16), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var lo))
                return false;

            result = new TraceId(hi, lo);
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this trace identifier is empty (both high and low parts are zero).
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
        /// <param name="destination">The span to write the formatted value into. Must have capacity for at least 32 characters.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters written to the destination.</param>
        /// <returns><see langword="true"/> if formatting succeeded; otherwise, <see langword="false"/>.</returns>
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
        /// If <paramref name="format"/> is null or empty, defaults to the canonical 32-character hex format.
        /// The <paramref name="provider"/> parameter is currently ignored.
        /// </remarks>
        /// <param name="destination">The span to write the formatted value into.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters written to the destination.</param>
        /// <param name="format">The format specifier. This parameter is currently ignored.</param>
        /// <param name="provider">The format provider. This parameter is currently ignored.</param>
        /// <returns><see langword="true"/> if formatting succeeded; otherwise, <see langword="false"/>.</returns>
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            // 32 hex characters are required for the combined 128-bit value.
            if (destination.Length < 32)
            {
                charsWritten = 0;
                return false;
            }

            if (format.IsEmpty)
            {
                format = "x16";
            }

            if (provider == null)
            {
                provider = CultureInfo.InvariantCulture;
            }

            // Format as hi then lo in big-endian style for readability and determinism.
            if (!_hi.TryFormat(destination.Slice(0, 16), out var hiChars, format, provider))
            {
                charsWritten = 0;
                return false;
            }

            if (!_lo.TryFormat(destination.Slice(16, 16), out var loChars, format, provider))
            {
                charsWritten = 0;
                return false;
            }

            charsWritten = hiChars + loChars;
            return true;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            Span<char> buffer = stackalloc char[32];

            return TryFormat(buffer, out var charsWritten)
                ? new string(buffer.Slice(0, charsWritten))
                : string.Empty;
        }

        /// <summary>
        /// Formats this identifier as a string.
        /// </summary>
        /// <remarks>
        /// The <paramref name="format"/> and <paramref name="formatProvider"/> parameters are currently ignored.
        /// The identifier is always rendered as 32 lowercase hexadecimal characters using invariant culture.
        /// </remarks>
        /// <param name="format">The format specifier. This parameter is currently ignored.</param>
        /// <param name="formatProvider">The format provider. This parameter is currently ignored.</param>
        /// <returns>A string representation of this trace identifier in canonical format.</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            Span<char> buffer = stackalloc char[32];

            return TryFormat(buffer, out var charsWritten)
                ? new string(buffer.Slice(0, charsWritten))
                : string.Empty;
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => IsEmpty ? "TraceId.Empty" : ToString();
    }
}
