## [ADR-0012](../adr/ADR-0012-dxa040-kernel-public-surface-freeze.md): [DXA040](../analyzers/DXA040.md) Kernel Public Surface Freeze

**Status:** Accepted  
**Date:** 2026-02-05

### Context
Kernel public surface must remain minimal and frozen.

### Decision
Implement [DXA040](../analyzers/DXA040.md) to flag additions to kernel public surface.

Triggers:
- New public types in Kernel, Primitives, Facts
- New public members
- Excludes [ApprovedKernelApi] with ADR reference

Severity: Error

---

### Enforcement Coverage
**Enforced by:** [DXA040](../analyzers/DXA040.md)

**Coverage Level:** **Strong**

---

### Enforcement Model
- **Type:** Static analyzer
- **Scope:** Kernel packages
- **Strength:** Strong

---

### Guarantee
Prevents scope creep.

### Dependencies
- [ADR-0007](../adr/ADR-0007-system-hardening-sequence.md)

### Consequences
Forces new APIs to edges; requires ADR for expansion.

### DPI Alignment
Items 6, 7 — enforces Non-Goals.
