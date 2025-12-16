namespace Dx.Domain
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a span identifier used to correlate work within a single trace.
    /// </summary>
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
