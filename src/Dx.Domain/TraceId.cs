// <summary>
//     <list type="bullet">
//         <item>
//             <term>File:</term>
//             <description>TraceId.cs</description>
//         </item>
//         <item>
//             <term>Project:</term>
//             <description>Dx.Domain</description>
//         </item>
//         <item>
//             <term>Description:</term>
//             <description>
//                 Defines a strongly-typed trace identifier used for end-to-end distributed tracing of
//                 operations within and across services.
//             </description>
//         </item>
//     </list>
// </summary>
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
    /// Represents a trace identifier used for distributed tracing scenarios.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct TraceId : IEquatable<TraceId>
    {
        /// <summary>
        /// Gets an empty <see cref="TraceId"/> with all bits set to zero.
        /// </summary>
        public static readonly TraceId Empty = new(0UL, 0UL);

        private readonly ulong _hi;
        private readonly ulong _lo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TraceId(ulong hi, ulong lo)
        {
            _hi = hi;
            _lo = lo;
        }

        /// <summary>
        /// Creates a new random <see cref="TraceId"/> instance.
        /// </summary>
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
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _hi == 0UL && _lo == 0UL;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(TraceId other) => _hi == other._hi && _lo == other._lo;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is TraceId other && Equals(other);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine(_hi, _lo);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(TraceId a, TraceId b) => a.Equals(b);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(TraceId a, TraceId b) => !a.Equals(b);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => IsEmpty ? "TraceId.Empty" : $"TraceId(hi={_hi},lo={_lo})";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ToString();
        }
    }
}
