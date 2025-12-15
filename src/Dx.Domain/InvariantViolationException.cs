namespace Dx.Domain
{
    using System.Diagnostics;

    [DebuggerDisplay("{Message,nq}")]
    public sealed class InvariantViolationException : Exception
    {
        public InvariantError Diagnostic { get; }

        public override string Message => Diagnostic.EffectiveMessage;

        public InvariantViolationException(InvariantError diagnostic)
            : base(diagnostic.EffectiveMessage)
        {
            Diagnostic = diagnostic;
        }
    }
}
