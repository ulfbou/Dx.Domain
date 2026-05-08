# Dx.Domain.Facts

Immutable types used to model domain facts.

## Purpose

Provide immutable record types for representing domain facts with causation metadata.

## Guarantees

- Public fact types expose immutable state

## Role in System

- Provide types for modeling domain facts with causation metadata
- No governance or enforcement role

## Public API Surface

- `Fact<TPayload>` — Immutable fact with payload and causation
- `Causation` — Correlation and trace identifiers for fact provenance
- `IDomainFact` — Fact marker interface
- `FactType` — Fact type identifier
- `TransitionResult<TState>` — Result of state transition with associated facts

### Creation

```csharp
var causation = Causation.Create(correlationId, traceId, actorId);
var fact = Fact.Create("UserRegistered", payload, causation);
````

## Non-Goals

*   No policy evaluation
*   No governance or enforcement
*   No analyzer-driven semantics

## Dependencies

*   `Dx.Domain.Primitives`  
    Provides correlation, tracing, and identity value types.

*   `Dx.Domain.Kernel`  
    Provides core domain result types used in state transitions.

## Constraints

*   Public fact types expose immutable state only
*   Causation metadata is required for all facts
*   Fact types are identified by string names; `FactType` exists for stronger typing

## Versioning

0.1.0-alpha — API surface subject to change. No compatibility guarantees.

## See Also

- [Dx.Domain.Kernel](../Dx.Domain.Kernel/readme.md) — Runtime implementation
- [Dx.Domain.Primitives](../Dx.Domain.Primitives/readme.md) — Core value types
- [Architecture Decision Records](../../docs/adr/index.md) — Design rationale
