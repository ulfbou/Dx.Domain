## [ADR-0001](adr-0001-utc-only-domaintime.md): Temporal Authority (UTC / DomainTime)

**Status:** Accepted  
**Date:** 2026-01-10

### Context
Direct use of system time introduces non-determinism and time zone ambiguity in domain logic.

### Decision
All domain time must flow through `DomainTime` (UTC only).

---

### Enforcement Coverage
**Enforced by:**
- **[DXA050](../../reference/diagnostics/DXA050.md)** — Temporal Helper Usage

**Coverage Level:** **Strong (Domain Layer, Static Scope)**

**Known Gaps:**
- Infrastructure code
- External inputs (DTOs, APIs)
- Runtime reflection/dynamic invocation

---

### Enforcement Model
- **Type:** Static analyzer
- **Scope:** Domain layer, intra-assembly
- **Strength:** Strong (within static visibility)

---

### Bypass Vectors
- Infrastructure utilities
- Reflection or dynamic invocation
- External system payloads

---

### Decision intent and demonstrated boundary
All statically analyzable domain code avoids direct `DateTime` access and uses controlled UTC sources.

**Outside this boundary:**
- correctness of time values
- runtime consistency across systems

---

### Dependencies
None
