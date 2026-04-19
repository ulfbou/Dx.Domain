**Status:** Accepted
**Date:** 2026-01-17
**Updated:** 2026-04-23

### Context
Direct construction bypasses invariant enforcement and fragments creation authority.

### Decision
All domain objects in **S1 Domain, S2 Application, and S3 Infrastructure** must be created through controlled facade or factory entry points.

**S0 Kernel is explicitly excluded.** Kernel primitives form the substrate and must be publicly constructible.

---

### Amendment 2026-04-23 — Kernel Primitive Exemption
The following S0 types are exempt from DXA010/DXA011/DXA080:
- `Dx.Domain.Kernel.Result<T>`
- `Dx.Domain.Kernel.Result<TSuccess,TFailure>`
- `Dx.Domain.Kernel.DomainError`
- `Dx.Domain.Kernel.Invariant`
- `Dx.Domain.Kernel.Unit`
- `Dx.Domain.Primitives.*` (CorrelationId, TraceId, ActorId, FactId, SpanId, UserId)
- `Dx.Domain.Facts.Fact<TPayload>`, `Causation`

Rationale: You cannot require a facade to create the type used to return failures from the facade. This creates circular dependency and breaks NuGet consumption.

### Enforcement Coverage
- **[DXA010](../analyzers/DXA010.md)** — Direct construction detection (**Moderate**) — S1–S3 only
- **[DXA011](../analyzers/DXA011.md)** — Public factory exposure (**Moderate–Strong**) — S1–S3 only
- **[DXA080](../analyzers/DXA080.md)** — Facade invariant presence (**Moderate**) — S1 only

**Composite Coverage Level:** **Partial**

**Known Gaps:**
- Reflection
- Serialization
- Internal kernel misuse
- Transitive factory delegation

---

### Enforcement Model
- **Type:** Static analysis
- **Scope:** Assemblies with DxAssemblyRole S1, S2, S3
- **Strength:** Composite (Moderate overall)

---

### Bypass Vectors
- `Activator.CreateInstance`
- ORM materialization
- Internal/private constructors
- Delegated factory chains

---

### Guarantee
Construction entry points are controlled in S1–S3 consumer code.

**Not guaranteed at runtime.**

---

### Dependencies
- [**DXA080**](../analyzers/DXA080.md) required to preserve invariant intent
