// <summary>
//     <list type="bullet">
//         <item>
//             <term>File:</term>
//             <description>TransitionResult{TState}.cs</description>
//         </item>
//         <item>
//             <term>Project:</term>
//             <description>Dx.Domain</description>
//         </item>
//         <item>
//             <term>Description:</term>
//             <description>
//                 Defines the result type returned from state transitions, bundling the outcome and any
//                 domain facts emitted during the transition.
//             </description>
//         </item>
//     </list>
// </summary>
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

namespace Dx.Domain.Factors
{
    using Dx.Domain.Invariants;

    /// <summary>
    /// Represents the outcome of a state transition along with the domain facts it produced.
    /// </summary>
    /// <typeparam name="TState">The type of the resulting state.</typeparam>
    public readonly struct TransitionResult<TState>
        where TState : notnull
    {
        /// <summary>Gets the result of the transition.</summary>
        public Result<TState> Outcome { get; }

        /// <summary>Gets the domain facts emitted by the transition.</summary>
        public IReadOnlyList<IDomainFact> Facts { get; }

        /// <summary>Gets a value indicating whether the transition was successful.</summary>
        public bool IsSuccess => Outcome.IsSuccess;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TransitionResult(
            Result<TState> outcome,
            IReadOnlyList<IDomainFact> facts)
        {
            Outcome = outcome;
            Facts = facts;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TransitionResult<TState> Success(
            TState state,
            IReadOnlyList<IDomainFact> facts)
        {
            Invariant.That(facts.Count > 0, DomainErrors.Transition.MissingFacts);

            return new TransitionResult<TState>(Result.Ok(state), facts);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TransitionResult<TState> Failure(DomainError error)
            => new TransitionResult<TState>(
                Result.Failure<TState>(error),
                Array.Empty<IDomainFact>());
    }
}
