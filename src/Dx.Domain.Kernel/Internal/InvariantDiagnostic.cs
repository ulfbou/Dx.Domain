namespace Dx.Domain.Internal
{
    internal sealed class InvariantDiagnostic
    {
        public string RuleId { get; }
        public string Message { get; }

        public InvariantDiagnostic(string ruleId, string message)
        {
            RuleId = ruleId;
            Message = message;
        }
    }
}
