## [ADR-0009](adr-0009-dxa020-result-ignored.md): [DXA020](../../reference/diagnostics/DXA020.md) `Result` Ignored

**Status:** Accepted  
**Date:** 2026-02-02

### Context
Result<T> represents explicit failure, but callers can discard return value, silently ignoring domain failures.

### Decision
Implement [DXA020](../../reference/diagnostics/DXA020.md) to flag discarded `Result` values.

Triggers:
- Invocation returning `Result` or `Result<T>` where return value not used
- Not assigned, not awaited, not passed to handler
- Excludes explicit discard with comment justification

Severity: Warning (alpha); subject to configuration in stable release

---

### Enforcement Coverage
**Enforced by:** [DXA020](../../reference/diagnostics/DXA020.md)

**Coverage Level (Historical):** Strong (as recorded in 2026-02-02)

**Current Strength:** Moderate — See [Dx.Domain Enforcement Specification](../enforcement-specification.md) for current authoritative classification.

---

### Enforcement Model
- **Type:** Static analyzer
- **Scope:** S1–S3; explicitly exempt from S0 (kernel code)
- **Strength:** Heuristic-based; catches common direct ignores but bypassable via reflection or dynamic invocation

---

### Bypass Vectors
- Explicit discard with suppression
- Dynamic invocation
- Reflection-based Result creation

---

### Decision intent and demonstrated boundary
Domain failures cannot be silently ignored in statically analyzable code.

### Dependencies
- [ADR-0006](adr-0006-result-as-failure-model.md)

### Consequences
Forces explicit handling; eliminates silent failure; increases verbosity.

### DPI Alignment
Items 3, 5 — compiler assistance, errors as values.

