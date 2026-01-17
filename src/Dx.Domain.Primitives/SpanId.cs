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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Dx.Domain.Primitives
{
    /// <summary>
    /// Represents a 64-bit opaque span identifier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Canonical string format is unsigned decimal (<see cref="ulong"/>).
    /// </para>
    /// </remarks>
    [DebuggerDisplay("{Value}")]
    public readonly struct SpanId :
        IEquatable<SpanId>,
        IParsable<SpanId>,
        ISpanFormattable
    {
        /// <summary>An empty span identifier.</summary>
        public static readonly SpanId Empty = new(0);

        /// <summary>The underlying value.</summary>
        public ulong Value { get; }

        private SpanId(ulong value) => Value = value;

        /// <summary>
        /// Creates a new random <see cref="SpanId"/> instance.
        /// </summary>
        /// <returns>A new <see cref="SpanId"/> whose <see cref="Value"/> is non-zero with high probability.</returns>
        public static SpanId New()
            => new(BitConverter.ToUInt64(Guid.NewGuid().ToByteArray()));

        /// <summary>Creates a <see cref="SpanId"/> from a value.</summary>
        /// <param name="value">The underlying value.</param>
        /// <returns>A new <see cref="SpanId"/> instance.</returns>
        public static SpanId FromUInt64(ulong value) => new(value);

        /// <summary>
        /// Gets a value indicating whether this identifier is empty.
        /// </summary>
        public bool IsEmpty => Value == 0UL;

        /// <inheritdoc />
        public static SpanId Parse(string s, IFormatProvider? provider)
            => new(ulong.Parse(s ?? throw new ArgumentNullException(nameof(s)), CultureInfo.InvariantCulture));

        /// <inheritdoc />
        public static bool TryParse(string? s, IFormatProvider? provider, out SpanId result)
        {
            if (ulong.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
            {
                result = new SpanId(value);
                return true;
            }

            result = default;
            return false;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SpanId other) => Value == other.Value;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is SpanId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public int CompareTo(SpanId other) => Value.CompareTo(other.Value);

        /// <inheritdoc />
        public int CompareTo(object? obj)
        {
            if (obj is null || obj is not SpanId other)
            {
                return 1;
            }

            return CompareTo(other);
        }

        /// <inheritdoc />
        public bool TryFormat(Span<char> destination, out int charsWritten)
            => Value.TryFormat(destination, out charsWritten, "x16", CultureInfo.InvariantCulture);

        /// <inheritdoc />
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => ((ISpanFormattable)Value).TryFormat(destination, out charsWritten, format, provider);

        /// <inheritdoc />
        public override string ToString()
            => IsEmpty ? "SpanId.Empty" : Value.ToString("x16", CultureInfo.InvariantCulture);

        /// <inheritdoc />
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(format ?? "x16", formatProvider ?? CultureInfo.InvariantCulture);

        /// <summary>
        /// Determines whether two <see cref="TraceId"/> values are equal.
        /// </summary>
        /// <param name="left">The first identifier to compare.</param>
        /// <param name="right">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(SpanId left, SpanId right) => left.Equals(right);

        /// <inheritdoc />
        public static bool operator !=(SpanId left, SpanId right) => !left.Equals(right);
    }
}
