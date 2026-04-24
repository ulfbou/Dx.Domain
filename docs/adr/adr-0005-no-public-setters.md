## [ADR-0005](../adr/adr-0005-no-public-setters.md): No Public Setters on Domain Types

**Status:** Accepted  
**Date:** 2026-01-19  
**Applies to:** All Dx.Domain packages

### Context
Domain types with public setters allow mutation after construction, bypassing invariants. This creates temporal coupling where object is valid at creation but becomes invalid later.

### Decision
All domain types in kernel will have:
- Private setters or readonly fields
- No public property setters
- Mutation via methods that enforce invariants

Applies to Primitives, Kernel types, and Facts.

---

### Enforcement Coverage
**Enforced by:**
- **Code review** (primary)
- **[DXA040](../analyzers/dxa040.md)** — prevents adding mutable public API without ADR
- **Future: [DXA060](../analyzers/dxa060.md)** may flag "Setter" patterns

**Coverage Level:** **Moderate**

**Known Gaps:**
- No dedicated analyzer for setters yet
- Serialization frameworks may require workarounds

---

### Enforcement Model
- **Type:** Design guideline + surface freeze
- **Scope:** Kernel, Primitives, Facts
- **Strength:** Moderate

---

### Bypass Vectors
- Reflection
- Source-generated setters
- Internal mutation

---

### Guarantee
Public API surface does not expose mutable setters.

**Does NOT guarantee:**
- internal immutability

---

### Dependencies
- [ADR-0004](../adr/adr-0004-result-struct.md) (struct-based `Result` aligns with immutability)

### Consequences
**Positive:** Enforces invariants, prevents anemic model
**Negative:** Breaks serializers, requires converters, increases verbosity
**Trade-off accepted:** Correctness over serialization convenience

### Alternatives Considered
1. Public setters with validation — rejected
2. Init-only setters — rejected
3. Dual mutable/immutable — rejected

### DPI Alignment
- Item 1: Enforces invariant
- Item 4: Teaches through friction
- Item 7: No semantic expansion
