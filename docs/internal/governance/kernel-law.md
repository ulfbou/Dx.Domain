<!-- path: docs/internal/governance/kernel-law.md -->
---
```yaml
id: kernel-law
title: Kernel Law
status: Accepted
audience: Maintainers
owners: [KernelOwner]
reviewers: [AnalyzersOwner, ReleaseManager]
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
applies_to:
  packages: [Dx.Domain.Kernel]
  layers: [Internal, Kernel]
canonical: docs/internal/governance/kernel-law.md
related:
  - docs/internal/specs/kernel.refactorization.specification.md
  - docs/internal/governance/api-freeze.md
  - docs/internal/rules/analyzers/rule-catalog.md
tags: [kernel, law, invariants, results, utc, api-freeze]
```
---

# Kernel Law

**Purpose** — Define the **non‑negotiable rules** that govern the `Dx.Domain.Kernel` assembly: scope, allowed behavior, time model, error/result semantics, diagnostics, ABI stability, and public API freeze. These laws preserve a **small, frozen, mechanical kernel** that judges values and **never** coordinates infrastructure.

---

## 1) Scope

The Kernel is the **runtime judge** for validity and failure. It provides:

- **Result algebra** and **error model** (`Result<…>`, `DomainError`) as immutable values.  
- **Executable invariants** (`Invariant.That`) that fail with **diagnostics‑as‑data** (`InvariantError`) and throw only `InvariantViolationException` for *programming* defects.  
- Minimal **time primitive** (`DomainTime`) with a **UTC‑only** contract.

> The Kernel **does not** include I/O, logging, DI, orchestration, policies, persistence, or transport concerns.

---

## 2) Laws (Normative)

### 2.1 Kernel Purity — *No IO, No Logging, No Ambient Context*
Kernel code must not perform I/O, logging, networking, persistence, or access ambient context (`HttpContext`, thread‑local, global singletons). All context is explicit and immutable.

- **Implication**: No infra dependencies (ORMs, ASP.NET, HTTP, EF Core). **DXK003** flags violations.  
- **Acceptance**: CI rejects PRs introducing forbidden namespaces or references.

### 2.2 Diagnostics‑as‑Data
Invariant violations must produce **structured diagnostics** (`InvariantError`) carrying **code, message, member, file, line, UTC timestamp**. Kernel never logs.

- **Mechanics**: `Invariant.That(...)` → `InvariantViolationException.Create(...)` → wraps `InvariantError` with caller info and UTC.  
- **Catalog**: Error codes live in an **append‑only** catalog; never repurposed.

### 2.3 Result Law
**Failures are values**. Kernel extensions (`Map/Bind/Tap/Ensure/Recover/Match/…`) are **pure**; they may throw **only** `InvariantViolationException` for programming errors (e.g., null delegates).

> For consumers, **DXA022** forbids throwing domain control exceptions from `Result`‑returning methods (Kernel is definitional and exempt). See the ../rules/analyzers/rule-catalog.md.

### 2.4 Time Model — UTC‑Only
Kernel represents time **only as UTC**. `DomainTime.From(DateTimeOffset utc)` asserts `Offset == 0`. `DomainTime.Now()` uses `DateTimeOffset.UtcNow`. **No clocks/abstractions** are introduced in Kernel.

### 2.5 Primitive Law (interaction)
Kernel‑visible primitives are **immutable** and enforce invariants at construction. Identity primitives are **`readonly struct`**, with guarded construction, no implicit casts, and `IParsable<T>/ISpanFormattable` when applicable.

### 2.6 ABI Stability & Public API Freeze
Kernel public surface is **frozen** by default. Any new public API requires **DPI‑aligned justification** and **baseline approval**.

- **Enforcement**: **DXA040** flags unapproved public API.  
- **Process**: See ./api-freeze.md.

### 2.7 Public API Freeze — Operational Steps
1) Update **API baseline** (e.g., `PublicAPI.Shipped.txt`) and include diff in PR.  
2) Annotate new public symbols with justification (e.g., `[ApprovedKernelApi("...")]`) and link the proposal.  
3) CI runs **API diff** + **DXA040**; PR fails if unapproved.  
4) **Approvals**: Kernel Owner + Analyzers Owner + Release Manager.

> Non‑goal: Do not add public “convenience” facades in Kernel.

### 2.8 Error Code Canon
Error codes are **stable, namespaced, append‑only**. Use `DomainError.Create(code, message)`; enrich with immutable metadata.

### 2.9 Assembly Dependency Law
Dependencies are **strict**:

- **Kernel → Annotations** (consume markers/contracts). **Never** depend on Analyzers.  
- **Analyzers → Annotations** (never runtime‑coupled to Kernel).  
- **No circular dependencies**. **DXK002**/**DXK007** enforce illegal edges.

---

## 3) Reference Examples

### Invariant & Diagnostics

```csharp
Invariant.That(
    utc.Offset == TimeSpan.Zero,
    "DomainTime.Invariant.Utc",
    "DomainTime must be UTC.");
````

### Result Extension (purity)

```csharp
public static Result<TOut> Map<TIn,TOut>(this Result<TIn> r, Func<TIn,TOut> map)
    where TIn : notnull where TOut : notnull
{
    Invariant.That(map is not null, "ResultMap.Parameter.CannotBeNull",
        $"Parameter '{nameof(map)}' cannot be null.");
    if (r.IsFailure) return Result.Failure<TOut>(r.Error);
    try { return Result.Success(map(r.Value)); }
    catch (Exception ex) {
        throw InvariantViolationException.Create("Result.Map.Exception",
            "An exception occurred while mapping the result.", ex);
    }
}
```

### UTC‑Only Time (construction)

```csharp
public readonly record struct DomainTime
{
    public DateTimeOffset Utc { get; }
    private DomainTime(DateTimeOffset utc) => Utc = utc;

    public static DomainTime Now() => new(DateTimeOffset.UtcNow);

    internal static DomainTime From(DateTimeOffset utc)
    {
        Invariant.That(utc.Offset == TimeSpan.Zero, "DomainTime.Invariant.Utc", "DomainTime must be UTC.");
        return new DomainTime(utc);
    }
}
```

***

## 4) Compliance Checklist (PR Gate)

*   [ ] **Purity**: No I/O/logging/ambient context introduced.
*   [ ] **Diagnostics**: `InvariantError` includes code/message/member/file/line/UTC.
*   [ ] **Results**: No throwing for domain outcomes; failures are values.
*   [ ] **Time**: UTC‑only; no clocks.
*   [ ] **Primitives**: Immutable; no public inheritance points.
*   [ ] **Error Codes**: New codes added to registry (append‑only).
*   [ ] **Dependencies**: Role matrix satisfied; no illegal references.
*   [ ] **API Freeze**: Baseline unchanged or justified + approved; **DXA040** clean.

***

## 5) Acceptance Criteria

1.  No `DXK00*`, `DXA01*`, `DXA02*`, or `DXA04*` diagnostics in Kernel at build time.
2.  No public surface delta unless justified and approved; **DXA040** passes.
3.  All guard paths use `Invariant.That(...)` and include UTC diagnostics.
4.  Non‑UTC `DomainTime` construction fails tests and invariants.
5.  No **DXK002/DXK007** dependency violations.

***

**See also**:

*   Normative spec — ../specs/kernel.refactorization.specification.md
*   Analyzer rules — ../rules/analyzers/rule-catalog.md
