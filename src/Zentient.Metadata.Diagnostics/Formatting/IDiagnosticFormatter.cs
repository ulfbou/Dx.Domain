using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Diagnostics;

namespace Zentient.Metadata.Diagnostics.Formatting
{
    public interface IDiagnosticFormatter
    {
        string ToText<TCodeDefinition, TErrorDefinition>(IDiagnosticReport<TCodeDefinition, TErrorDefinition> report)
            where TCodeDefinition : ICodeDefinition
            where TErrorDefinition : IErrorDefinition;

        string ToJson<TCodeDefinition, TErrorDefinition>(IDiagnosticReport<TCodeDefinition, TErrorDefinition> report)
            where TCodeDefinition : ICodeDefinition
            where TErrorDefinition : IErrorDefinition;
    }
}
