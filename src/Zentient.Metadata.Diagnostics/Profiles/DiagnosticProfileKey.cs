using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Diagnostics;

namespace Zentient.Metadata.Diagnostics.Profiles
{
    public sealed record DiagnosticProfileKey(string Name) : IPresetKey;
}
