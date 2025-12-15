// File: Dx.Domain/TraceId.cs
using System.Diagnostics;

namespace Dx.Domain
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct TraceId : IEquatable<TraceId>
    {
        public static readonly TraceId Empty = new(0UL, 0UL);

        private readonly ulong _hi;
        private readonly ulong _lo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TraceId(ulong hi, ulong lo)
        {
            _hi = hi;
            _lo = lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TraceId FromBytes(ReadOnlySpan<byte> bytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(bytes.Length, 16, nameof(bytes));

            ulong hi = 0, lo = 0;

            for (int i = 0; i < 8; i++)
                hi = (hi << 8) | bytes[i];

            for (int i = 8; i < 16; i++)
                lo = (lo << 8) | bytes[i];

            return new TraceId(hi, lo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToBytes(Span<byte> destination)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(destination.Length, 16, nameof(destination));

            ulong v = _hi;
            for (int i = 7; i >= 0; i--)
            { destination[i] = (byte)(v & 0xFF); v >>= 8; }

            v = _lo;
            for (int i = 15; i >= 8; i--)
            { destination[i] = (byte)(v & 0xFF); v >>= 8; }
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _hi == 0UL && _lo == 0UL;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(TraceId other) => _hi == other._hi && _lo == other._lo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is TraceId other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine(_hi, _lo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(TraceId a, TraceId b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(TraceId a, TraceId b) => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => IsEmpty ? "TraceId.Empty" : $"TraceId(hi={_hi},lo={_lo})";

        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ToString();
        }
    }
}
