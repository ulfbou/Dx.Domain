<!-- path: docs/internal/non-goals.md -->
---
id: non-goals
title: Dx.Domain Non‑Goals
status: Accepted
audience: Maintainers
owners: [KernelOwner]
reviewers: [DocsLead]
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
applies_to:
  packages: [Dx.Domain.Kernel]
  layers: [Internal]
canonical: docs/internal/non-goals.md
related:
  - docs/internal/manifesto.md
  - docs/internal/governance/kernel-law.md
tags: [governance, scope, exclusions]
---

# Dx.Domain Non‑Goals

## A Declaration of Permanent Refusal

Dx.Domain is defined as much by what it **excludes** as by what it enforces.  
These are not “out of scope for now.” They are **structural refusals** that
protect the kernel described in the ./manifesto.md.

If a proposal conflicts with these non‑goals, it is not a feature request –
it is a request to change the project’s identity.

---

## What Dx.Domain Will Never Be

### 1. A General‑Purpose Utility Library
Dx.Domain will not accumulate helpers, extensions, or convenience methods.

If a construct does not **enforce domain correctness**, invariants, or
semantic clarity, it does not belong. Prefer application libraries, adapter layers,
or dedicated utility packages over putting “useful” helpers in the core.

> The kernel may contain **internal mechanical support** (e.g., value factories like `TraceId.New()`,
> invariant/require helpers, caller‑info capture, perf utilities) solely to construct/enforce primitives.
> These are not consumer APIs, add no domain meaning, and must not escape the kernel boundary.

### Forbidden: Semantic Helpers
Not allowed, even if implemented as helpers or utilities:

- Types or methods that introduce new domain concepts (`AggregateRoot`, `DomainEvent`, `CommandContext`)  
- Helpers implying workflow/progression (`Apply`, `Handle`, `When`, `TransitionTo`)  
- Policy‑encoding helpers (“success must produce facts”)  
- Dispatch/publishing/coordination helpers

If a helper’s name describes *business meaning* rather than *mechanical action*, it does not belong in the kernel.

### 2. A DDD Pattern Museum
No re‑implementation of aggregates/repositories/services as textbook abstractions.
Patterns should be **emergent** from constraints (identifiers, invariants, results, errors), not shipped as APIs.

### 3. A Persistence Framework
Databases and formats are replaceable; **invariants are not**. Persistence is **adapter‑only**:

- No ORM surface in `Dx.Domain`.  
- No storage‑driven entity models.  
- No coupling between domain primitives and specific databases or transports.

See ./manifesto.md#scope--evolution.

### 4. A Runtime‑First Safety Net
Correctness is not deferred to execution. Prefer stronger types, single‑point invariants, and compile‑time analyzers/generators.

(Clarification: `Invariant`/`Require` are allowed because they enforce declared rules and fail via kernel values; they do not add semantics.)

### 5. A Convenience Layer
Ergonomics serve **correctness**, never the reverse. Sharp APIs that prevent incoherent models are acceptable.

### 6. A Kitchen Sink
No creeping scope. Spine only: time, causation, invariants, results, values. Everything else stays **outside** or **on top** (adapters, analyzers, generators, app‑specific layers).

---

## Why These Non‑Goals Exist

To prevent drift into a bloated framework, accidental authority, and a loss of focus on **domain correctness**.

Before adding surface, ask:

> Does this push more correctness into the type system and invariants – or quietly expand kernel scope?

If the latter, it belongs elsewhere.
