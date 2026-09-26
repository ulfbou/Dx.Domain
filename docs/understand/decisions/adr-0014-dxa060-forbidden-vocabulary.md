## [ADR-0014](adr-0014-dxa060-forbidden-vocabulary.md): [DXA060](../../reference/diagnostics/DXA060.md) Forbidden Vocabulary

**Status:** Accepted  
**Date:** 2026-02-07

### Context
Certain terms indicate semantic expansion: "Manager", "Helper", "Util", "Service", base classes.

### Decision
Implement [DXA060](../../reference/diagnostics/DXA060.md) to flag forbidden vocabulary in kernel.

Triggers: Type names containing terms; method names like Process/Handle/Apply
Configurable via dx.forbidden.vocabulary

Severity: Warning in alpha, Error in stable

---

### Enforcement Coverage
**Enforced by:** [DXA060](../../reference/diagnostics/DXA060.md)

**Coverage Level:** **Moderate**

---

### Decision intent and demonstrated boundary
Prevents pattern museum.

### Dependencies
- [ADR-0007](adr-0007-system-hardening-sequence.md)

### Consequences
Forces emergent patterns; requires renaming.

### DPI Alignment
Items 6, 7 — prevents semantic expansion.
