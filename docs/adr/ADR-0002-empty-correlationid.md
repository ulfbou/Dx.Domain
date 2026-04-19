## [ADR-0002](../adr/ADR-0002-empty-correlationid.md): Empty CorrelationId Permitted

**Status:** Accepted  
**Date:** 2026-01-16  
**Applies to:** Dx.Domain.Primitives

### Context
Correlation IDs group related operations across services. In some contexts (background jobs, system-initiated operations), no correlation exists.

Question: Should `CorrelationId` require non-empty value, or permit empty to represent uncorrelated context?

### Decision
`CorrelationId.Empty` is permitted and represents an explicitly uncorrelated context.

`Causation.Create` requires non-empty CorrelationId and TraceId for normal operations, but allows Empty for specific scenarios via separate factory or parameter.

Empty is a valid value, not a null or error state.

---

### Enforcement Coverage
**Enforced by:**
- **Code-level invariant** in `Causation.Create` (throws if empty in normal path)
- **Type design** — `CorrelationId` exposes `Empty` as explicit sentinel

**Coverage Level:** **Partial (Runtime)**

**Known Gaps:**
- Developers may pass Empty where correlation is expected
- No static analyzer prevents misuse

---

### Enforcement Model
- **Type:** Runtime invariant + API design
- **Scope:** Primitives and Facts packages
- **Strength:** Moderate

---

### Bypass Vectors
- Direct struct construction via reflection
- Bypassing `Causation.Create` and using private ctor

---

### Guarantee
Empty correlation is explicit, not accidental. Normal causation creation fails fast on empty values.

**Does NOT guarantee:**
- semantic correctness of using Empty

---

### Dependencies
- [ADR-0001](../adr/ADR-0001-utc-only-domaintime.md) (UTC time for Causation timestamp)

### Consequences
**Positive:**
- Makes lack of correlation explicit rather than accidental
- Allows system-initiated operations without fabricating correlation
- Distinguishes "no correlation" from "forgot to set correlation"

**Negative:**
- Developers may misuse Empty instead of generating proper correlation
- Requires discipline to use Empty only when appropriate
- Adds complexity to Causation validation logic

**Trade-off accepted:** Explicitness over enforcement.

### Alternatives Considered
1. **Require non-empty always:** Rejected — forces fabrication of IDs
2. **Use nullable CorrelationId?:** Rejected — introduces null handling
3. **Separate Uncorrelated type:** Rejected — unnecessary complexity

### DPI Alignment
- Item 2: Reduces accidental complexity
- Item 4: Teaches through friction — Empty is explicit
- Item 5: Aligns with Manifesto demand for explicitness
