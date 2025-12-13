// This file is part of Zentient.Metadata.Abstractions (staging area for future core abstractions).
// Namespace is stable and forward-compatible: Zentient.Abstractions.Metadata.Diagnostics

using Zentient.Abstractions.Common;
using Zentient.Abstractions.Common.Definitions;

namespace Zentient.Abstractions.Metadata.Diagnostics
{
    /// <summary>
    /// Represents a definition for a diagnostic check that can be discovered, profiled, and executed.
    /// </summary>
    public interface IDiagnosticCheckDefinition : ITypeDefinition
    {
        string DisplayName { get; }
        Type TargetComponentType { get; }
        string DiagnosticsCategory { get; }
    }
}
