## [ADR-0011](../adr/ADR-0011-dxa030-unapproved-handler.md): [DXA030](../analyzers/DXA030.md) Unapproved Handler

**Status:** Accepted  
**Date:** 2026-02-04

### Context
Result<T> handlers must be used consistently. Ad-hoc handling fragments patterns.

### Decision
Implement [DXA030](../analyzers/DXA030.md) to flag unapproved `Result` handlers.

Triggers:
- Direct Result.Value access without IsSuccess check
- Custom extensions not in approved list
- Handlers not registered via dx.result.handlers

Severity: Warning
Configuration: dx.result.handlers in .editorconfig

---

### Enforcement Coverage
**Enforced by:** [DXA030](../analyzers/DXA030.md)

**Coverage Level:** **Moderate**

---

### Enforcement Model
- **Type:** Static analyzer with config
- **Scope:** All code
- **Strength:** Moderate

---

### Guarantee
Consistent `Result` handling patterns.

### Dependencies
- [ADR-0009](../adr/ADR-0009-dxa020-result-ignored.md)

### Consequences
Enforces consistency; allows registration; prevents unsafe access.

### DPI Alignment
Items 3, 4 — compiler assistance, teaches through friction.
