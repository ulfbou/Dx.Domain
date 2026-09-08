# Dx.Domain.Kernel

**Purpose:** Explicit results, errors, invariants, and recoverable requirements.
**Release:** `0.1.0-alpha`
**Targets:** .NET 8, .NET 9, and .NET 10

## Install

```bash
dotnet add package Dx.Domain.Kernel --version 0.1.0-alpha
```

## Key surface

`Result<T>`, `Result<TValue, TError>`, `DomainError`, `Invariant`, `Dx.Result`, and `Dx.Require`.

## Constraints

The package provides no persistence, transport, dispatch, logging, or business workflow. Public API and package composition are provisional during alpha.

## Related material

- [Architecture](../architecture.md)
- [Stability](../stability.md)
- [Limitations](../limitations.md)
