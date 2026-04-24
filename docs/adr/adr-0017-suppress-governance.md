## **ADR-0017**: Suppression Governance

**Status:** Accepted  
**Date:** 2026-04-22

### Context
Analyzer suppressions must be justified to prevent erosion of guarantees.

### Decision
Require justification for all suppressions via `[SuppressMessage]` with ADR reference or issue link. DXA090 (planned) will flag unjustified suppressions.

---

### Enforcement Coverage
**Enforced by:** Process + future DXA090 (planned)

**Coverage Level:** **Process**

---

### Guarantee
Suppressions are auditable.

### Dependencies
- [ADR-0007](../adr/adr-0007-system-hardening-sequence.md)
