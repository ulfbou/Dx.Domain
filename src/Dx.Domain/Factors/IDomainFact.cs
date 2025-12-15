namespace Dx.Domain.Factors
{
    public interface IDomainFact
    {
        FactId Id { get; }
        string FactType { get; }
        Causation Causation { get; }
        DateTimeOffset UtcTimestamp { get; }
    }
}
