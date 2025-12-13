using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Diagnostics;

namespace Zentient.Metadata.Diagnostics.Context
{
    public interface IMetadataDiagnosticContext : IDiagnosticContext
    {
        IMetadata Metadata { get; }
    }
}
