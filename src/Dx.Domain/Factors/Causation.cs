namespace Dx.Domain.Factors
{
    using Dx.Domain.Invariants;
    using System.Diagnostics;

    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
    public readonly struct Causation : IEquatable<Causation>
    {
        public CorrelationId CorrelationId { get; }
        public TraceId TraceId { get; }
        public string? ActorId { get; }
        public DateTime UtcTimestamp { get; }

        private Causation(CorrelationId correlationId, TraceId traceId, string? actorId, DateTime utcTimestamp)
        {
            CorrelationId = correlationId;
            TraceId = traceId;
            ActorId = actorId;
            UtcTimestamp = utcTimestamp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Causation Create(CorrelationId correlationId, TraceId traceId, string? actorId = null)
        {
            Invariant.That(!correlationId.IsEmpty, DomainErrors.Causation.MissingCorrelation);
            Invariant.That(!traceId.IsEmpty, DomainErrors.Causation.MissingTrace);
            return new Causation(correlationId, traceId, actorId, DateTime.UtcNow);
        }

        public bool Equals(Causation other)
            => CorrelationId == other.CorrelationId &&
               TraceId == other.TraceId &&
               ActorId == other.ActorId &&
               UtcTimestamp == other.UtcTimestamp;

        public override bool Equals(object? obj) => obj is Causation && Equals((Causation)obj);

        public static bool operator ==(Causation left, Causation right) => left.Equals(right);

        public static bool operator !=(Causation left, Causation right) => !(left == right);

        public override int GetHashCode()
            => HashCode.Combine(CorrelationId, TraceId, ActorId, UtcTimestamp);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => $"Causation {{ CorrelationId = {CorrelationId}, TraceId = {TraceId}, ActorId = {ActorId}, UtcTimestamp = {UtcTimestamp:u} }}";
        }
    }
}
