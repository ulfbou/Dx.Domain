## [ADR-0016](../adr/adr-0016-dxa080-facade-invariant-enforcement.md): [DXA080](../analyzers/dxa080.md) Facade Invariant Enforcement

**Status:** Accepted  
**Date:** 2026-02-09

### Context
Dx facade methods must enforce invariants.

### Decision
Implement [DXA080](../analyzers/dxa080.md) to verify facade methods call Invariant.That.

Triggers: Public methods in Dx facade creating domain types without invariant call
Severity: Error

---

### Enforcement Coverage
**Enforced by:** [DXA080](../analyzers/dxa080.md)

**Coverage Level:** **Moderate**

---

### Guarantee
Facade does not become bypass vector.

### Dependencies
- [ADR-0003](../adr/adr-0003-dxa010-warning.md)

### Consequences
Centralizes invariant enforcement; increases complexity.

### DPI Alignment
Items 1, 4 — enforces invariant, teaches through friction.
