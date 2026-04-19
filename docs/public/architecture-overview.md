# Dx.Domain — Architecture Overview

## What Dx.Domain Is
A substrate for invariants, results, errors, identities, and structural history. If it compiles, passes analyzers, and the Kernel accepts it, the state is valid.

It is intentionally small. Everything else belongs at the edges.

## The Four Packages
- **Annotations**: pure vocabulary, no runtime logic
- **Primitives**: immutable value types for IDs and tracing
- **Kernel**: Result<T>, DomainError, Invariant. No I/O.
- **Facts**: immutable, append-only history. Not domain events.

## Dependency Rules
1. Kernel depends on Annotations only
2. Primitives depends on Annotations only
3. Facts depends on Annotations, Primitives, Kernel
4. Analyzers depends on Annotations only

## Scopes S0–S3
- **S0 Kernel** — Dx.Domain itself. Trusted, exempt from DXA010/DXA011 per [ADR-0018](../adr/ADR-0018-kernel-public-surface.md)
- **S1 Domain Facades** — construction boundary
- **S2 Application** — orchestration
- **S3 Infrastructure** — I/O and adapters

Configure in `.editorconfig`:
```
dx.scope.map = S0:Dx.Domain;S1:MyApp.Domain;S2:MyApp.Application;S3:MyApp.Api
```

## Analyzer Governance
Analyzers apply to S1–S3 only:

- **DXA010** Construction Authority — [ADR-0003](../adr/ADR-0003-dxa010-warning.md)
- **DXA011** Public Factory Exposure — [ADR-0008](../adr/ADR-0008-dxa011-public-factory-exposure.md)
- **DXA020** Result Ignored
- **DXA022** No throw in Result methods
- **DXA030** Unapproved Handler
- **DXA040** Kernel Surface Freeze
- **DXA050** No temporal helpers
- **DXA060** Forbidden vocabulary
- **DXA070** Generated code tagging
- **DXA080** Facade Invariant Enforcement

## Navigation
**Up:** [Overview](OVERVIEW.md)
**Next:** [Core Platform Specification](specification/core-platform.md)
**Learn:** [Dx.Domain in 90 seconds](../learn/index.md)
**Packages:** [Annotations](packages/annotations.md) | [Primitives](packages/primitives.md) | [Kernel](packages/kernel.md) | [Facts](packages/facts.md) | [Analyzers](packages/analyzers.md)

