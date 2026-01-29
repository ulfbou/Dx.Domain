using Dx.Domain;
using Dx.Domain.Factors;
using Dx.Domain.Primitives;
using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Core;
using Dx.Domain.Generators.Diagnostics;
using Dx.Domain.Generators.Internal;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

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
            // 1. Build Context and Transaction FIRST to fix CS0841/CS0103
            var context = new StageContext(
                _fingerprint,
                _manifest,
                _policy,
                new ReadOnlyFactStoreProjection(_store),
                _clock,
                _identity);

            using var transaction = new StageTransaction(_store);

            // 2. Execute Stage (Renamed 'result' to 'stageResult' to avoid CS0136)
            var stageResult = await stage.ExecuteAsync(context, transaction, ct).ConfigureAwait(false);

            if (stageResult.IsFailure)
            {
                // Accessing the diagnostic property correctly based on generators.cs
                var diagnostic = new GeneratorDiagnostic(
                    id: "DXG.PreFlight",
                    @class: FailureClass.IntentViolation,
                    title: "Orchestrator",
                    message: stageResult.Error.Diagnostic.Message,
                    inputFingerprint: _fingerprint,
                    stageName: stage.StageName,
                    location: null,
                    remediationOptions: Enumerable.Empty<Remediation>(),
                    fixPreview: null,
                    impact: ImpactLevel.Breaking
                );

                return Result<StageSuccessPayload, StageFailurePayload>.Failure(
                    new StageFailurePayload(
                        FailureClass.IntentViolation,
                        diagnostic,
                        null));
            }

            try
            {
                // 3. Monotonic Commit
                var causation = Causation.Create(
                    correlationId: CorrelationId.New(),
                    traceId: TraceId.New(),
                    actorId: null);

                var commit = _store.AtomicCommit(
                    stage.StageName,
                    transaction.Snapshot(),
                    causation);

                if (commit.IsFailure)
                {
                    var failureMessage = string.Join("; ", commit.Error.Conflicts.Select(c => c.ToString()));

                    return Result<StageSuccessPayload, StageFailurePayload>.Failure(
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

                return stageResult;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Result<StageSuccessPayload, StageFailurePayload>.Failure(
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
