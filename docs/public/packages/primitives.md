# Dx.Domain.Primitives

**Purpose:** Strongly typed, immutable identity values.
**Release:** `0.1.0-alpha`
**Targets:** .NET 8, .NET 9, and .NET 10

## Install

```bash
dotnet add package Dx.Domain.Primitives --version 0.1.0-alpha
```

## Key surface

`UserId`, `CorrelationId`, `TraceId`, `ActorId`, `FactId`, and `SpanId`.

## Constraints

Default struct values and serializer or reflection bypass remain possible. Public API and package composition are provisional during alpha.

## Related material

- [Architecture](../architecture.md)
- [Stability](../stability.md)
- [Limitations](../limitations.md)
