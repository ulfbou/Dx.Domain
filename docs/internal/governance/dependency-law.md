<!-- path: docs/internal/governance/dependency-law.md -->
---
```yaml
id: dependency-law
title: Dependency Law (Role Matrix & Illegal Edges)
status: Accepted
audience: Maintainers
owners: [KernelOwner]
reviewers: [AnalyzersOwner, ReleaseManager]
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
applies_to:
  packages: [Dx.Domain.Kernel, Dx.Domain.Annotations, Dx.Domain.Analyzers, Dx.Domain.Primitives, Dx.Domain.Facts]
  layers: [Internal]
canonical: docs/internal/governance/dependency-law.md
related:
  - docs/internal/specs/kernel.refactorization.specification.md
  - docs/internal/governance/kernel-law.md
  - docs/internal/governance/analyzers-law.md
tags: [governance, dependencies, roles, matrix]
```
---

# Dependency Law (Role Matrix & Illegal Edges)

**Purpose.** Enforce a strict architectural graph so rules, invariants, and analyzers remain sound and portable.

## Role matrix (summary)
- **Annotations (Abstractions)** → No dependencies on Kernel/Analyzers.  
- **Kernel** → May depend on **Annotations**; **must not** depend on Analyzers.  
- **Analyzers** → Depend on **Annotations**; analyze Kernel and user code but are not a runtime dependency.  
- **Primitives/Facts** → Must not introduce cycles; follow the same directionality away from Analyzers.  
- **User code** → May depend on Kernel/Annotations; never on Analyzers at runtime.

## Illegal edges (examples)
- Annotations → Kernel (forbidden)
- Kernel → Analyzers (forbidden)
- Any → circular reference (forbidden)

## Enforcement
- Static checks (DXK* family) must fail builds on illegal edges.
- CI validates project references and NuGet dependencies align with the role matrix.

## Acceptance criteria
- No DXK* diagnostics in CI for approved edges.
- No project or package references violate the matrix.
- ADR recorded if the matrix needs expansion; default is deny.

**References:**  
- Kernel Spec → `docs/internal/specs/kernel.refactorization.specification.md`  
- Kernel Law → `docs/internal/governance/kernel-law.md`  
- Analyzers Law → `docs/internal/governance/analyzers-law.md`
