// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Fact{T}.cs" company="Dx.Domain Team">
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
    /// Represents an immutable domain fact with a strongly typed payload and associated metadata.
    /// </summary>
    /// <remarks>A fact encapsulates a discrete event or piece of information within a domain, including its
    /// type, payload, causation metadata, and timestamp. Facts are typically used to record domain events or state
    /// changes in event sourcing and domain-driven design scenarios. This struct is immutable and
    /// thread-safe.</remarks>
    /// <typeparam name="TPayload">The type of the payload carried by the fact. Must be non-nullable.</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct Fact<TPayload> : IDomainFact where TPayload : notnull
    {
        /// <inheritdoc />
        public FactId Id { get; }

        /// <inheritdoc />
        public string FactType { get; }

        /// <summary>Gets the payload associated with this fact.</summary>
        public TPayload Payload { get; }

        /// <inheritdoc />
        public Causation Causation { get; }

        /// <inheritdoc />
        public DateTimeOffset UtcTimestamp { get; }

        /// <summary>
        /// Initializes a new instance of the Fact class with the specified identifier, fact type, payload, causation,
        /// and optional UTC timestamp.
        /// </summary>
        /// <param name="id">The unique identifier for the fact.</param>
        /// <param name="factType">The type or category of the fact. Cannot be null or empty.</param>
        /// <param name="payload">The payload data associated with the fact.</param>
        /// <param name="causation">The causation information that describes the origin or reason for the fact.</param>
        /// <param name="utcTimestamp">The UTC timestamp when the fact occurred. If null, the current UTC time is used.</param>
        private Fact(FactId id, string factType, TPayload payload, Causation causation, DateTimeOffset? utcTimestamp = null)
        {
            Id = id;
            FactType = factType;
            Payload = payload;
            Causation = causation;
            UtcTimestamp = utcTimestamp ?? DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Creates a new domain fact with the specified type, payload, and causation metadata.
        /// </summary>
        /// <param name="factType">The logical type or category of the fact. Must not be null or whitespace.</param>
        /// <param name="payload">The fact payload.</param>
        /// <param name="causation">The causation metadata associated with this fact.</param>
        /// <returns>A new <see cref="Fact{T}"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
        internal static Fact<TPayload> Create(string factType, TPayload payload, Causation causation)
        {
            Invariant.That(!string.IsNullOrWhiteSpace(factType), Dx.Faults.Fact.MissingFactType);
            Invariant.That(causation.TraceId != TraceId.Empty, Dx.Faults.Fact.MissingTrace);
            Invariant.That(payload is not null, Dx.Faults.Fact.MissingPayload);
            return new(FactId.New(), factType, payload!, causation, DateTimeOffset.UtcNow);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Fact<{typeof(TPayload).Name}> Id={Id}, Type={FactType}";
    }
}
