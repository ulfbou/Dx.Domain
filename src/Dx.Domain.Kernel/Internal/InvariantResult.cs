using Dx.Domain.Internal.Diagnostics;

namespace Dx.Domain.Internal
{
    internal readonly struct InvariantResult
    {
        public bool Valid { get; }
        public InvariantDiagnostic Diagnostic { get; }

        private InvariantResult(bool valid, InvariantDiagnostic diagnostic)
        {
            Valid = valid;
            Diagnostic = diagnostic;
        }

        public static InvariantResult Success() => new InvariantResult(true, null);
        public static InvariantResult Fail(InvariantDiagnostic d) => new InvariantResult(false, d);
    }
}
