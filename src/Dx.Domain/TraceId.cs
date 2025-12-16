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

namespace Dx.Domain
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a 128-bit unique identifier for distributed tracing scenarios.
    /// </summary>
    /// <remarks>A TraceId is typically used to uniquely identify a trace across process and service
    /// boundaries in distributed systems. It provides equality comparison, string representation, and supports
    /// generation of random identifiers suitable for tracing use cases. The struct is immutable and
    /// thread-safe.</remarks>
    [DebuggerDisplay("TraceId = hi={_hi}, lo={_lo}")]
    public readonly struct TraceId : IEquatable<TraceId>
    {
        /// <summary>
        /// Gets an empty <see cref="TraceId"/> with all bits set to zero.
        /// </summary>
        public static readonly TraceId Empty = new(0UL, 0UL);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TraceId New()
        {
            Span<byte> buffer = stackalloc byte[16];
            Random.Shared.NextBytes(buffer);
            ulong hi = BitConverter.ToUInt64(buffer.Slice(0, 8));
            ulong lo = BitConverter.ToUInt64(buffer.Slice(8, 8));
            return new TraceId(hi, lo);
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
        /// <param name="a">The first identifier to compare.</param>
        /// <param name="b">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(TraceId a, TraceId b) => a.Equals(b);

        /// <summary>
        /// Determines whether two <see cref="TraceId"/> values are not equal.
        /// </summary>
        /// <param name="a">The first identifier to compare.</param>
        /// <param name="b">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(TraceId a, TraceId b) => !a.Equals(b);

        /// <inheritdoc />
        public override string ToString()
            => IsEmpty ? "TraceId.Empty" : $"TraceId(hi={_hi},lo={_lo})";
    }
}
