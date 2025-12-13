using System.Diagnostics;
using System.Collections.Generic;
using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Diagnostics;

namespace Zentient.Metadata.Diagnostics.Debugging
{
    [DebuggerDisplay("{Status} | Errors = {Errors.Count} | Duration = {CheckDuration}")]
    public abstract class DebuggerDisplayDiagnosticResult : IDiagnosticResult
    {
        public abstract string Status { get; }
        public abstract IReadOnlyCollection<string> Errors { get; }
        public abstract double CheckDuration { get; }
    }

    [DebuggerDisplay("{Status} | Errors = {Errors.Count} | Duration = {CheckDuration}")]
    public abstract class DebuggerDisplayDiagnosticReport<TCodeDefinition, TErrorDefinition> : IDiagnosticReport<TCodeDefinition, TErrorDefinition>
        where TCodeDefinition : ICodeDefinition
        where TErrorDefinition : IErrorDefinition
    {
        public abstract string Status { get; }
        public abstract IReadOnlyCollection<TErrorDefinition> Errors { get; }
        public abstract double CheckDuration { get; }
    }
}
