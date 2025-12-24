// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="MonotonicFactStore.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Factors;

using System.Collections.Concurrent;
using System.Linq;

using static Dx.Dx;

namespace Dx.Domain.Generators.Core
{
    public sealed class MonotonicFactStore
    {
        private readonly ConcurrentDictionary<string, IDomainFact> _store = new();
        private readonly object _lock = new();
        private readonly IFactFactory _factory;
        private readonly ITelemetry _telemetry;

        public MonotonicFactStore(
            IFactFactory? factory = null,
            ITelemetry? telemetry = null)
        {
            _factory = factory ?? new FactFactoryRegistry();
            _telemetry = telemetry ?? new NoopTelemetry();
        }

        public bool TryGet(string key, out IDomainFact? fact)
            => _store.TryGetValue(key, out fact);

        public Result<Unit, CommitFailure> AtomicCommit(
            string stageName,
            IReadOnlyDictionary<string, object> proposals,
            Causation causation)
        {
            _telemetry.TrackCommitAttempt(stageName, proposals.Count);

            var snapshot = proposals
                .OrderBy(p => p.Key, StringComparer.Ordinal)
                .ToArray();

            var conflicts = Check(snapshot);
            if (conflicts.Count > 0)
                return Result.Failure<Unit, CommitFailure>(new(conflicts));

            lock (_lock)
            {
                conflicts = Check(snapshot);
                if (conflicts.Count > 0)
                {
                    return Result.Failure<Unit, CommitFailure>(new(conflicts));
                }

                foreach (var (key, value) in snapshot)
                {
                    if (_store.ContainsKey(key))
                        continue;
                    var fact = _factory.Create(key, value, causation);
                    _store.TryAdd(key, fact);
                }
            }

            _telemetry.TrackCommitSuccess(stageName, snapshot.Length);
            return Result.Ok<Unit, CommitFailure>(Unit.Value);
        }

        private List<CommitConflict> Check(
            KeyValuePair<string, object>[] snapshot)
        {
            var conflicts = new List<CommitConflict>();

            foreach (var (key, proposed) in snapshot)
            {
                if (_store.TryGetValue(key, out var existing))
                {
                    if (!StructuralComparer.StructurallyEqual(
                            existing.GetPayload(), proposed))
                    {
                        conflicts.Add(new CommitConflict(
                            key, existing, proposed));
                    }
                }
            }

            return conflicts;
        }
    }
}
