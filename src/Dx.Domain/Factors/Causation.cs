// <summary>
//     <list type="bullet">
//         <item>
//             <term>File:</term>
//             <description>Causation.cs</description>
//         </item>
//         <item>
//             <term>Project:</term>
//             <description>Dx.Domain</description>
//         </item>
//         <item>
//             <term>Description:</term>
//             <description>
//                 Defines the causation value object used to capture correlation, trace, actor, and timestamp
//                 metadata for domain facts and operations.
//             </description>
//         </item>
//     </list>
// </summary>
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

namespace Dx.Domain.Factors
{
    using Dx.Domain.Invariants;

    using System.Diagnostics;

    /// <summary>
    /// Represents contextual information that identifies the origin and trace of an operation, including correlation,
    /// trace, actor, and timestamp data.
    /// </summary>
    /// <remarks>Use this struct to propagate causation details across service boundaries or between
    /// components to enable distributed tracing, correlation, and auditing. Causation information is typically attached
    /// to messages, events, or logs to provide end-to-end visibility into the flow of operations. All properties are
    /// immutable.</remarks>
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
    public readonly struct Causation : IEquatable<Causation>
    {
        /// <summary>Gets the correlation identifier that groups related operations.</summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>Gets the trace identifier used for distributed tracing.</summary>
        public TraceId TraceId { get; }

        /// <summary>Gets the actor responsible for the action, if known.</summary>
        public ActorId ActorId { get; }

        /// <summary>Gets the UTC timestamp when the causation was recorded.</summary>
        public DateTime UtcTimestamp { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Causation(CorrelationId correlationId, TraceId traceId, ActorId? actorId, DateTime utcTimestamp)
        {
            CorrelationId = correlationId;
            TraceId = traceId;
            ActorId = actorId ?? ActorId.Empty;
            UtcTimestamp = utcTimestamp;
        }

        /// <summary>
        /// Creates a new <see cref="Causation"/> instance with the specified correlation and trace identifiers and optional actor.
        /// </summary>
        /// <param name="correlationId">The correlation identifier. Must not be empty.</param>
        /// <param name="traceId">The trace identifier. Must not be empty.</param>
        /// <param name="actorId">The optional actor responsible for the action.</param>
        /// <returns>A new <see cref="Causation"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Causation Create(CorrelationId correlationId, TraceId traceId, ActorId? actorId = null)
        {
            Invariant.That(!correlationId.IsEmpty, DomainErrors.Causation.MissingCorrelation);
            Invariant.That(!traceId.IsEmpty, DomainErrors.Causation.MissingTrace);
            return new Causation(correlationId, traceId, actorId, DateTime.UtcNow);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Causation other)
            => CorrelationId == other.CorrelationId &&
               TraceId == other.TraceId &&
               ActorId == other.ActorId &&
               UtcTimestamp == other.UtcTimestamp;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is Causation other && Equals(other);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
            => HashCode.Combine(CorrelationId, TraceId, ActorId, UtcTimestamp);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Causation left, Causation right) => left.Equals(right);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Causation left, Causation right) => !left.Equals(right);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => $"Causation {{ CorrelationId = {CorrelationId}, TraceId = {TraceId}, ActorId = {ActorId}, UtcTimestamp = {UtcTimestamp:u} }}";
        }
    }
}
