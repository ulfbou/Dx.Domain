// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Dx.Domain.Errors;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dx.Domain.Facts;

/// <summary>
/// Represents the outcome of a state transition and the domain facts it emitted.
/// </summary>
/// <typeparam name="TState">The type of the resulting state.</typeparam>
/// <remarks>
/// <para>
/// This type is typically returned from aggregate state transition methods:
/// <code>
/// var result = TransitionResult&lt;OrderState&gt;.Success(newState, facts);
/// if (result.IsSuccess) { /* persist newState and facts */ }
/// </code>
/// </para>
/// <para>
/// TransitionResult combines a <see cref="Result{T}"/> (from Kernel) with structural facts.
/// </para>
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct TransitionResult<TState>
    where TState : notnull
{
    /// <summary>
    /// Gets the result of the transition.
    /// </summary>
    public Result<TState> Outcome { get; }

    /// <summary>
    /// Gets the domain facts emitted by the transition.
    /// </summary>
    public IReadOnlyList<IDomainFact> Facts { get; }

    /// <summary>
    /// Gets a value indicating whether the transition was successful.
    /// </summary>
    public bool IsSuccess => Outcome.IsSuccess;

    /// <summary>
    /// Gets a value indicating whether the transition failed.
    /// </summary>
    public bool IsFailure => Outcome.IsFailure;

    private TransitionResult(
        Result<TState> outcome,
        IReadOnlyList<IDomainFact> facts)
    {
        Outcome = outcome;
        Facts = facts ?? Array.Empty<IDomainFact>();
    }

    /// <summary>
    /// Creates a successful transition result with the specified state and emitted facts.
    /// </summary>
    /// <param name="state">The resulting state after the transition.</param>
    /// <param name="facts">The domain facts emitted by the transition.</param>
    /// <returns>A <see cref="TransitionResult{TState}"/> representing a successful transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Factory helpers are intentional on TransitionResult.")]
    public static TransitionResult<TState> Success(TState state, IReadOnlyList<IDomainFact> facts)
    {
        ArgumentNullException.ThrowIfNull(state);

        return new TransitionResult<TState>(
            Result<TState>.Success(state),
            facts ?? Array.Empty<IDomainFact>());
    }

    /// <summary>
    /// Creates a successful transition result with the specified state and a single fact.
    /// </summary>
    /// <param name="state">The resulting state after the transition.</param>
    /// <param name="fact">The single domain fact emitted by the transition.</param>
    /// <returns>A <see cref="TransitionResult{TState}"/> representing a successful transition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Factory helpers are intentional on TransitionResult.")]
    public static TransitionResult<TState> Success(TState state, IDomainFact fact)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(fact);

        return new TransitionResult<TState>(
            Result<TState>.Success(state),
            new[] { fact });
    }

    /// <summary>
    /// Creates a failed transition result from the specified domain error.
    /// </summary>
    /// <param name="error">The error that caused the transition to fail.</param>
    /// <returns>
    /// A <see cref="TransitionResult{TState}"/> whose <see cref="Outcome"/> is a failed result
    /// and whose <see cref="Facts"/> collection is empty.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Factory helpers are intentional on TransitionResult.")]
    public static TransitionResult<TState> Failure(DomainError error)
    {
        if (error.Equals(default))
            throw new ArgumentException("Error must be initialized.", nameof(error));

        return new TransitionResult<TState>(
            Result<TState>.Failure(error),
            Array.Empty<IDomainFact>());
    }

    /// <summary>
    /// Deconstructs the transition result into its success status, state, facts, and error information.
    /// </summary>
    /// <param name="isSuccess">When this method returns, contains <see langword="true"/> if the transition succeeded; otherwise, <see langword="false"/>.</param>
    /// <param name="state">When this method returns, contains the state if the transition succeeded; otherwise, the default value for the type.</param>
    /// <param name="facts">When this method returns, contains the facts emitted by the transition.</param>
    /// <param name="error">When this method returns, contains the error if the transition failed; otherwise, the default value.</param>
    public void Deconstruct(out bool isSuccess, out TState? state, out IReadOnlyList<IDomainFact> facts, out DomainError? error)
    {
        isSuccess = IsSuccess;
        state = Outcome.IsSuccess ? Outcome.Value : default;
        facts = Facts;
        error = Outcome.IsFailure ? Outcome.Error : default;
    }

    /// <summary>
    /// Deconstructs the transition result into its failure status, error, state, and facts.
    /// </summary>
    /// <param name="isFailure">When this method returns, contains <see langword="true"/> if the transition failed; otherwise, <see langword="false"/>.</param>
    /// <param name="error">When this method returns, contains the error if the transition failed; otherwise, the default value.</param>
    /// <param name="state">When this method returns, contains the state if the transition succeeded; otherwise, the default value for the type.</param>
    /// <param name="facts">When this method returns, contains the facts emitted by the transition.</param>
    public void Deconstruct(out bool isFailure, out DomainError? error, out TState? state, out IReadOnlyList<IDomainFact> facts)
    {
        isFailure = IsFailure;
        error = Outcome.IsFailure ? Outcome.Error : default;
        state = Outcome.IsSuccess ? Outcome.Value : default;
        facts = Facts;
    }

    /// <summary>
    /// Deconstructs the transition result into its state and facts.
    /// </summary>
    /// <param name="state">When this method returns, contains the state if the transition succeeded; otherwise, the default value for the type.</param>
    /// <param name="facts">When this method returns, contains the facts emitted by the transition.</param>
    public void Deconstruct(out TState? state, out IReadOnlyList<IDomainFact> facts)
    {
        state = Outcome.IsSuccess ? Outcome.Value : default;
        facts = Facts;
    }

    /// <summary>
    /// Deconstructs the transition result into its error component, if the transition failed.
    /// </summary>
    /// <param name="error">When this method returns, contains the error if the transition failed; otherwise, the default value.</param>
    public void Deconstruct(out DomainError? error)
    {
        error = Outcome.IsFailure ? Outcome.Error : default;
    }

    private string DebuggerDisplay => IsSuccess
        ? $"Success: State = {Outcome.Value}, Facts.Count = {Facts.Count}"
        : $"Failure: Error = {Outcome.Error}";
}
