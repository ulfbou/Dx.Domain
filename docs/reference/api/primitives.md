# Dx.Domain.Primitives

**Package**: `Dx.Domain.Primitives`  
**Role**: Strongly-typed primitives for identity and tracing

## Purpose
Immutable, side-effect-free value types for identities and distributed tracing.

## Core Types
- **CorrelationId** - Correlates operations across service boundaries
- **TraceId** - Distributed trace identifier
- **ActorId** - Identifies the actor performing an operation
- **FactId** - Unique identifier for a structural fact
- **SpanId** - Identifies spans within a trace
- **UserId** - Strongly-typed user identifier

## Guarantees
- Immutable by construction
- No I/O, no side effects
- Deterministic equality and hashing
- Cannot be default(T) or empty where semantically invalid

## Usage
Primitives are required parameters for `Fact.Create` and all operations that need causation tracking. Empty `CorrelationId` is permitted for uncorrelated contexts.

See: [`public/packages/primitives.md`](../../public/packages/primitives.md)
