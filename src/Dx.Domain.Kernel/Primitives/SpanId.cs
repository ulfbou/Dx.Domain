// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="SpanId.cs" company="Dx.Domain Team">
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
    /// Represents a unique identifier for a span within a distributed tracing system.
    /// </summary>
    /// <remarks>A SpanId is typically used to correlate and track individual operations or requests across
    /// system boundaries. The value is a 64-bit unsigned integer, and a SpanId with a value of 0 is considered empty.
    /// SpanId is immutable and supports value equality comparison.</remarks>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct SpanId : IEquatable<SpanId>, IComparable<SpanId>, IComparable, ISpanFormattable
    {
        /// <summary>
        /// Gets an empty <see cref="SpanId"/> with a value of <c>0</c>.
        /// </summary>
        public static readonly SpanId Empty = new(0UL);

        private readonly ulong _value;

        /// <summary>
        /// Gets the underlying numeric span value.
        /// </summary>
        public ulong Value => _value;

        /// <summary>
        /// Initializes a new <see cref="SpanId"/> with the provided numeric value.
        /// </summary>
        /// <param name="value">The underlying span value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SpanId(ulong value) => _value = value;

        /// <summary>
        /// Creates a new random <see cref="SpanId"/> instance.
        /// </summary>
        /// <returns>A new <see cref="SpanId"/> whose <see cref="Value"/> is non-zero with high probability.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SpanId New()
        {
            Span<byte> buffer = stackalloc byte[8];
            Random.Shared.NextBytes(buffer);
            return new SpanId(BitConverter.ToUInt64(buffer));
        }

        /// <summary>
        /// Gets a value indicating whether this identifier is empty.
        /// </summary>
        public bool IsEmpty => _value == 0UL;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SpanId other) => _value == other._value;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is SpanId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(SpanId other) => _value.CompareTo(other._value);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object? obj)
        {
            if (obj is null || obj is not SpanId other)
            {
                return 1;
            }

            return CompareTo(other);
        }

        /// <summary>
        /// Attempts to format the value as a hexadecimal string into the provided span.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFormat(Span<char> destination, out int charsWritten)
            => _value.TryFormat(destination, out charsWritten, "x16", CultureInfo.InvariantCulture);

        /// <summary>
        /// General-purpose span formatting implementation used by composite formatting APIs.
        /// </summary>
        /// <remarks>
        /// The <paramref name="format"/> is optional; when omitted, the value is rendered as 16 lowercase hexadecimal
        /// characters.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            var effectiveFormat = format.IsEmpty ? "x16" : new string(format);
            return _value.TryFormat(destination, out charsWritten, effectiveFormat, provider ?? CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => IsEmpty ? "SpanId.Empty" : _value.ToString("x16", CultureInfo.InvariantCulture);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string? format, IFormatProvider? formatProvider)
            => _value.ToString(format ?? "x16", formatProvider ?? CultureInfo.InvariantCulture);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SpanId left, SpanId right) => left.Equals(right);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SpanId left, SpanId right) => !left.Equals(right);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(SpanId left, SpanId right) => left.CompareTo(right) < 0;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(SpanId left, SpanId right) => left.CompareTo(right) <= 0;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(SpanId left, SpanId right) => left.CompareTo(right) > 0;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(SpanId left, SpanId right) => left.CompareTo(right) >= 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => IsEmpty ? "SpanId.Empty" : _value.ToString("x16", CultureInfo.InvariantCulture);
    }
}
