## [ADR-0013](../adr/ADR-0013-dxa050-temporal-helper-usage.md): [DXA050](../analyzers/DXA050.md) Temporal Helper Usage

**Status:** Accepted  
**Date:** 2026-02-06

### Context
Direct use of DateTime.Now bypasses DomainTime.

### Decision
Implement [DXA050](../analyzers/DXA050.md) to flag direct temporal helper usage.

Triggers: DateTime.Now, UtcNow, DateTimeOffset.Now, Stopwatch.GetTimestamp in domain logic
Excludes infrastructure/test

Severity: Warning
Remediation: Use DomainTime.Now()

---

### Enforcement Coverage
**Enforced by:** [DXA050](../analyzers/DXA050.md)

**Coverage Level:** **Strong**

---

### Enforcement Model
- **Type:** Static analyzer
- **Scope:** Domain layer
- **Strength:** Strong

---

### Guarantee
Enforces [ADR-0001](../adr/ADR-0001-utc-only-domaintime.md).

### Dependencies
- [ADR-0001](../adr/ADR-0001-utc-only-domaintime.md)

### Consequences
Makes time explicit; breaks direct DateTime code.

### DPI Alignment
Items 1, 3 — enforces invariant, compiler assistance.
