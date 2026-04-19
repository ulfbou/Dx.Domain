## [ADR-0009](../adr/ADR-0009-dxa020-result-ignored.md): [DXA020](../analyzers/dxa020.md) `Result` Ignored

**Status:** Accepted  
**Date:** 2026-02-02

### Context
Result<T> represents explicit failure, but callers can discard return value, silently ignoring domain failures.

### Decision
Implement [DXA020](../analyzers/dxa020.md) to flag discarded `Result` values.

Triggers:
- Invocation returning `Result` or `Result<T>` where return value not used
- Not assigned, not awaited, not passed to handler
- Excludes explicit discard with comment justification

Severity: Warning

---

### Enforcement Coverage
**Enforced by:** [DXA020](../analyzers/dxa020.md)

**Coverage Level:** **Strong**

---

### Enforcement Model
- **Type:** Static analyzer
- **Scope:** All code
- **Strength:** Strong

---

### Bypass Vectors
- Explicit discard with suppression
- Dynamic invocation

---

### Guarantee
Domain failures cannot be silently ignored in static code.

### Dependencies
- [ADR-0006](../adr/ADR-0006-result-as-failure-model.md)

### Consequences
Forces explicit handling; eliminates silent failure; increases verbosity.

### DPI Alignment
Items 3, 5 — compiler assistance, errors as values.

