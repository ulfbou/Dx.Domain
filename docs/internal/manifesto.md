<!-- path: docs/internal/manifesto.md -->
---
id: manifesto
title: Dx.Domain Manifesto
status: Accepted
audience: Contributors
owners: [KernelOwner]
reviewers: [DocsLead]
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
applies_to:
  packages: [Dx.Domain.Kernel]
  layers: [Internal]
canonical: docs/internal/manifesto.md
related:
  - docs/internal/non-goals.md
  - docs/internal/governance/dpi.md
  - docs/internal/governance/kernel-law.md
tags: [manifesto, principles, constraints]
---

# Dx.Domain Manifesto

## A Declaration of Refusal

Dx.Domain exists to make **incorrect domain modeling impossible to ignore**.

This is not a convenience library.  
This is not a toolkit of helpers.  
This is a **line in the sand**.

The kernel is small, opinionated, and compiler‑assisted. Everything else
belongs at the edges.

See also:

- ./non-goals.md — what we permanently refuse to become.  
- ./governance/dpi.md — how every change is judged.

---

## What Dx.Domain Refuses to Tolerate

### Ambiguity
Every domain concept must be **explicit**: no magic strings, implicit defaults, or silent fallbacks.
If a value can be wrong, it must fail **loudly and deterministically**.

### Accidental Complexity
Eliminate boilerplate and leaky abstractions. Identifiers, invariants, errors, and results are **first‑class**.

### Runtime Guesswork
Correctness is **compiler‑assisted**. Prefer stronger types, local invariants, and analyzers/generators.

### Silent Failure
No hidden nulls or swallowed exceptions. Operations succeed or fail **explicitly** via structured results and errors.

### Incoherent Error Semantics
Errors are **structured, centralized, provenance‑aware** (`DomainError`, `Result<…>`), while invariant violations use diagnostic carriers (`InvariantError`, `InvariantViolationException`).

### Unteachable Architecture
Friction is intentional; misuse is blocked; ergonomics are **enforced, not suggested**.

---

## Kernel Rule: Mechanics vs. Semantics

> The kernel forbids semantic expansion, not mechanical support.

The kernel **may** include internal mechanical code to construct/enforce existing primitives (e.g., `TraceId.New()`, `Invariant.That(...)`, `Require` helpers, performance utilities, caller‑info capture) **without** adding domain vocabulary.

Such code is:

- `internal`,  
- non‑extensible by consumers,  
- free of workflow/policy/lifecycle semantics, and  
- contained within the kernel boundary.

### Forbidden: Semantic Helpers
Not allowed: domain‑naming helpers (`AggregateRoot`, `DomainEvent`, `CommandContext`), progression helpers (`Apply`, `Handle`, `When`, `TransitionTo`), policy encoders, or dispatch/publish/coordination helpers.

### Invariants and Requirements
Allowed as guardrails that **fail via kernel values** and add no semantics.

---

## What Dx.Domain Demands

- **Explicit Invariants** — executable contracts, not comments.  
- **Minimal, Frozen Kernel** — evolution happens at the **edges**.  
- **Errors as Values** — boundary‑safe failures, not exceptions.  
- **Architecture That Teaches** — illegal states are unrepresentable.

---

## Scope & Evolution

Spine and edges:

- `Dx.Domain` — invariants, results, errors, time, causation primitives.  
- `Dx.Domain.Values` — identities, primitives, enforced value objects.  
- `Dx.Domain.Analyzers` — compile‑time enforcement, migration pressure.  
- `Dx.Domain.Generators` — boilerplate reduction without semantic drift.  
- `Dx.Domain.Persistence.*` — adapters, never dependencies.

Names may evolve. **Principles do not.**  
Every change must answer:

> Does this uphold the refusal – or compromise it?
