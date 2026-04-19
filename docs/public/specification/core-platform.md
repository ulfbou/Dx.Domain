# Core Platform Specification

## Scope
- S0 Kernel — Dx.Domain — trusted
- S1 Domain Facades — construction boundary
- S2 Application — orchestration
- S3 Infrastructure — I/O and adapters

## Package Map
- Kernel: Result<T>, DomainError, Invariant, Unit
- Primitives: CorrelationId, TraceId, ActorId, FactId, SpanId, UserId
- Facts: Fact<TPayload>, Causation, TransitionResult<TState>
- Annotations: Scope, DxAssemblyRole, AggregateRootAttribute, ValueObjectAttribute
- Analyzers: DXA010-DXA080

## Navigation
**Up**: [Architecture Overview](../architecture-overview.md)
**Packages**: [Annotations](../packages/annotations.md) | [Kernel](../packages/kernel.md)
