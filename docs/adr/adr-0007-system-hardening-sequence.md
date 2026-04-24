## [ADR-0007](../adr/adr-0007-system-hardening-sequence.md): System Hardening Sequence

**Status:** Accepted  
**Date:** 2026-01-29

### Context
To reach "closed under explanation" architecture, we must split Facts cleanly, finalize Kernel laws, tighten Primitives, and harden analyzers.

### Decision (order of work)
1. **Facts split**
2. **Kernel cleanup**
3. **Primitives tightening**
4. **Analyzer hardening** (DXA090 (planned) for suppression detection)

---

### Enforcement Coverage
**Enforced by:**
- Project milestones and ADR dependencies

**Coverage Level:** **Process**

---

### Enforcement Model
- **Type:** Governance
- **Scope:** Repository
- **Strength:** Administrative

---

### Guarantee
Substrate stability through ordered hardening.

### Consequences
Consumer misuse fails deterministically at build time.

