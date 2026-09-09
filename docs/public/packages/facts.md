# Dx.Domain.Facts

**Purpose:** Immutable structural facts and causation values.
**Release:** `0.1.0-alpha`
**Targets:** .NET 8, .NET 9, and .NET 10

## Install

```bash
dotnet add package Dx.Domain.Facts --version 0.1.0-alpha
```

## Key surface

`Fact<TPayload>`, `FactType`, `Causation`, and `TransitionResult<TState>`.

## Constraints

Facts are structural records, not domain-event dispatch. Public API and package composition are provisional during alpha.

## Related material

- [Architecture](../architecture.md)
- [Stability](../stability.md)
- [Limitations](../limitations.md)
