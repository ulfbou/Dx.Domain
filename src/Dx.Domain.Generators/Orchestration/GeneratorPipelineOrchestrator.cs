// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="GeneratorPipelineOrchestrator.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain;
using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Core;
using Dx.Domain.Generators.Diagnostics;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using static Dx.Dx;

namespace Dx.Domain.Generators.Orchestration
{
    public sealed partial class GeneratorPipelineOrchestrator
    {
        private readonly MonotonicFactStore _store;

        public GeneratorPipelineOrchestrator(MonotonicFactStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<Result<StageSuccessPayload, StageFailurePayload>>
                ExecuteStageAsync(
                    IGeneratorStage stage,
                    CancellationToken cancellationToken)
        {
            var transaction = new StageTransaction(_store);

            var context = new StageContext(transaction);

            var result = await stage.ExecuteAsync(context, cancellationToken);

            if (result.IsFailure)
                return result;

            var commit = _store.AtomicCommit(
                stage.StageName,
                transaction.Snapshot(),
                context.Causation);

            if (commit.IsFailure)
            {
                return Result.Failure<StageSuccessPayload, StageFailurePayload>(
                    new StageFailurePayload(
                        Diagnostics.FailureClass.InvariantViolation,
                        new GeneratorDiagnostic(
                            "Monotonic commit failed."),
                        commit.Error.ToResolutionRequest()));
            }

            return result;
        }

        /// <summary>
        /// Executes a single generator stage and translates its outcome into a canonical Result.
        /// This method never throws for domain, policy, or semantic failures.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown when orchestration contracts are violated.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a stage violates execution invariants (e.g. returns null).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Propagated to preserve cooperative cancellation semantics.
        /// </exception>
        private static async Task<Result<StageSuccessPayload, StageFailurePayload>> ExecuteStageInternalAsync(
            IGeneratorStage stage,
            StageContext context,
            CancellationToken cancellationToken)
        {
            // ------------------------------------------------------------
            // Hard contract validation (programmer errors only)
            // ------------------------------------------------------------
            ArgumentNullException.ThrowIfNull(stage);
            ArgumentNullException.ThrowIfNull(context);

            cancellationToken.ThrowIfCancellationRequested();

            // ------------------------------------------------------------
            // Execute stage
            // ------------------------------------------------------------
            Result<StageSuccessPayload, StageFailurePayload> stageResult;

            try
            {
                stageResult = await stage
                    .ExecuteAsync(context, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is not a failure
                throw;
            }
            catch (Exception ex)
            {
                // Any other exception is a stage bug, not a domain outcome
                throw new InvalidOperationException(
                    $"Stage '{stage.StageName}' violated execution rules by throwing an exception. " +
                    "Stages must return semantic outcomes via Result<StageSuccessPayload, StageFailurePayload>.",
                    ex);
            }

            // Result is a struct; null is not a valid value, but callers might default it.
            // Enforce that a stage always returns a meaningful outcome.
            if (!stageResult.IsSuccess && stageResult.Error is null)
            {
                throw new InvalidOperationException(
                    $"Stage '{stage.StageName}' reported failure without failure details.");
            }

            return stageResult;
        }

        // ------------------------------------------------------------
        // Failure normalization (legacy helper retained for compatibility)
        // ------------------------------------------------------------

        private static DomainError CreateDomainError(
            IGeneratorStage stage,
            StageFailurePayload failure)
        {
            if (failure is null)
            {
                throw new InvalidOperationException(
                    $"Stage '{stage.StageName}' reported failure without failure details.");
            }

            // Map generator diagnostic into a DomainError using the public GeneratorFaults facade.
            return Dx.Generator.FromDiagnostic(failure.Diagnostic);
        }
    }
}
