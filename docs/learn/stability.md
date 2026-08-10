# Enforcement Map

**Status:** Pre-release alpha
**Applies to:** Dx.Domain v0.1.0-alpha
**Last reviewed:** 2026-04-23

> Philosophy is mechanically enforced.

## Map

| Principle | Mechanism in code | Enforcement |
| --- | --- | --- |
| No ambiguity in identity | Strongly typed structs in Primitives | Compiler type checking |
| Explicit correlation | `Causation` requiring CorrelationId and TraceId | `Invariant.That` in Causation.Create |
| UTC-only time | `DomainTime` | Runtime invariant, DXA050 |
| No silent failure | `Result<T>` with public Success/Failure | DXA020 (S1–S3) |
| Invariants are central | `Invariant.That` | Runtime throw |
| Construction authority | Private constructors, Dx facade | DXA010, DXA011, DXA080 (S1–S3 only, S0 exempt per ADR-0018) |
| No direct new Guid for IDs | Factory methods | DXA010 flags in S1–S3 |
| Semantic vocabulary | Attributes in Annotations | Analyzers validate usage |

## Current enforcement gaps

| Gap | Status |
| --- | --- |
| Invariant usage not enforced by analyzer | Planned |
| DomainTime must replace DateTimeOffset | DXA050 partial |
| Generator output validation | Not implemented |

## Navigation

- [ADR-0003](../adr/adr-0003-dxa010-warning.md)
- [ADR-0018](../adr/adr-0018-kernel-public-surface.md)
