## ADR-0006: Result as Failure Model (Exclude Option/Either)

**Status:** Accepted  
**Date:** 2026-01-29
**Updated:** 2026-04-23

### Context
Functional wrappers like `Option`, `Either`, `NonEmpty` offer marginal semantic benefits but introduce surface area, governance costs, and analyzer burden.

### Decision
Exclude Option, Either, NonEmpty from **domain-facing APIs**. Use `Result<T>` for domain control-flow semantics.

Kernel retains `Result<TSuccess,TFailure>` as the low-level primitive. `Result<T>` is the domain alias for `Result<T, DomainError>`.

---

### Amendment 2026-04-23 — Clarification
This ADR does not forbid the two-type Result in Kernel. It forbids exposing Option/Either semantics to domain authors. The generic form exists to avoid boxing and to support analyzers, and is covered by the S0 exemption in ADR-0003.

### Enforcement Coverage
**Enforced by:**
- **[DXA040](../analyzers/dxa040.md)** — Kernel Public Surface Freeze (prevents addition)
- **[DXA020](../analyzers/dxa020.md), [DXA022](../analyzers/dxa022.md), [DXA030](../analyzers/dxa030.md)** — enforce `Result` usage patterns

**Coverage Level:** **Strong**

**Known Gaps:**
- External libraries may introduce similar types at edges

---

### Enforcement Model
- **Type:** Design decision + analyzer
- **Scope:** Kernel
- **Strength:** Strong

---

### Bypass Vectors
- Consumer code defining own Option types (allowed at edges)

---

### Guarantee
Kernel exposes single failure representation to domain code.

**Does NOT guarantee:**
- edge code choices

---

### Dependencies
- [ADR-0004](adr-0004-result-struct.md) (Result as struct)

### Consequences
- Smaller, clearer kernel
- Fewer analyzer permutations
- `Result<T>` becomes single failure representation for domain authors

### DPI Alignment
- Items 2, 6, 7 — reduces complexity, enforces Non-Goals

