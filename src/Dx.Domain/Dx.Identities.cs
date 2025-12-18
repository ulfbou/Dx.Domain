// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dx.Identities.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Factors;

namespace Dx
{
    /// <summary>
    /// Root facade into the Dx Domain Kernel, exposing public factories for results, identities,
    /// causation, and related primitives while keeping enforcement mechanics internal.
    /// </summary>
    /// <remarks>
    /// All partial implementations of <see cref="Dx"/> contribute focused entry points (for example
    /// results, identities, causation, invariants, and preconditions) but present a single, cohesive
    /// surface to consumers of the domain kernel.
    /// </remarks>
    public static partial class Dx
    {
        /// <summary>
        /// Factories for creating <see cref="ActorId"/> values.
        /// </summary>
        public static partial class Actor
        {
            /// <summary>
            /// Creates a new <see cref="ActorId"/> with a freshly generated <see cref="Guid"/> value.
            /// </summary>
            /// <returns>A non-empty <see cref="ActorId"/>.</returns>
            public static ActorId New() => ActorId.InternalNew();

            /// <summary>
            /// Creates an <see cref="ActorId"/> from an existing <see cref="Guid"/> value.
            /// </summary>
            /// <param name="value">The GUID to wrap.</param>
            /// <returns>An <see cref="ActorId"/> whose underlying value is <paramref name="value"/>.</returns>
            public static ActorId From(Guid value) => ActorId.InternalFrom(value);
        }

        /// <summary>
        /// Factories for creating <see cref="CorrelationId"/> values.
        /// </summary>
        public static partial class Correlation
        {
            /// <summary>
            /// Creates a new <see cref="CorrelationId"/> with a freshly generated <see cref="Guid"/> value.
            /// </summary>
            /// <returns>A non-empty <see cref="CorrelationId"/>.</returns>
            public static CorrelationId New() => CorrelationId.InternalNew();

            /// <summary>
            /// Creates a <see cref="CorrelationId"/> from an existing <see cref="Guid"/> value.
            /// </summary>
            /// <param name="value">The GUID to wrap.</param>
            /// <returns>A <see cref="CorrelationId"/> whose underlying value is <paramref name="value"/>.</returns>
            public static CorrelationId From(Guid value) => CorrelationId.InternalFrom(value);
        }

        /// <summary>
        /// Factory for creating <see cref="TraceId"/> values used in distributed tracing.
        /// </summary>
        public static partial class Trace
        {
            /// <summary>
            /// Creates a new random <see cref="TraceId"/> instance.
            /// </summary>
            /// <returns>A <see cref="TraceId"/> with a uniformly random 128-bit value.</returns>
            public static TraceId New() => TraceId.InternalNew();
        }

        /// <summary>
        /// Factory for creating <see cref="SpanId"/> values used within a single trace.
        /// </summary>
        public static partial class Span
        {
            /// <summary>
            /// Creates a new random <see cref="SpanId"/> instance.
            /// </summary>
            /// <returns>A <see cref="SpanId"/> whose underlying value is non-zero with high probability.</returns>
            public static SpanId New() => SpanId.InternalNew();
        }

        /// <summary>
        /// Factories for creating fact identifiers and fact instances.
        /// </summary>
        public static partial class Fact
        {
            /// <summary>
            /// Factories for creating <see cref="FactId"/> values.
            /// </summary>
            public static partial class Id
            {
                /// <summary>
                /// Creates a new <see cref="FactId"/> with a freshly generated <see cref="Guid"/> value.
                /// </summary>
                /// <returns>A non-empty <see cref="FactId"/>.</returns>
                public static FactId New() => FactId.InternalNew();

                /// <summary>
                /// Creates a <see cref="FactId"/> from an existing <see cref="Guid"/> value.
                /// </summary>
                /// <param name="value">The GUID to wrap.</param>
                /// <returns>A <see cref="FactId"/> whose underlying value is <paramref name="value"/>.</returns>
                public static FactId From(Guid value) => FactId.InternalFrom(value);
            }

            /// <summary>
            /// Creates a new domain fact with the specified type, payload, and causation metadata.
            /// </summary>
            /// <typeparam name="TPayload">The type of the payload carried by the fact.</typeparam>
            /// <param name="factType">The logical type or category of the fact. Must not be null or whitespace.</param>
            /// <param name="payload">The fact payload.</param>
            /// <param name="causation">The causation metadata describing why this fact occurred.</param>
            /// <returns>A new <see cref="Fact{TPayload}"/> instance.</returns>
            public static Fact<TPayload> Create<TPayload>(string factType, TPayload payload, Causation causation)
                where TPayload : notnull
                => Fact<TPayload>.InternalCreate(factType, payload, causation);
        }
    }
}
