// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Dx.Domain;
using Dx.Domain.Errors;
using Dx.Domain.Primitives;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Dx.Domain.Facts;

/// <summary>
/// Represents an immutable domain fact with a strongly typed payload and associated metadata.
/// </summary>
/// <typeparam name="TPayload">The type of the payload carried by the fact. Must be non-nullable.</typeparam>
/// <remarks>
/// <para>
/// A fact encapsulates a discrete event or piece of information within a domain, including its
/// type, payload, causation metadata, and timestamp.
/// </para>
/// <para>
/// Facts are structural, lineage-aware primitives used to record domain state changes.
/// They are NOT domain events (domain events are business-level semantics).
/// </para>
/// <para>
/// This struct is immutable and thread-safe.
/// </para>
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct Fact<TPayload> : IDomainFact<TPayload>
    where TPayload : notnull
{
    /// <inheritdoc />
    public FactId Id { get; }

    /// <inheritdoc />
    public string FactType { get; }

    /// <summary>
    /// Gets the payload associated with this fact.
    /// </summary>
    public TPayload Payload { get; }

    /// <inheritdoc />
    public Causation Causation { get; }

    /// <inheritdoc />
    public DateTimeOffset UtcTimestamp { get; }

    /// <inheritdoc />
    public TPayload GetPayload() => Payload;

    object IDomainFact.GetPayload() => Payload;

    private Fact(
        FactId id,
        string factType,
        TPayload payload,
        Causation causation,
        DateTimeOffset? utcTimestamp)
    {
        Id = id;
        FactType = factType ?? throw new ArgumentNullException(nameof(factType));
        Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        Causation = causation;
        UtcTimestamp = utcTimestamp ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Creates a new domain fact with the specified type, payload, and causation metadata.
    /// </summary>
    /// <param name="factType">The logical type or category of the fact. Must not be <see langword="null"/> or whitespace.</param>
    /// <param name="payload">The fact payload. Must not be <see langword="null"/>.</param>
    /// <param name="causation">The causation metadata associated with this fact.</param>
    /// <param name="utcTimestamp">
    /// The UTC timestamp when the fact occurred. If <see langword="null"/>, the current UTC time is used.
    /// </param>
    /// <returns>A new <see cref="Fact{TPayload}"/> instance.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="factType"/> or <paramref name="payload"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="factType"/> is whitespace.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types",
        Justification = "Factory pattern for immutable struct; idiomatic for Facts")]
    public static Fact<TPayload> Create(
        string factType,
        TPayload payload,
        Causation causation,
        DateTimeOffset? utcTimestamp = null)
    {
        if (string.IsNullOrWhiteSpace(factType))
            throw new ArgumentException("Fact type must not be null or whitespace.", nameof(factType));

        return new Fact<TPayload>(
            id: FactId.New(),
            factType: factType,
            payload: payload,
            causation: causation,
            utcTimestamp: utcTimestamp);
    }

    /// <summary>
    /// Tries to create a new domain fact with the specified type, payload, and causation metadata.
    /// </summary>
    /// <param name="factType">The logical type or category of the fact. Must not be <see langword="null"/> or whitespace.</param>
    /// <param name="payload">The fact payload. Must not be <see langword="null"/>.</param>
    /// <param name="causation">The causation metadata associated with this fact.</param>
    /// <param name="utcTimestamp">
    /// The UTC timestamp when the fact occurred. If <see langword="null"/>, the current UTC time is used.
    /// </param>
    /// <returns>A <see cref="Result{TValue}"/> containing the new fact or a failure error.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types",
        Justification = "Factory pattern for immutable struct; idiomatic for Facts")]
    public static Result<Fact<TPayload>> TryCreate(
        string factType,
        TPayload payload,
        Causation causation,
        DateTimeOffset? utcTimestamp = null)
    {
        if (string.IsNullOrWhiteSpace(factType))
            return Result<Fact<TPayload>>.Failure(CreateInvalidFactTypeError());

        if (payload is null)
            return Result<Fact<TPayload>>.Failure(CreateNullPayloadError());

        return Result<Fact<TPayload>>.Success(new Fact<TPayload>(
            id: FactId.New(),
            factType: factType,
            payload: payload,
            causation: causation,
            utcTimestamp: utcTimestamp));
    }

    private string DebuggerDisplay
        => $"Fact<{typeof(TPayload).Name}>: {FactType} (Id={Id}, Time={UtcTimestamp:O})";

    private static DomainError CreateInvalidFactTypeError()
        => DomainError.Create("dx.facts.invalid_fact_type", "Fact type must not be null or whitespace.");

    private static DomainError CreateNullPayloadError()
        => DomainError.Create("dx.facts.null_payload", "Fact payload must not be null.");
}
