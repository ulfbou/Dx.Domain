# Dx.Domain Manifesto

## A Declaration of Refusal

Dx.Domain exists to make **incorrect domain modeling impossible to ignore**.

This is not a convenience library.  
This is not a toolkit of helpers.  
This is a **line in the sand**.

The kernel is small, opinionated, and compiler‑assisted. Everything else belongs at the edges.

See also:

- [Non‑Goals](NON_GOALS.md) – what we permanently refuse to become.
- [Design Pressure Index](DPI.md) – how every change is judged.

---

## What Dx.Domain Refuses to Tolerate

### Ambiguity

Every domain concept must be **explicit**.

- No magic strings.  
- No implicit defaults.  
- No silent fallbacks.

If a value can be wrong, it must fail **loudly and deterministically**.

### No Magic Strings

We do not use strings to drive logic, identify states, or configure behavior in an ad-hoc manner.

- **Refusal:** No `if (status == "Active")`. No string-based reflection for core logic.
- **Demand:** Use strongly-typed identifiers or enums for state.
- **Clarification:** Canonical identifiers expressed as strings (such as error codes or fact types) are permitted only when they are centralized, validated, and treated as semantic identifiers rather than ad-hoc literals. They must represent a unique identity in the global domain space, not a local variable.

### Accidental Complexity

Boilerplate, ceremony, and leaky abstractions are eliminated.

Identifiers, invariants, errors, and results are **first‑class**, never
ad‑hoc utilities sprinkled across the codebase.

### Runtime Guesswork

Correctness is **compiler‑assisted**.

If the compiler cannot help prove safety, the API should not exist. Prefer:

- Stronger types over defensive conditionals.  
- Invariant enforcement close to the type.  
- Analyzers and generators over manual checklists.

### Silent Failure

No hidden nulls. No swallowed exceptions. No “best effort” semantics.

Operations either succeed **explicitly** or fail **explicitly** via
structured results and errors.

### Incoherent Error Semantics

Errors are **structured, centralized, and provenance‑aware**.

- No scattered string messages.  
- No improvised codes.  
- No ambiguity about origin or intent.

Domain failures use `DomainError` and `Result<…>`; invariant violations use
diagnostic carriers such as `InvariantError` and `InvariantViolationException`.

### Unteachable Architecture

Friction is intentional.

Misuse is blocked. Correct usage is guided. Ergonomics are **enforced, not
suggested**.

The design itself should act as pedagogy: developers learn the right
patterns by being **unable to express the wrong ones**.

---

## Kernel Rule: Mechanics vs. Semantics

> The kernel forbids semantic expansion, not mechanical support.

Dx.Domain refuses to add new domain meaning or architectural surface to the kernel.  
It does, however, permit internal, mechanical code that enforces or constructs existing primitives safely and efficiently.

### Allowed: Mechanical Support Code

The Dx.Domain kernel **explicitly allows** internal code whose purpose is *mechanical*, not *semantic*.

Mechanical support code:

- has no independent domain meaning  
- does not introduce new nouns into the model  
- does not encode workflows, policies, or lifecycles  
- exists solely to construct, validate, or enforce existing primitives

Examples of allowed mechanical support include:

- Internal factory methods for value creation  
  (e.g. `TraceId.New()`, `SpanId.New()`)

- Internal invariant enforcement helpers  
  (e.g. `Invariant.That(...)`)

- Internal requirement helpers that return existing primitives  
  (e.g. `Require.NotNull`, `Require.That`)

- Performance‑oriented utilities  
  (e.g. `AggressiveInlining`, `stackalloc`, bit‑level operations)

- Caller‑info capture for diagnostics  
  (`CallerMemberName`, `CallerFilePath`, etc.)

Mechanical support code may include low‑level or performance‑oriented implementations, provided they do not alter observable semantics.

Such code:

- must be `internal`  
- must not introduce new semantic concepts  
- must not be extensible by consumers  
- must not escape the kernel boundary

Mechanical helpers are implementation detail, not part of the domain vocabulary.

### Forbidden: Semantic Helpers

The following are **not allowed**, even if implemented as helpers or utilities:

- Types or methods that introduce new domain concepts  
  (e.g. `AggregateRoot`, `DomainEvent`, `CommandContext`)

- Helpers that imply workflow or progression  
  (e.g. `Apply`, `Handle`, `When`, `TransitionTo`)

- Helpers that encode policy decisions  
  (e.g. “success must produce facts”)

- Helpers that perform dispatch, publishing, or coordination

If a helper can be named meaningfully by a domain expert, it does not belong in the kernel.  
If a helper’s name describes **business meaning** rather than **mechanical action**, it does not belong in the kernel.

### Invariants and Requirements

Invariant and requirement helpers are permitted because they:

- do not introduce new domain meaning  
- only enforce already‑defined rules  
- always fail by producing existing kernel values (`DomainError`, `Result<T>`)

They are guardrails, not abstractions.  
They exist to make incorrect code impossible to ignore, not to express business logic.

---

## What Dx.Domain Demands

### Explicit Invariants

Every domain type declares what it refuses to accept.

Invariants are **executable contracts**, not comments. APIs like `Invariant`
exist to make these contracts concrete and enforceable.

### Minimal, Frozen Kernel

The core is **small, stable, and opinionated**.

Evolution happens at the **edges** – analyzers, generators, adapters – never
by quietly expanding the kernel’s scope.

### Errors as Values

Failures are modeled, typed, and composable.

Results and domain errors are values that cross boundaries (APIs,
persistence, tests). Surprises are **architectural bugs**, not business as
usual.

### Architecture That Teaches

The architecture should teach by **constraining expression**:

- Illegal states are unrepresentable.  
- Incoherent error flows are hard to write.  
- Ambiguous semantics feel unnatural or impossible.

Developers should feel where the invariants live simply by using the types.

---

## Scope & Evolution

The ecosystem is organized around a spine and its edges:

- `Dx.Domain` — invariants, results, errors, time, causation primitives.  
- `Dx.Domain.Values` — identities, primitives, enforced value objects.  
- `Dx.Domain.Analyzers` — compile‑time enforcement, migration pressure.  
- `Dx.Domain.Generators` — boilerplate reduction that preserves semantics.  
- `Dx.Domain.Persistence.*` — adapters, never dependencies.

Names may evolve. **Principles do not.**

This manifesto is not documentation; it is the **guardrail against drift**.

Every change answers one question:

> Does this uphold the refusal – or compromise it?
