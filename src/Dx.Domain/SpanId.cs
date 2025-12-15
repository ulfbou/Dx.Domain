// File: Dx.Domain/SpanId.cs
using System.Diagnostics;

namespace Dx.Domain
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct SpanId : IEquatable<SpanId>
    {
        public static readonly SpanId Empty = new(0UL);

        private readonly ulong _value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanId(ulong value) => _value = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanId New()
        {
            Span<byte> buffer = stackalloc byte[8];
            Random.Shared.NextBytes(buffer);
            return new SpanId(BitConverter.ToUInt64(buffer));
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value == 0UL;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SpanId other) => _value == other._value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is SpanId other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SpanId a, SpanId b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SpanId a, SpanId b) => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => IsEmpty ? "SpanId.Empty" : $"SpanId(v={_value})";

        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ToString();
        }
    }
}
