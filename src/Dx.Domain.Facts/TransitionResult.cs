// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Dx.Domain.Errors;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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
    private readonly ImmutableArray<IDomainFact> _facts;

    /// <summary>
    /// Gets the result of the transition.
    /// </summary>
    public Result<TState> Outcome { get; }

    /// <summary>
    /// Gets the domain facts emitted by the transition.
    /// </summary>
    public IReadOnlyList<IDomainFact> Facts => FactsImmutable;

    /// <summary>
    /// Gets the domain facts emitted by the transition as an immutable array.
    /// </summary>
    public ImmutableArray<IDomainFact> FactsImmutable
        => _facts.IsDefault ? ImmutableArray<IDomainFact>.Empty : _facts;

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
        ImmutableArray<IDomainFact> facts)
    {
        Outcome = outcome;
        _facts = facts.IsDefault ? ImmutableArray<IDomainFact>.Empty : facts;
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
        ArgumentNullException.ThrowIfNull(facts);

        return new TransitionResult<TState>(
            Result<TState>.Success(state),
            NormalizeFacts(facts));
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
        ArgumentNullException.ThrowIfNull(fact);

        return new TransitionResult<TState>(
            Result<TState>.Success(state),
            ImmutableArray.Create(fact));
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
            ImmutableArray<IDomainFact>.Empty);
    }

    /// <summary>
    /// Transforms the successful state while preserving facts.
    /// </summary>
    /// <typeparam name="TNext">The type of the resulting state.</typeparam>
    /// <param name="map">The mapping function to apply to the state.</param>
    /// <returns>A new transition result with the mapped state and preserved facts.</returns>
    public TransitionResult<TNext> Map<TNext>(Func<TState, TNext> map)
        where TNext : notnull
    {
        ArgumentNullException.ThrowIfNull(map);

        if (IsFailure)
            return TransitionResult<TNext>.Failure(Outcome.Error);

        return new TransitionResult<TNext>(
            Result<TNext>.Success(map(Outcome.Value)),
            _facts);
    }

    /// <summary>
    /// Composes transitions, concatenating facts on success.
    /// </summary>
    /// <typeparam name="TNext">The type of the resulting state.</typeparam>
    /// <param name="bind">The transition function to apply to the state.</param>
    /// <returns>A composed transition result.</returns>
    public TransitionResult<TNext> Bind<TNext>(Func<TState, TransitionResult<TNext>> bind)
        where TNext : notnull
    {
        ArgumentNullException.ThrowIfNull(bind);

        if (IsFailure)
            return TransitionResult<TNext>.Failure(Outcome.Error);

        var next = bind(Outcome.Value);
        if (next.IsFailure)
            return TransitionResult<TNext>.Failure(next.Outcome.Error);

        var combinedFacts = ConcatFacts(_facts, next._facts);
        return new TransitionResult<TNext>(
            Result<TNext>.Success(next.Outcome.Value),
            combinedFacts);
    }

    /// <summary>
    /// LINQ projection over a successful state.
    /// </summary>
    /// <typeparam name="TNext">The type of the resulting state.</typeparam>
    /// <param name="selector">The projection to apply.</param>
    /// <returns>A new transition result with the projected state and preserved facts.</returns>
    public TransitionResult<TNext> Select<TNext>(Func<TState, TNext> selector)
        where TNext : notnull
        => Map(selector);

    /// <summary>
    /// LINQ composition over transitions with fact accumulation.
    /// </summary>
    /// <typeparam name="TNext">The type produced by the bind step.</typeparam>
    /// <typeparam name="TResult">The type produced by the projection.</typeparam>
    /// <param name="bind">The bind function.</param>
    /// <param name="project">The projection combining the original and bound state.</param>
    /// <returns>A composed transition result with accumulated facts.</returns>
    public TransitionResult<TResult> SelectMany<TNext, TResult>(
        Func<TState, TransitionResult<TNext>> bind,
        Func<TState, TNext, TResult> project)
        where TNext : notnull
        where TResult : notnull
    {
        ArgumentNullException.ThrowIfNull(bind);
        ArgumentNullException.ThrowIfNull(project);

        if (IsFailure)
            return TransitionResult<TResult>.Failure(Outcome.Error);

        var next = bind(Outcome.Value);
        if (next.IsFailure)
            return TransitionResult<TResult>.Failure(next.Outcome.Error);

        var projected = project(Outcome.Value, next.Outcome.Value);
        var combinedFacts = ConcatFacts(_facts, next._facts);

        return new TransitionResult<TResult>(
            Result<TResult>.Success(projected),
            combinedFacts);
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

    private static ImmutableArray<IDomainFact> NormalizeFacts(IReadOnlyList<IDomainFact>? facts)
    {
        if (facts is null || facts.Count == 0)
            return ImmutableArray<IDomainFact>.Empty;

        if (facts is ImmutableArray<IDomainFact> immutable)
            return immutable;

        return ImmutableArray.CreateRange(facts);
    }

    private static ImmutableArray<IDomainFact> ConcatFacts(
        ImmutableArray<IDomainFact> first,
        ImmutableArray<IDomainFact> second)
    {
        if (first.IsDefaultOrEmpty)
            return second.IsDefault ? ImmutableArray<IDomainFact>.Empty : second;

        if (second.IsDefaultOrEmpty)
            return first;

        var builder = ImmutableArray.CreateBuilder<IDomainFact>(first.Length + second.Length);
        builder.AddRange(first);
        builder.AddRange(second);
        return builder.MoveToImmutable();
    }
}
