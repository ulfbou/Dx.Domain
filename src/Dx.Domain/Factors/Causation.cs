namespace Dx.Domain.Factors
{
    using Dx.Domain.Invariants;

    using System.Diagnostics;

    /// <summary>
    /// Captures correlation, trace, actor, and timestamp information that explains why a fact or operation occurred.
    /// </summary>
    /// <remarks>
    /// Typical usage when emitting a fact:
    /// <code>
    /// var causation = Causation.Create(correlationId, traceId, actorId);
    /// var fact = Fact.Create("OrderPlaced", payload, causation);
    /// </code>
    /// </remarks>
    public readonly struct Causation : IEquatable<Causation>
    {
        /// <summary>Gets the correlation identifier that groups related operations.</summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>Gets the trace identifier used for distributed tracing.</summary>
        public TraceId TraceId { get; }

        /// <summary>Gets the actor responsible for the action, if known.</summary>
        public ActorId ActorId { get; }

        /// <summary>Gets the UTC timestamp when the causation was recorded.</summary>
        public DateTime UtcTimestamp { get; }

        private Causation(CorrelationId correlationId, TraceId traceId, ActorId? actorId, DateTime utcTimestamp)
        {
            CorrelationId = correlationId;
            TraceId = traceId;
            ActorId = actorId ?? ActorId.Empty;
            UtcTimestamp = utcTimestamp;
        }

        /// <summary>
        /// Creates a new <see cref="Causation"/> instance with the specified correlation and trace identifiers and optional actor.
        /// </summary>
        /// <param name="correlationId">The correlation identifier. Must not be empty.</param>
        /// <param name="traceId">The trace identifier. Must not be empty.</param>
        /// <param name="actorId">The optional actor responsible for the action.</param>
        /// <returns>A new <see cref="Causation"/> instance whose <see cref="UtcTimestamp"/> is set to <see cref="DateTime.UtcNow"/>.</returns>
        /// <remarks>
        /// This method enforces the invariant that both <paramref name="correlationId"/> and <paramref name="traceId"/>
        /// are non-empty. If either identifier is empty, an invariant violation is raised.
        /// </remarks>
        /// <exception cref="InvariantViolationException">
        /// Thrown if <paramref name="correlationId"/> or <paramref name="traceId"/> is empty.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Causation Create(CorrelationId correlationId, TraceId traceId, ActorId? actorId = null)
        {
            Invariant.That(!correlationId.IsEmpty, DomainErrors.Causation.MissingCorrelation);
            Invariant.That(!traceId.IsEmpty, DomainErrors.Causation.MissingTrace);
            return new Causation(correlationId, traceId, actorId, DateTime.UtcNow);
        }

        /// <inheritdoc />
        public bool Equals(Causation other)
            => CorrelationId == other.CorrelationId &&
               TraceId == other.TraceId &&
               ActorId == other.ActorId &&
               UtcTimestamp == other.UtcTimestamp;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Causation other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(CorrelationId, TraceId, ActorId, UtcTimestamp);

        /// <summary>
        /// Determines whether two <see cref="Causation"/> values are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true"/> if the values are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(Causation left, Causation right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Causation"/> values are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(Causation left, Causation right) => !left.Equals(right);
    }
}
