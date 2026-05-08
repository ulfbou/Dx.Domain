# Dx.Domain.Kernel

Functional core types for Dx.Domain.

## Purpose

Provide result types and error representations for domain operations without exceptions for control flow.

## Guarantees

- Public types expose immutable state
- Result types model success and failure explicitly

## Constraints

- No external dependencies beyond .NET runtime
- Public types are immutable
- No ambient context or static state

## Role in System

- Provides foundational types consumed by Primitives, Facts, and domain layers
- No governance or enforcement role
- Used for representing operation outcomes

## Public API Surface

### Result Types

- `Result<TValue, TError>` — Discriminated union for operations
- `Result<TValue>` — Result with DomainError

### Errors

- `DomainError` — Error representation
- `InvariantError` — Invariant violation error

### Supporting Types

- `DomainTime` — UTC timestamp
- `Unit` — Void result type
- `InvariantViolationException` — Exception for invariant failures

## Versioning

0.1.0-alpha — API surface subject to change. No compatibility guarantees.

## See Also

- [Dx.Domain.Primitives](../Dx.Domain.Primitives/readme.md) — Core value types
- [Dx.Domain.Facts](../Dx.Domain.Facts/readme.md) — Immutable facts
- [Dx.Domain.Annotations](../Dx.Domain.Annotations/readme.md) — Metadata vocabulary
- [Architecture Decision Records](../../docs/adr/index.md) — Design rationale
