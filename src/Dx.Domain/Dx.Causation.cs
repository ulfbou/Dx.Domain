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

