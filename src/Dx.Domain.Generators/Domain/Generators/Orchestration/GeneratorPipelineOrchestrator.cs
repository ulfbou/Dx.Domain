using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dx.Domain;
using Dx.Domain.Generators.Core;
using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Diagnostics;
using Dx.Domain.Generators.Internal;

namespace Dx.Domain.Generators.Orchestration
{
    public sealed partial class GeneratorPipelineOrchestrator
    {
        private readonly MonotonicFactStore _store;
        private readonly InputFingerprint _fingerprint;
        private readonly IReadOnlyDictionary<string, object> _manifest;
        private readonly IReadOnlyPolicy _policy;
        private readonly IClock _clock;
        private readonly IDeterministicIdentity _identity;

        public GeneratorPipelineOrchestrator(
            MonotonicFactStore store,
            InputFingerprint fingerprint,
            IReadOnlyDictionary<string, object> manifest,
            IReadOnlyPolicy policy,
            IClock clock,
            IDeterministicIdentity identity)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _fingerprint = fingerprint ?? throw new ArgumentNullException(nameof(fingerprint));
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
        }

        public async Task<Result<StageSuccessPayload, StageFailurePayload>> ExecuteStageAsync(
            IGeneratorStage stage,
            CancellationToken ct)
        {
            // 1. Pre-Flight Assertion Check
            var assertionResult = stage.Assertions.Validate(_store);
            if (assertionResult.IsFailure)
            {
                var diagnostic = new GeneratorDiagnostic(
                    id: "DXG.PreFlight",
                    @class: FailureClass.IntentViolation,
                    title: "Orchestrator",
                    message: assertionResult.Error.Message,
                    inputFingerprint: _fingerprint,
                    stage: stage.StageName,
                    location: null,
                    remediationOptions: Enumerable.Empty<Remediation>(),
                    fixPreview: null,
                    impact: ImpactLevel.Breaking
                );

                return Result<StageSuccessPayload, StageFailurePayload>.InternalFailure(
                    new StageFailurePayload(
                        FailureClass.IntentViolation,
                        diagnostic,
                        null));
            }

            // 2. Build Context
            // FIX CS1061: MonotonicFactStore mapping to IReadOnlyFactSet
            // Based on generators.cs, we wrap the store or use a projection
            var context = new StageContext(
                _fingerprint,
                _manifest,
                _policy,
                new ReadOnlyFactStoreProjection(_store),
                _clock,
                _identity);

            try
            {
                using var transaction = new StageTransaction(_store);
                var result = await stage.ExecuteAsync(context, ct).ConfigureAwait(false);

                if (result.IsFailure)
                    return result;

                // 3. Monotonic Commit
                // Using the specific Dx identity factories provided in your prompt
                var causation = Dx.CausationFactory.Create(
                    correlationId: Dx.Correlation.New(),
                    traceId: Dx.Trace.New(),
                    actorId: ActorId.Empty);

                var commit = _store.AtomicCommit(
                    stage.StageName,
                    transaction.Snapshot(),
                    causation);

                if (commit.IsFailure)
                {
                    // FIX CS1061: Aggregate conflict messages from CommitFailure
                    var failureMessage = string.Join("; ", commit.Error.Conflicts.Select(c => c.ToString()));

                    return Result<StageSuccessPayload, StageFailurePayload>.InternalFailure(
                        new StageFailurePayload(
                            FailureClass.InternalError,
                            new GeneratorDiagnostic(
                                "DXG.Commit",
                                FailureClass.InternalError,
                                failureMessage,
                                stage.StageName,
                                _fingerprint,
                                stage.StageName,
                                null, Enumerable.Empty<Remediation>(), null, ImpactLevel.Blocker),
                            null));
                }

                return result;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Result<StageSuccessPayload, StageFailurePayload>.InternalFailure(
                    new StageFailurePayload(
                        FailureClass.InternalError,
                        new GeneratorDiagnostic(
                            "DXG.Crash",
                            FailureClass.InternalError,
                            ex.Message,
                            stage.StageName,
                            _fingerprint,
                            stage.StageName,
                            null, Enumerable.Empty<Remediation>(), ex.StackTrace, ImpactLevel.Blocker),
                        null));
            }
        }

        /// <summary>
        /// Bridge to provide IReadOnlyFactSet from the MonotonicFactStore.
        /// </summary>
        private sealed class ReadOnlyFactStoreProjection : IReadOnlyFactSet
        {
            private readonly MonotonicFactStore _store;
            public ReadOnlyFactStoreProjection(MonotonicFactStore store) => _store = store;

            public bool ContainsKey(string key) => _store.TryGet(key, out _);
            public bool TryGetValue(string key, out object? value)
            {
                if (_store.TryGet(key, out var fact))
                {
                    value = fact!.GetPayload();
                    return true;
                }
                value = null;
                return false;
            }

            // Note: If MonotonicFactStore doesn't expose all keys, 
            // this property requires internal access or a store update.
            public IReadOnlyDictionary<string, object> All => throw new NotSupportedException("Full iteration not supported by store.");
        }
    }
}
