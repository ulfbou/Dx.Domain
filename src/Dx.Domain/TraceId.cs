namespace Dx.Domain
{
    using System.Diagnostics;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct TraceId : IEquatable<TraceId>
    {
        public static readonly TraceId Empty = new(0UL, 0UL);

        private readonly ulong _hi;
        private readonly ulong _lo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TraceId(ulong hi, ulong lo)
        {
            _hi = hi;
            _lo = lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TraceId New()
        {
            Span<byte> buffer = stackalloc byte[16];
            Random.Shared.NextBytes(buffer);
            ulong hi = BitConverter.ToUInt64(buffer.Slice(0, 8));
            ulong lo = BitConverter.ToUInt64(buffer.Slice(8, 8));
            return new TraceId(hi, lo);
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
