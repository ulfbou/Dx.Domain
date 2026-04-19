## ADR-0008: DXA011 Public Factory Exposure

**Status:** Accepted
**Date:** 2026-02-01
**Updated:** 2026-04-23

### Context
Public factory methods on domain types allow bypass of Dx facade, fragmenting construction authority.

### Decision
Implement DXA011 to flag public factory methods on domain types **in S1 Domain, S2 Application, and S3 Infrastructure assemblies only**.

Triggers (all must be true):
- Public static method returning a domain type
- Located in assembly with DxAssemblyRole S1, S2, or S3
- Not marked with [DxFacade] or equivalent
- Declaring type is not in Dx.Domain.Kernel, Primitives, or Facts

Severity: Warning in alpha, Error in stable

---

### Amendment 2026-04-23 — S0 Exemption
DXA011 explicitly ignores S0 Kernel assemblies. This resolves the NuGet instantiation issue for Result<T>, Result<TSuccess,TFailure>, DomainError, and primitives. See ADR-0003 for exemption list.

### Enforcement Coverage
**Enforced by:** [DXA011](../analyzers/DXA011.md)

**Coverage Level:** **Moderate–Strong**

**Known Gaps:** Reflection, internal factories

---

### Enforcement Model
- **Type:** Static analyzer
- **Scope:** Consumer assemblies (S1–S3)
- **Strength:** Moderate

---

### Bypass Vectors
- Reflection invocation
- Internal visibility factories

---

### Guarantee
All public construction routes in S1–S3 are visible to facade.

### Dependencies
- [ADR-0003](../adr/ADR-0003-dxa010-warning.md)

### Consequences
Forces S1–S3 construction through single facade; enables centralized logging; breaks direct factory calls in consumer code. Does not affect S0.

### DPI Alignment
Items 1, 4, 7 — enforces invariant, teaches through friction, no semantic expansion.
