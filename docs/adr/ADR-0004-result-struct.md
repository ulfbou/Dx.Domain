## ADR-0004: Result<T> Uses Struct Not Class


**Status:** Accepted  
**Date:** 2026-01-18  
**Updated:** 2026-04-23
**Applies to:** Dx.Domain.Kernel

### Context
`Result<T>` represents success or failure. Implementation choice: struct (value type) or class (reference type).

Struct advantages: no heap allocation, cannot be null, better performance for hot paths.
Class advantages: can be null, inheritance possible, familiar.

### Decision
`Result<T>` will be implemented as readonly struct with **public static factories**.

Rationale:
- `Result` should never be null — null represents absence of result, not failure
- Performance critical for domain operations
- Aligns with other primitives being structs
- Prevents null reference exceptions
- Public factories are required for NuGet usability

Implementation:
```csharp
public readonly struct Result<T>
{
    internal Result(T value) { ... }
    internal Result(DomainError error) { ... }
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(DomainError error) => new(error);
}
```

---

### Amendment 2026-04-23 — Public Surface Requirement
Constructors remain `internal`. Public API is the two static methods only. This satisfies both immutability (ADR-0005) and usability. DXA011 exemption for S0 is required (see ADR-0003).

### Enforcement Coverage
**Enforced by:**
- **Compiler** — struct cannot be null
- **[DXA040](../analyzers/DXA040.md)** — prevents changing to class without ADR

**Coverage Level:** **Strong**

**Known Gaps:**
- Boxing when cast to interface
- Large T increases struct size

---

### Enforcement Model
- **Type:** Language + static analyzer
- **Scope:** Kernel
- **Strength:** Strong

---

### Bypass Vectors
- Reflection-based instantiation
- Unsafe code

---

### Guarantee
Result values are never null in statically typed code.

**Does NOT guarantee:**
- performance for large T

---

### Dependencies
None

### Consequences
**Positive:** No heap allocations, cannot pass null, better performance
**Negative:** Larger size for large T, no inheritance, possible boxing
**Trade-off accepted:** Performance and null-safety over flexibility

### Alternatives Considered
1. Class — rejected (allows null)
2. Abstract base hierarchy — rejected (heap, complexity)
3. External library — rejected (dependency)

### DPI Alignment
- Item 3: Increases compiler-assisted correctness
- Item 2: Reduces accidental complexity

---
