# Dx.Domain Kernel Admission Test

This checklist defines the **non-negotiable conditions** for any change to the Dx.Domain foundations.

It is derived from `docs/kernel.refactorization.specification.md` and is **normative** for:

- `Dx.Domain.Abstractions` (Vocabulary)
- `Dx.Domain` (Kernel)
- `Dx.Domain.Analyzers` (Tooling)

Any PR touching these assemblies **must** satisfy this document.

---

## 1. Assembly & Dependency Jurisdiction

For every PR, verify:

- **Abstractions** (`Dx.Domain.Abstractions`):
  - [ ] Does **not** reference `Dx.Domain` (Kernel) or `Dx.Domain.Analyzers`.
  - [ ] Contains only:
    - Marker interfaces
    - Attributes
    - Metadata records
    - Diagnostic canon (`DxRuleIds`, `DxCategories`, `DxSeverities`)

- **Kernel** (`Dx.Domain`):
  - [ ] References `Dx.Domain.Abstractions`.
  - [ ] Does **not** reference analyzers, generators, or infrastructure libraries.

- **Analyzers** (`Dx.Domain.Analyzers`):
  - [ ] References `Dx.Domain.Abstractions` (and Roslyn/runtime) only.
  - [ ] Does **not** introduce types that Abstractions must depend on.

---

## 2. Abstractions Purity Test (Vocabulary-Only)

For any change under `Dx.Domain.Abstractions`:

- [ ] No methods with control flow (`if`, `switch`, loops, `try/catch`, `yield`).
- [ ] No exceptions, guards, or validation helpers.
- [ ] No identity value types, `Result` types, or other runtime primitives.
- [ ] No extension methods.
- [ ] All attributes are:
  - [ ] `sealed`
  - [ ] Parameterised only by primitives (`string`, `bool`, `enum`).
- [ ] All metadata types are `record`(-like) and:
  - [ ] Immutable
  - [ ] Contain **no** runtime behavior (no reflection, no lazy evaluation).

Abstractions are **0% logic, 100% metadata**.

---

## 3. Kernel Scope Test (The Judge Only)

For any change in `Dx.Domain` (Kernel):

- [ ] No ambient context (`HttpContext`, thread-local, static global state, service locators).
- [ ] No I/O, persistence, networking, or logging.
- [ ] No orchestration or policies (no retries, backoff, telemetry, metrics).
- [ ] Public primitives are effectively final:
  - `sealed` classes or `readonly struct`s
  - No public base types intended for external inheritance.

Kernel code **judges values**; it never coordinates infrastructure.

---

## 4. Facade Exclusion Rule

Developer-friendly facades **must not** live in the Kernel.

- [ ] `Dx.Domain` contains **no** ergonomic facades (e.g. `DxDomain` static entry points).
- [ ] Any convenience or sugar API lives in an **outer** assembly (e.g. `Dx.Domain.Runtime`, `Dx.Domain.Extensions`).
- [ ] Outer facades may depend on Kernel and Abstractions, but **never** the other way around.

Kernel exposes:

- Core primitives (identities, `Result`, `Fact<TPayload>`, `Causation`, `TransitionResult<TState>`, error types).
- Judges (`Invariant.That`, `Require.That`).
- Pure functional extensions (`Map`, `Bind`, `Tap`, `Ensure`).

---

## 5. Identity Admission Test

For each identity primitive (`ActorId`, `TraceId`, `CorrelationId`, `SpanId`, `FactId`, or any new identity type):

- [ ] Implemented as a `readonly struct`.
- [ ] No public constructors; creation is guarded via internal factories (e.g. `InternalNew`, `InternalFrom`).
- [ ] Implements `IIdentity` from `Dx.Domain.Abstractions`.
- [ ] Implements `IParsable<T>` and `ISpanFormattable` where applicable.
- [ ] No implicit conversions to or from primitive types (`Guid`, `string`, etc.).
- [ ] Uses only **allowed** generators:
  - `Guid.NewGuid()` and/or cryptographic randomness.
  - No time-based, sequential, or business-semantic generation.

---

## 6. Result Algebra Test

For `Result`, `Result<T>`, `Result<T, TError>` and their extensions:

- [ ] No implicit cast operators.
- [ ] Failures are represented as values (`DomainError` / `TError`), not by throwing.
- [ ] All `Map`, `Bind`, `Tap`, `Ensure`, etc. are:
  - Pure
  - Side-effect-free
  - Free of ambient state and I/O

Result is the exclusive flow-control mechanism inside the Kernel.

---

## 7. Invariant & Require Test

For `Invariant`, `Require`, and any new enforcement helpers:

- [ ] `Invariant.That(...)`:
  - [ ] Throws **only** `InvariantViolationException` on failure.
  - [ ] Always constructs an `InvariantError` containing code, message, member, file, line, and correlation context.
- [ ] `Require.That(...)` and friends:
  - [ ] Never throw for normal failures; they return `Result.Failure(...)`.
  - [ ] Use `DomainError` and canonical codes from `DxDomain.Codes`.
- [ ] No logging or side effects in either path.

Diagnostics are **data**, not logs.

---

## 8. Error & Codes Canon Test

For `DomainError`, `InvariantError`, and `DxDomain.Codes`:

- [ ] Errors are immutable and contain no side effects.
- [ ] No logging or environment access in error construction.
- [ ] `DxDomain.Codes` is a **pure** catalog of constants, logically grouped (e.g. `Invariant`, `Domain`, `Common`, `Validation`).
- [ ] New codes follow the existing naming scheme and are treated as stable identifiers.

Error codes are part of the public canon and must not be silently repurposed.

---

## 9. Fact System Admission Test

For `Fact<TPayload>`, `Causation`, `TransitionResult<TState>` and related types:

- [ ] Types are structural holders of history and causation, not behavior.
- [ ] No event dispatch, messaging, or integration logic is present in Kernel.
- [ ] Construction is guarded via internal factories; no public mutable constructors.
- [ ] Facts are explicitly **not** domain events.

---

## 10. Analyzer Contract Test

For any change that affects attributes, marker interfaces, or metadata consumed by analyzers:

- [ ] Contracts (`[AggregateRoot]`, `[ValueObject]`, `IdentityMetadata`, etc.) live in `Dx.Domain.Abstractions`.
- [ ] Kernel merely consumes these; it does not redefine or shadow them.
- [ ] New rules reflected in:
  - `DxRuleIds`
  - `DxCategories`
  - `DxSeverities`
- [ ] Analyzers can enforce at least:
  - Factory-only construction
  - Result usage discipline
  - Identity and immutability rules
  - Vocabulary purity (no Kernel semantics in Abstractions)

---

## 11. Mandatory Review Acknowledgement

Every PR touching Kernel, Abstractions, or Analyzers **must** include:

- [ ] A short note in the PR description linking to this file.
- [ ] An explicit statement by the author confirming that this checklist was reviewed and all applicable items are satisfied.

If any item cannot be satisfied, the PR must:

- Clearly document the deviation, and
- Mark it as a **spec defect** to be fixed, not as a new precedent.
