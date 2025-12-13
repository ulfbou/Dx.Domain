using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Zentient.Abstractions.Metadata.Diagnostics;

namespace Zentient.Metadata.Diagnostics.Profiles
{
    public sealed class DiagnosticProfileRegistry : IDiagnosticProfileRegistry
    {
        private readonly ConcurrentDictionary<DiagnosticProfileKey, HashSet<IDiagnosticCheckDefinition>> _profiles = new();

        public void Register(DiagnosticProfileKey key, IEnumerable<IDiagnosticCheckDefinition> checks)
        {
            var set = _profiles.GetOrAdd(key, _ => new());
            foreach (var check in checks)
                set.Add(check);
        }

        public IReadOnlyCollection<IDiagnosticCheckDefinition> Resolve(DiagnosticProfileKey key)
            => _profiles.TryGetValue(key, out var set) ? set : new HashSet<IDiagnosticCheckDefinition>();

        public IEnumerable<DiagnosticProfileKey> AvailableProfiles => _profiles.Keys;
    }
}
