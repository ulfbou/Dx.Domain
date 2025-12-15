namespace Dx.Domain.Factors
{
    using Dx.Domain.Invariants;

    public readonly struct TransitionResult<TState>
        where TState : notnull
    {
        public Result<TState> Outcome { get; }
        public IReadOnlyList<IDomainFact> Facts { get; }

        public bool IsSuccess => Outcome.IsSuccess;

        private TransitionResult(
            Result<TState> outcome,
            IReadOnlyList<IDomainFact> facts)
        {
            Outcome = outcome;
            Facts = facts;
        }

        internal static TransitionResult<TState> Success(
            TState state,
            IReadOnlyList<IDomainFact> facts)
        {
            Invariant.That(facts.Count > 0, DomainErrors.Transition.MissingFacts);

            return new TransitionResult<TState>(Result.Ok(state), facts);
        }

        internal static TransitionResult<TState> Failure(DomainError error)
            => new TransitionResult<TState>(
             Result.Failure<TState>(error),
             Array.Empty<IDomainFact>());

    }
}
