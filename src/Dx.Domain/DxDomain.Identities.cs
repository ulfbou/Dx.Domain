// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxDomain.Identities.cs" company="Dx.Domain Team">
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
using Dx.Domain.Primitives;

using System;

namespace Dx.Domain
{
    public static partial class DxDomain
    {
        /// <summary>Factories for creating <see cref="ActorId"/> values.</summary>
        public static partial class Actor
        {
            public static ActorId New() => ActorId.InternalNew();

            public static ActorId From(Guid value) => ActorId.InternalFrom(value);
        }

        public static partial class Correlation
        {
            public static CorrelationId New() => CorrelationId.New();

            public static CorrelationId From(Guid value) => CorrelationId.From(value);
        }

        public static partial class Trace
        {
            public static TraceId New() => TraceId.New();
        }

        public static partial class Span
        {
            public static SpanId New() => SpanId.New();
        }

        public static partial class Fact
        {
            public static partial class Id
            {
                public static FactId New() => FactId.New();

                public static FactId From(Guid value) => FactId.From(value);
            }

            public static Fact<TPayload> Create<TPayload>(string factType, TPayload payload, Causation causation)
                where TPayload : notnull
                => Fact<TPayload>.InternalCreate(factType, payload, causation);
        }

        public static partial class CausationFactory
        {
            public static Causation Create(
                CorrelationId correlationId,
                TraceId traceId,
                ActorId? actorId = null)
                => Causation.InternalCreate(correlationId, traceId, actorId);
        }
    }
}
