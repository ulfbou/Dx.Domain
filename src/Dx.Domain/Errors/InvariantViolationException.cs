namespace Dx.Domain
{
    public sealed class InvariantViolationException : Exception
    {
        public InvariantError Diagnostic { get; }

        private InvariantViolationException(InvariantError diagnostic)
            : base(diagnostic.EffectiveMessage)
        {
            Diagnostic = diagnostic;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InvariantViolationException From(InvariantError diagnostic)
            => new InvariantViolationException(diagnostic);
    }
}
