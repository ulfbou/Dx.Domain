# Dx.Domain System Model

## Purpose
A small, opinionated substrate for invariants, results, errors, identities, and structural history — designed so incorrect domain models are hard to express. If it compiles, passes analyzers, and the Kernel accepts it, the state is valid.

*Source: docs/learn/architecture-overview.md*

## The Four Packages
- **Annotations** — pure vocabulary and metadata; no runtime logic
- **Primitives** — immutable, side-effect-free value types
- **Kernel** — the runtime judge of invariants, results, errors, facts; no I/O/infrastructure
- **Facts** — structural, lineage-aware, meaning-agnostic history; not domain events

*Source: docs/public/architecture-overview.md*

## Dependency Rules
1. Kernel depends on Annotations only
2. Primitives depends on Annotations only
3. Facts depends on Annotations, Primitives, Kernel
4. Analyzers depends on Annotations only

*Source: docs/public/architecture-overview.md*

## Scopes S0–S3
- **S0 Kernel** — Dx.Domain itself. Trusted, exempt from DXA010/DXA011 per ADR-0018
- **S1 Domain Facades** — construction boundary
- **S2 Application** — orchestration
- **S3 Infrastructure** — I/O and adapters

*Source: docs/public/architecture-overview.md*

## Construction Authority
All domain objects in S1, S2, and S3 must be created through controlled facade or factory entry points. S0 Kernel is explicitly excluded.

S0 types exempt from DXA010/DXA011/DXA080:
- Result<T>, Result<TSuccess,TFailure>, DomainError, Invariant, Unit

*Source: ADR-0003, ADR-0018*
