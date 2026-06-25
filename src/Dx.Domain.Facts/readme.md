# Dx.Domain.Facts

Immutable types used to model domain facts with causation metadata.

## Purpose

Provide immutable value types and contracts for representing domain facts with causation metadata, enabling auditable state transitions without side effects.

## Guarantees

- Public fact types expose immutable state only
- Structural equality and stable hashing
- Thread-safe and allocation-conscious
- Causation metadata is always present and immutable

## Constraints

- Public fact types expose immutable state only
- Causation metadata is required for all facts
- Fact types are identified by string names; FactType provides stronger typing
- No dependencies beyond Primitives and Kernel

## Alpha Limitations

- API surface subject to change during alpha
- No compatibility guarantees prior to 0.1.0 stable
- Fact lifecycle, persistence, and storage are application-defined
- Breaking changes are unlikely but possible

## Role in System

- Provides the sanctioned package for modeling domain facts with causation metadata
- Consumed by domain and application layers for event sourcing and audit trails
- No governance or enforcement role; enforcement is provided by Analyzers

## Public API Surface

### Core Types

- `Fact<TPayload>` — Immutable readonly struct with payload, causation, and timestamp
- `Causation` — Immutable readonly struct for correlation, trace, actor, and timestamp provenance
- `IDomainFact` — Fact marker interface
- `FactType` — Strongly-typed fact type identifier
- `TransitionResult<TState>` — Result of state transition with associated facts

### Extensions

- `DomainFactExtensions.TryGetPayload<TPayload>()` — Safely extracts typed payload from a fact

### Creation

- `Fact<TPayload>.Create(...)` — Creates a fact and throws for invalid `factType`
- `Fact<TPayload>.TryCreate(...)` — Returns `Result<Fact<TPayload>>` for validation-friendly creation
- `Causation.Create(...)` — Creates causation metadata and enforces non-empty correlation/trace identifiers

```csharp
var causation = Causation.Create(correlationId, traceId, actorId);
var fact = Fact<string>.Create("UserRegistered", payload, causation);

var attempted = Fact<string>.TryCreate("UserRegistered", payload, causation);
if (attempted.IsSuccess)
{
	var created = attempted.Value;
}
```

## Non-Goals

- No policy evaluation
- No governance or enforcement
- No analyzer-driven semantics
- No persistence or serialization opinions

## Dependencies

- `Dx.Domain.Primitives` — Provides CorrelationId, TraceId, UserId
- `Dx.Domain.Kernel` — Provides Result types used in state transitions

## Versioning

0.1.0-alpha — API surface subject to change. No compatibility guarantees.

## See Also

- [Dx.Domain.Kernel](../Dx.Domain.Kernel/readme.md) — Runtime implementation
- [Dx.Domain.Primitives](../Dx.Domain.Primitives/readme.md) — Core value types
- [Dx.Domain.Annotations](../Dx.Domain.Annotations/readme.md) — Metadata vocabulary
- [Architecture Decision Records](../../docs/adr/index.md) — Design rationale
