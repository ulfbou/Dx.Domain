namespace Dx.Domain.Factors
{
    using Dx.Domain.Invariants;
    using System.Diagnostics;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct Fact<T> : IDomainFact where T : notnull
    {
        public FactId Id { get; }
        public string FactType { get; }
        public T Payload { get; }
        public Causation Causation { get; }
        public DateTimeOffset UtcTimestamp { get; }

        private Fact(FactId id, string factType, T payload, Causation causation, DateTimeOffset utcTimestamp)
        {
            Id = id;
            FactType = factType;
            Payload = payload;
            Causation = causation;
            UtcTimestamp = utcTimestamp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
        public static Fact<T> Create(string factType, T payload, Causation causation)
        {
            Invariant.That(!string.IsNullOrWhiteSpace(factType), DomainErrors.Fact.MissingType);
            return new Fact<T>(FactId.New(), factType, payload, causation, DateTimeOffset.UtcNow);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => $"Fact<{typeof(T).Name}> {{ Id = {Id}, FactType = {FactType}, UtcTimestamp = {UtcTimestamp:u} }}";
        }
    }
}
