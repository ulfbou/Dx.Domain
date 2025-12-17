// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Causation.cs" company="Dx.Domain Team">
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
    /// Captures correlation, trace, actor, and timestamp information that explains why a fact or operation occurred.
    /// </summary>
    /// <remarks>
    /// Typical usage when emitting a fact:
    /// <code>
    /// var causation = Causation.Create(correlationId, traceId, actorId);
    /// var fact = Fact.Create("OrderPlaced", payload, causation);
    /// </code>
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplay, nq")]
    public readonly struct Causation : IEquatable<Causation>
    {
        /// <summary>Gets the correlation identifier that groups related operations.</summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>Gets the trace identifier used for distributed tracing.</summary>
        public TraceId TraceId { get; }

        /// <summary>Gets the actor responsible for the action, if known.</summary>
        public ActorId ActorId { get; }

        /// <summary>Gets the UTC timestamp when the causation was recorded.</summary>
        public DateTimeOffset UtcTimestamp { get; }

        /// <summary>
        /// Initializes a new instance of the Causation class with the specified correlation, trace, actor, and
        /// timestamp information.
        /// </summary>
        /// <param name="correlationId">The unique identifier that correlates related operations or events.</param>
        /// <param name="traceId">The identifier used to trace the flow of a request or operation across system boundaries.</param>
        /// <param name="actorId">The identifier of the actor responsible for the operation, or null if not applicable.</param>
        /// <param name="utcTimestamp">The timestamp, in Coordinated Universal Time (UTC), indicating when the causation event occurred.</param>
        private Causation(CorrelationId correlationId, TraceId traceId, ActorId? actorId, DateTimeOffset utcTimestamp)
        {
            CorrelationId = correlationId;
            TraceId = traceId;
            ActorId = actorId ?? default;
            UtcTimestamp = utcTimestamp;
        }

        /// <summary>
        /// Creates a new <see cref="Causation"/> instance with the specified correlation and trace identifiers and optional actor.
        /// </summary>
        /// <param name="correlationId">The correlation identifier. Must not be empty.</param>
        /// <param name="traceId">The trace identifier. Must not be empty.</param>
        /// <param name="actorId">The optional actor responsible for the action.</param>
        /// <returns>A new <see cref="Causation"/> instance whose <see cref="UtcTimestamp"/> is set to <see cref="DateTimeOffset.UtcNow"/>.</returns>
        /// <remarks>
        /// This method enforces the invariant that both <paramref name="correlationId"/> and <paramref name="traceId"/>
        /// are non-empty. If either identifier is empty, an invariant violation is raised.
        /// </remarks>
        /// <exception cref="InvariantViolationException">
        /// Thrown if <paramref name="correlationId"/> or <paramref name="traceId"/> is empty.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Causation Create(CorrelationId correlationId, TraceId traceId, ActorId? actorId = null)
        {
            Dx.Invariant.That(correlationId.Value != Guid.Empty, Faults.Causation.MissingCorrelation);
            Dx.Invariant.That(!traceId.IsEmpty, Faults.Causation.MissingTrace);
            return new Causation(correlationId, traceId, actorId, DateTimeOffset.UtcNow);
        }

        /// <inheritdoc />
        public bool Equals(Causation other)
            => CorrelationId == other.CorrelationId &&
               TraceId == other.TraceId &&
               ActorId == other.ActorId &&
               UtcTimestamp == other.UtcTimestamp;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Causation other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(CorrelationId, TraceId, ActorId, UtcTimestamp);

        /// <inheritdoc />
        public static bool operator ==(Causation left, Causation right) => left.Equals(right);

        /// <inheritdoc />
        public static bool operator !=(Causation left, Causation right) => !left.Equals(right);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Causation={CorrelationId}, {TraceId}, {ActorId}, {UtcTimestamp:O}";
    }
}
