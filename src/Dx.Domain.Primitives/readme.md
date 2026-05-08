# Dx.Domain.Primitives

Strongly-typed identity and tracing primitives.

## Purpose

Provide value types for correlation, tracing, and identity.

## Guarantees

- Public types expose immutable state
- Structural equality

## Constraints

- No implicit conversions
- No dependencies on Kernel

## Role in System

- Consumed by Kernel and Facts
- Provides foundational value types

## Public API Surface

### Identity and Tracing

- `CorrelationId` — Correlation identifier
- `TraceId` — Trace identifier
- `SpanId` — Span identifier
- `UserId` — User identifier
- `FactId` — Fact identifier

## Versioning

0.1.0-alpha — API surface subject to change. No compatibility guarantees.

## See Also

- [Dx.Domain.Kernel](../Dx.Domain.Kernel/readme.md) — Functional core types
- [Dx.Domain.Facts](../Dx.Domain.Facts/readme.md) — Immutable facts
- [Architecture Decision Records](../../docs/adr/index.md) — Design rationale
