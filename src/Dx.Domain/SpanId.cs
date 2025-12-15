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
        public static SpanId FromBytes(ReadOnlySpan<byte> bytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(bytes.Length, 8, nameof(bytes));

            ulong v = 0;
            for (int i = 0; i < 8; i++)
                v = (v << 8) | bytes[i];
            return new SpanId(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToBytes(Span<byte> destination)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(destination.Length, 8, nameof(destination));

            ulong v = _value;
            for (int i = 7; i >= 0; i--)
            {
                destination[i] = (byte)(v & 0xFF);
                v >>= 8;
            }
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
