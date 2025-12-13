using System.Collections.Generic;
using Zentient.Abstractions.Diagnostics;
using Zentient.Abstractions.Metadata.Diagnostics;

namespace Zentient.Metadata.Diagnostics.Profiles
{
    public interface IDiagnosticProfileRegistry
    {
        void Register(DiagnosticProfileKey key, IEnumerable<IDiagnosticCheckDefinition> checks);
        IReadOnlyCollection<IDiagnosticCheckDefinition> Resolve(DiagnosticProfileKey key);
        IEnumerable<DiagnosticProfileKey> AvailableProfiles { get; }
    }
}
