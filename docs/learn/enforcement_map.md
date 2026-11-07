# Enforcement Map

**Status:** Pre-release alpha  
**Applies to:** Dx.Domain v0.1.0-alpha
**Last reviewed:** 2026-04-22

> Philosophy in this repository is not aspirational. It is mechanically enforced. This map connects each principle to its implementation mechanism and its enforcement tool.

## Map

| Principle | Mechanism in code | Enforcement |
| --- | --- | --- |
| No ambiguity in identity | Strongly typed structs: `CorrelationId`, `TraceId`, `ActorId`, `FactId`, `SpanId` in Dx.Domain.Primitives | Compiler type checking. No implicit conversions to Guid or string. |
| Explicit correlation across boundaries | `Causation` struct requiring non-empty CorrelationId and TraceId | `Invariant.That` checks in `Causation.Create`. Runtime exception on violation. |
| UTC-only time | `DomainTime` with private constructor and `From` method checking `Offset == TimeSpan.Zero` | Runtime invariant. Future analyzer planned. |
| No silent failure | `Result<T>` and `DomainError` in Dx.Domain.Kernel | Type system forces handling. No exceptions for domain failures by design. |
| Invariants are central | `Invariant.That(condition, code, message)` | Runtime throw of `InvariantViolationException`. Used throughout Kernel and Facts. |
| Construction authority | Private constructors on primitives and domain types. Creation only via static `Create` or `Dx` facade | DXA010 analyzer: "Create domain instances via the Dx facade. Direct construction or public factory use is forbidden outside kernel packages." Current severity: Warning. |
| No direct `new Guid()` for IDs | Factory methods `CorrelationId.Create()`, `TraceId.Create()` etc. | DXA010 flags `new CorrelationId()` or `new Guid()` usage in consuming code. |
| Semantic vocabulary | Attributes in Dx.Domain.Annotations: `AggregateRootAttribute` and others | Analyzers will validate correct usage. Generators will emit code based on attributes. |

## How to read this

- **Principle:** From MANIFESTO.md and NON_GOALS.md
- **Mechanism:** The concrete type or method that embodies it
- **Enforcement:** What stops a violation today (compiler, analyzer, runtime)

## Current enforcement gaps

| Gap | Status |
| --- | --- |
| Invariant usage not enforced by analyzer | Planned |
| Result must be checked | Planned analyzer |
| DomainTime must be used instead of DateTimeOffset | Planned |
| Generator output validation | Not implemented |

This map will be updated each alpha when enforcement moves from runtime to compile-time.
