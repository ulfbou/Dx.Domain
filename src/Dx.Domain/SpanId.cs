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

namespace Dx.Domain
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a unique identifier for a span within a distributed tracing system.
    /// </summary>
    /// <remarks>A SpanId is typically used to correlate and track individual operations or requests across
    /// system boundaries. The value is a 64-bit unsigned integer, and a SpanId with a value of 0 is considered empty.
    /// SpanId is immutable and supports value equality comparison.</remarks>
    [DebuggerDisplay("SpanId = {Value}")]
    public readonly struct SpanId : IEquatable<SpanId>
    {
        /// <summary>
        /// Gets an empty <see cref="SpanId"/> with a value of <c>0</c>.
        /// </summary>
        public static readonly SpanId Empty = new(0UL);

        /// <summary>
        /// Gets the underlying numeric span value.
        /// </summary>
        public ulong Value => _value;

        private readonly ulong _value;

        /// <summary>
        /// Initializes a new <see cref="SpanId"/> with the provided numeric value.
        /// </summary>
        /// <param name="value">The underlying span value.</param>
        public SpanId(ulong value) => _value = value;

        /// <summary>
        /// Creates a new random <see cref="SpanId"/> instance.
        /// </summary>
        /// <returns>A new <see cref="SpanId"/> whose <see cref="Value"/> is non-zero with high probability.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanId New()
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
        public bool Equals(SpanId other) => _value == other._value;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is SpanId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <summary>
        /// Determines whether two <see cref="SpanId"/> values are equal.
        /// </summary>
        /// <param name="a">The first identifier to compare.</param>
        /// <param name="b">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(SpanId a, SpanId b) => a.Equals(b);

        /// <summary>
        /// Determines whether two <see cref="SpanId"/> values are not equal.
        /// </summary>
        /// <param name="a">The first identifier to compare.</param>
        /// <param name="b">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(SpanId a, SpanId b) => !a.Equals(b);

        /// <inheritdoc />
        public override string ToString()
            => IsEmpty ? "SpanId.Empty" : $"SpanId(v={_value})";
    }
}
