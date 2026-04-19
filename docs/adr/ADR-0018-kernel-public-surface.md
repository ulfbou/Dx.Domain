## ADR-0018: Kernel Public Surface Contract

**Status:** Accepted
**Date:** 2026-04-23

### Context
ADR-0003 and ADR-0008 created ambiguity about which Kernel types are publicly constructible from NuGet. Construction Authority must not apply to the substrate itself.

### Decision
The following S0 types form the immutable public contract for Dx.Domain 0.x and 1.x. They must remain publicly constructible via static factories, and are exempt from DXA010, DXA011, and DXA080:

| Type | Public Factories | Notes |
|------|------------------|-------|
| Result<T> | Success(T), Failure(DomainError) | Primary domain result |
| Result<TSuccess,TFailure> | Success(TSuccess), Failure(TFailure) | Low-level primitive |
| DomainError | Create(string code, string message, object? context = null) | Structured error |
| Unit | Value | Void equivalent |
| Invariant | That(bool condition, DomainError error) | Precondition check |
| Primitives (CorrelationId, TraceId, ActorId, FactId, SpanId, UserId) | FromGuid, New, Empty | Immutable identities |
| Fact<TPayload>, Causation | Create(...) | Structural history |

No facade is required to use these types. They are the building blocks for facades.

---

### Enforcement Coverage
**Enforced by:**
- [DXA040](../analyzers/DXA040.md) — Kernel surface freeze
- DXA010/DXA011 scope check for S0 exemption

**Coverage Level:** Strong

**Known Gaps:** None

---

### Enforcement Model
- **Type:** Design decision + static analysis
- **Scope:** S0 assemblies
- **Strength:** Strong

---

### Bypass Vectors
- None — public surface is intentional

---

### Guarantee
NuGet consumers can instantiate Results and Errors without a facade. Breaking this contract requires a new ADR and major version bump.

### Dependencies
- [ADR-0003](../adr/ADR-0003-dxa010-warning.md)
- [ADR-0004](../adr/ADR-0004-result-struct.md)
- [ADR-0008](../adr/ADR-0008-dxa011-public-factory-exposure.md)

### Consequences
**Positive:** Unblocks NuGet publish, resolves circular dependency, clarifies substrate boundary
**Negative:** Slightly larger public surface to maintain
**Trade-off accepted:** Usability over theoretical purity

### DPI Alignment
- Item 3: Increases compiler-assisted correctness
- Item 7: No semantic expansion beyond substrate
