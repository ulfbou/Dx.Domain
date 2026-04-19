# Dx.Domain.Facts

**Package**: `Dx.Domain.Facts`  
**Role**: Monotonic domain knowledge representation

## Purpose
Represent immutable, append-only facts derived from domain state. Structural, lineage-aware, **meaning-agnostic** history; **not** domain events.

## Guarantees
- Facts are immutable
- Facts are non-contradictory
- Facts are deterministic
- Monotonic append-only

## Constraints
- No mutation
- No workflow logic
- No runtime decision making

## Core Types
- **Fact<TPayload>** - Immutable fact with type, payload, and causation
- **Causation** - Required context: CorrelationId, TraceId, ActorId, timestamp
- **TransitionResult<TState>** - Result of state transitions

## Role in System
- Produced by Kernel
- Consumed by analyzers and generators
- `Fact.Create` requires Causation parameter

**Trade-off:** Every operation must thread context. Increases method signatures.  
**DPI alignment:** Enforces invariant, aligns with Manifesto demand for provenance.

See: [`public/packages/facts.md`](../../public/packages/facts.md)
