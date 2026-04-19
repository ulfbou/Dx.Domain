# Dx.Domain

**Deterministic domain correctness for .NET 8, 9, and 10.**

Dx.Domain turns domain correctness into compile-time constraints enforced by analyzers and a minimal runtime kernel.

Dx.Domain is a compile-time enforced domain modeling system. If it compiles, passes analyzers, and the Kernel accepts it — the state satisfies all enforced constraints.

Enforcement applies only within statically analyzable scope and declared analyzer coverage.

**System model:** [SYSTEM.md](docs/SYSTEM.md) · **Normative spec:** [SPEC.md](docs/SPEC.md) · **Enforcement model:** [ENFORCEMENT_MODEL.md](docs/ENFORCEMENT_MODEL.md) · **Limitations:** [LIMITATIONS.md](docs/LIMITATIONS.md)

---

## What Dx.Domain is not

- A runtime validation framework
- A DDD opinionated toolkit
- A persistence or infrastructure layer

---

## Start Here

- **New to Dx.Domain** → [Dx.Domain in 90 seconds](docs/learn/index.md)
- **Want the guarantees** → [SPEC.md](docs/SPEC.md)
- **Evaluating enforcement** → [ENFORCEMENT_MODEL.md](docs/ENFORCEMENT_MODEL.md) + [Limitations](docs/LIMITATIONS.md)
- **Implementing** → [Getting Started](docs/learn/getting-started.md) → [Quickstart](docs/learn/quickstart.md)
- **Auditing decisions** → [ADR Index](docs/adr/index.md)

---

## Why Dx.Domain exists

Most domain bugs are silent: ignored results, implicit construction, temporal drift, mutable state. Dx.Domain moves those failures to build time.

- **Annotations** — semantic vocabulary, zero runtime cost
- **Primitives** — immutable identities
- **Kernel** — judges invariants, results, errors; no I/O
- **Facts** — append-only structural history
- **Analyzers** — enforce rules on every build in S1–S3 (DXA010–DXA080)

Rationale: [Manifesto](docs/MANIFESTO.md) · [Non-Goals](docs/NON_GOALS.md) · [DPI](docs/DPI.md)

---

## What is guaranteed

| Guarantee | Enforcement | Strength |
|-----------|-------------|----------|
| Result<T> must be handled | DXA020 | Strong |
| No direct construction in S1–S3 | DXA010/DXA011/DXA080 | Heuristic |
| UTC-only time | DXA050 | Strong |
| Kernel surface frozen | DXA040 | Strong |
| Result factories public in S0 | ADR-0018 exemption | Strong |

Strength is bounded by static analysis scope and declared coverage. See [ENFORCEMENT_MODEL.md](docs/ENFORCEMENT_MODEL.md).

Full normative rules: [SPEC.md](docs/SPEC.md)

---

## Packages

```bash
dotnet add package Dx.Domain.Kernel
dotnet add package Dx.Domain.Primitives
dotnet add package Dx.Domain.Facts
dotnet add package Dx.Domain.Annotations
```

- [Kernel](docs/public/packages/kernel.md) · [Primitives](docs/public/packages/primitives.md) · [Facts](docs/public/packages/facts.md) · [Annotations](docs/public/packages/annotations.md) · [Analyzers](docs/public/packages/analyzers.md)

Analyzers apply to S1–S3 only. S0 is exempt per ADR-0018.

---

## Stability

**Current:** `0.1.0-alpha.4`

| Area | Stability |
|------|-----------|
| Result<T> factories | High (frozen) |
| Kernel surface | High (frozen) |
| Primitives | Medium |
| Namespaces | Low |

Details: [STABILITY.md](docs/learn/STABILITY.md)

---

## Learn More

- **Architecture:** [SYSTEM.md](docs/SYSTEM.md) → [Architecture Overview](docs/public/architecture-overview.md)
- **When build fails:** [When the compiler fails](docs/when-the-compiler-fails.md)
- **Examples:** [Build a facade](docs/learn/guides/build-a-facade.md)
- **Full index:** [Documentation Index](docs/index.md)
