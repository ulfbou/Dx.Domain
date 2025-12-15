using System.Diagnostics;

namespace Dx.Domain.Factors
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct FactId : IEquatable<FactId>
    {
        public Guid Value { get; }
        private FactId(Guid value) => Value = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FactId New() => new(Guid.NewGuid());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FactId From(Guid value) => new(value);

        public bool Equals(FactId other) => Value.Equals(other.Value);

        public override bool Equals(object? obj) => obj is FactId && Equals((FactId)obj);

        public static bool operator ==(FactId left, FactId right) => left.Equals(right);

        public static bool operator !=(FactId left, FactId right) => !(left == right);

        public static implicit operator FactId(Guid value) => From(value);

        public static explicit operator Guid(FactId factId) => factId.Value;

        public override int GetHashCode() => Value.GetHashCode();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => $"FactId {{ Value = {Value:N} }}";
        }
    }
}
