# Dx.Domain Non‑Goals

## A Declaration of Permanent Refusal

Dx.Domain is defined as much by what it **excludes** as by what it enforces.  
These are not “out of scope for now.” They are **structural refusals** that
protect the kernel described in the [Manifesto](MANIFESTO.md).

If a proposal conflicts with these non‑goals, it is not a feature request –
it is a request to change the project’s identity.

---

## What Dx.Domain Will Never Be

### 1. A General‑Purpose Utility Library

Dx.Domain will not accumulate helpers, extensions, or convenience methods.

If a construct does not **enforce domain correctness**, invariants, or
semantic clarity, it does not belong. Prefer:

- Application libraries
- Framework or adapter layers
- Dedicated utility packages

over putting “useful” helpers in the core.

Note: The kernel may contain **internal mechanical support** (e.g., value factories like `TraceId.New()`, invariant/require helpers, caller‑info capture, perf utilities) that exist solely to construct or enforce existing primitives. These are not consumer APIs, do not add domain meaning, and must not escape the kernel boundary.

### 2. A DDD Pattern Museum

Dx.Domain does not re‑implement aggregates, repositories, factories, or
services as textbook abstractions.

Patterns are **emergent consequences of constraints**, not APIs to be
memorized. The kernel provides primitives (identifiers, invariants, results,
errors) that *shape* aggregates and services in your codebase – it does not
ship canonical implementations.

### 3. A Persistence Framework

Databases are replaceable. Serialization formats are replaceable.
Invariants are **not**.

Persistence exists only as **adapters**, never as a design center:

- No ORM surface in `Dx.Domain`.
- No storage‑driven entity models.
- No coupling between domain primitives and specific databases or transport
  stacks.

See the scope notes in the [Manifesto](MANIFESTO.md#scope--evolution).

### 4. A Runtime‑First Safety Net

Correctness is not deferred to execution.

Runtime validation exists only where the compiler cannot reach – and that
boundary must be **explicit and minimal**. Prefer:

- Stronger types over `if`‑chains.
- Invariants expressed once, close to the type.
- Compile‑time analyzers and generators over ad‑hoc checks.

If a feature primarily adds runtime checks that the type system could
enforce, it does not belong.

(Clarification: `Invariant` and `Require` helpers are permitted because they enforce already‑declared rules and fail via kernel values; they do not add semantics.)

### 5. A Convenience Layer

Dx.Domain does not optimize for “easy.”

Ergonomics serve **correctness**, never the reverse. If a shortcut weakens
guarantees, it is rejected outright – even if it looks pleasant at the call
site.

APIs are allowed to be opinionated and slightly “sharp” if that sharpness
prevents incoherent models or error semantics.

### 6. A Kitchen Sink

No creeping scope. No “while we’re here.”

Time, causation, invariants, results, values – this is the **spine** of
Dx.Domain. Everything else lives **outside** or **on top**:

- Edge adapters (persistence, transport)
- Analyzers and generators
- Application‑specific layers

---

## Why These Non‑Goals Exist

The non‑goals:

- Prevent drift from a small, frozen kernel into a bloated framework.
- Prevent accidental authority over concerns that should stay at the edges.
- Keep the project focused on **domain correctness**, not convenience.

Before adding any new surface, ask:

> Does this push more correctness into the type system and invariants –
> or does it quietly expand the scope of the kernel?

If it is the latter, the change belongs elsewhere.
