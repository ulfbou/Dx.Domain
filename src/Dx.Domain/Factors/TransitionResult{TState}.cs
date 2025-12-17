// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="TransitionResult{TState}.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Diagnostics;
using static Dx.Dx;

namespace Dx.Domain.Factors
{
    /// <summary>
    /// Represents the outcome of a state transition and the domain facts it emitted.
    /// </summary>
    /// <typeparam name="TState">The type of the resulting state.</typeparam>
    /// <remarks>
    /// This type is typically returned from aggregate state transition methods:
    /// <code>
    /// var result = TransitionResult&lt;OrderState&gt;.Success(newState, facts);
    /// if (result.IsSuccess) { /* persist newState and facts */ }
    /// </code>
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct TransitionResult<TState>
        where TState : notnull
    {
        /// <summary>Gets the result of the transition.</summary>
        public Result<TState> Outcome { get; }

        /// <summary>Gets the domain facts emitted by the transition.</summary>
        public IReadOnlyList<IDomainFact> Facts { get; }

        /// <summary>Gets a value indicating whether the transition was successful.</summary>
        public bool IsSuccess => Outcome.IsSuccess;

        /// <summary>Gets a value indicating whether the transition failed.</summary>
        public bool IsFailure => Outcome.IsFailure;

        private TransitionResult(
            Result<TState> outcome,
            IReadOnlyList<IDomainFact> facts)
        {
            Outcome = outcome;
            Facts = facts;
        }

        /// <summary>
        /// Creates a successful transition result with the specified state and emitted facts.
        /// </summary>
        /// <param name="state">The resulting state after the transition.</param>
        /// <param name="facts">The domain facts emitted by the transition. Must contain at least one fact.</param>
        /// <returns>A <see cref="TransitionResult{TState}"/> representing a successful transition.</returns>
        /// <exception cref="InvariantViolationException">
        /// Thrown if <paramref name="facts"/> is empty.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TransitionResult<TState> Success(
            TState state,
            IReadOnlyList<IDomainFact> facts)
            => new TransitionResult<TState>(Result.Ok(state), facts);

        /// <summary>
        /// Creates a failed transition result from the specified domain error.
        /// </summary>
        /// <param name="error">The error that caused the transition to fail.</param>
        /// <returns>
        /// A <see cref="TransitionResult{TState}"/> whose <see cref="Outcome"/> is a failed result and whose
        /// <see cref="Facts"/> collection is empty.
        /// </returns>
        internal static TransitionResult<TState> Failure(DomainError error)
            => new TransitionResult<TState>(
                Result.Failure<TState>(error),
                Array.Empty<IDomainFact>());

        private string DebuggerDisplay => IsSuccess
                ? $"Success: State = {Outcome.Value}, Facts.Count = {Facts.Count}"
                : $"Failure: Error = {Outcome.Error}";
    }
}
