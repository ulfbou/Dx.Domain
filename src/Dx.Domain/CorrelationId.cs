namespace Dx.Domain
{
    using System.Diagnostics;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct CorrelationId : IEquatable<CorrelationId>
    {
        public static readonly CorrelationId Empty = new(Guid.Empty);

        public Guid Value { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CorrelationId(Guid value) => Value = value;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value == Guid.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFormat(Span<char> destination, out int charsWritten)
            => Value.TryFormat(destination, out charsWritten, "N");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => Value.ToString("N");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(CorrelationId other) => Value.Equals(other.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is CorrelationId other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(CorrelationId a, CorrelationId b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(CorrelationId a, CorrelationId b) => !a.Equals(b);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsEmpty ? "CorrelationId.Empty" : ToString();
        }
    }
}
