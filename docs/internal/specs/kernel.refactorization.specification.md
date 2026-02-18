<!-- path: docs/internal/specs/kernel.refactorization.specification.md -->
---
id: kernel-abstractions-v1-0-authoritative
title: Dx.Domain.Kernel Refactorization Specification
status: Accepted
audience: Maintainers
owners: [KernelOwner]
reviewers: [AnalyzersOwner]
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
applies_to:
  packages: [Dx.Domain.Kernel, Dx.Domain.Annotations]
  layers: [Internal]
canonical: docs/internal/specs/kernel.refactorization.specification.md
related:
  - docs/internal/governance/kernel-law.md
  - docs/internal/governance/dependency-law.md
  - docs/internal/governance/non-silence-axiom.md
tags: [spec, kernel, abstractions, normative]
---

# Dx.Domain.Kernel Refactorization Specification

## Kernel & Abstractions v1.0 (Authoritative)

---

## 0. Status of This Document

This specification is **normative**.

- Any implementation claiming compliance with Dx.Domain **must** conform to this document.
- Any deviation is a defect, not an interpretation difference.
- Kernel and Abstractions are considered **foundational infrastructure** and are expected to remain stable across major framework evolution.

---

## 1. Core Philosophy: The Substrate of Correctness

Dx.Domain does **not** model business logic.  
It provides the **substrate upon which incorrect business logic is structurally difficult to express**.

> **If it compiles, passes analyzers, and the Kernel accepts it, the state is valid.**

Separation of concerns:

- **Dx.Domain.Abstractions** — *semantic vocabulary* for intent and invariants.  
- **Dx.Domain.Kernel** — *runtime judgment* for validity and failure.  
- **Dx.Domain.Analyzers** — design‑time enforcement (out of scope for this spec).

---

## 2. Package Boundaries & Dependency Model

### 2.1 Canonical Dependency Graph (Corrected)

```text
      [Dx.Domain.Abstractions] <------- (Implements / Uses) -----------------+
             ^        ^                                                      |
             |        |                                                      |
    (Analyzes)        +---------------------+                                |
             |                              |                                |
   [Dx.Domain.Analyzers]           [Dx.Domain.Kernel]                        |
             ^                              ^                                |
             |                              |                                |
             +--------- (Analyzes) ---------+                                |
                                            |                                |
                                     [User Domain Code] -- (Depends on) -----+
````

### Dependency Rules (Strict)

*   **Dx.Domain.Abstractions** — **no dependencies** on Kernel, Analyzers, or user code.
*   **Dx.Domain.Kernel** — **must** depend on Abstractions; **must not** depend on Analyzers.
*   **Dx.Domain.Analyzers** — depend on Abstractions; may analyze Kernel and user code; never a runtime dependency.
*   **User Domain Code** — depends on Kernel and Abstractions; never on Analyzers at runtime.

### Packaging vs. Assembly Clarification

A **meta‑package** may bundle Kernel + Abstractions + Analyzers; **assemblies remain decoupled**:

*   `Dx.Domain.Kernel.dll` never references analyzers.
*   Analyzer assemblies are compiler‑only artifacts.

***

## 3. Dx.Domain.Abstractions — *The Vocabulary*

### 3.1 Purpose

Defines **semantic intent** for compiler, analyzers, and generators. It encodes *meaning*, not behavior.

### 3.2 Hard Constraints

Abstractions **must not contain** runtime logic, control flow, extension methods, exceptions, validation helpers, identity value types, result types, or time/randomness/environment access. Abstractions are **pure metadata**.

### 3.3 Assembly Contents (Authoritative)

*   Marker interfaces (no members, no inheritance): `IAggregateRoot`, `IEntity`, `IValueObject`, `IDomainEvent`, `IDomainPolicy`, `IDomainFactory`, `IIdentity`.
*   Attributes (sealed, primitive parameters only): `[AggregateRoot]`, `[Entity]`, `[ValueObject]`, `[DomainEvent]`, `[Invariant]`, `[Identity]`, `[Factory]`, `[Policy]`.
*   Metadata records (immutable, no reflection/lazy behavior).
*   Diagnostic canon: `DxRuleIds`, `DxCategories`, `DxSeverities` — **versioned and append‑only**.

***

## 4. Dx.Domain.Kernel — *The Judge*

Purpose: **exclusive runtime authority** for validity, failure, identity, invariants, transitions, and structural history — without infrastructure concerns.

*   **No ambient context** (global state, thread‑locals, `HttpContext`, service locators).
*   **Restricted generation** (no business semantics; GUID/crypto only).
*   **Diagnostics‑as‑data** (`InvariantError`), **functional purity**, **final types**, **no policies**.

### Assembly Contents

*   Identity primitives (readonly structs; guarded construction; no implicit casts; `IParsable<T>` / `ISpanFormattable`).
*   Result algebra: `Result`, `Result<T>`, `Result<T, TError>` (failures as values; no implicit casts).
*   Error model: `DomainError`, `InvariantError` (immutable; no logging).
*   Invariant enforcement: `Invariant.That(...)` (panic) / `Require.That(...)` (recoverable failure).
*   Fact system: `Fact<TPayload>`, `Causation`, `TransitionResult<TState>` (structural; not domain events).
*   Functional extensions: `Map`, `Bind`, `Tap`, `Ensure` (pure, explicit).

***

## 5. Tooling Contract (Analyzer Alignment)

Analyzers consume **Abstractions** and analyze **Kernel + User Code** to enforce immutability, factory‑only construction, result discipline, and vocabulary purity. They remain **external judges** (no runtime coupling).

***

## 6. Final Assessment

With the corrected dependency graph and Kernel → Abstractions linkage:

*   The architecture is sound; contracts are enforceable; runtime purity is preserved; tooling authority is externalized.

This is the **authoritative** definition for v1.0.
