using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Diagnostics;
using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Errors.Definitions;

namespace Zentient.Metadata.Diagnostics.Reports
{
    public interface IMetadataDiagnosticReport<TCodeDefinition, TErrorDefinition> : IDiagnosticReport<TCodeDefinition, TErrorDefinition>
        where TCodeDefinition : ICodeDefinition
        where TErrorDefinition : IErrorDefinition
    {
        Zentient.Abstractions.Metadata.IMetadata Metadata { get; }
        Zentient.Metadata.Diagnostics.Profiles.DiagnosticProfileKey? PresetApplied { get; }
    }
}
