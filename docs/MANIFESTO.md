# Dx.Domain Manifesto

## A Declaration of Refusal

Dx.Domain exists to make incorrect domain modeling impossible to ignore.

This is not a convenience library.  
This is not a toolkit of helpers.  
This is a line in the sand.

---

## What Dx.Domain Refuses to Tolerate

### Ambiguity

Every domain concept must be explicit.  
No magic strings. No implicit defaults. No silent fallbacks.  
If a value can be wrong, it must fail loudly and deterministically.

### Accidental Complexity

Boilerplate, ceremony, and leaky abstractions are eliminated.  
Identifiers, invariants, errors, and results are first-class — never ad-hoc utilities.

### Runtime Guesswork

Correctness is compiler-assisted.  
If the compiler cannot help prove safety, the API must not exist.

### Silent Failure

No hidden nulls. No swallowed exceptions. No “best effort” semantics.  
Operations either succeed explicitly or fail explicitly.

### Incoherent Error Semantics

Errors are structured, centralized, and provenance-aware.  
No scattered strings. No improvised codes. No ambiguity about origin or intent.

### Unteachable Architecture

Friction is intentional.  
Misuse is blocked. Correct usage is guided.  
Ergonomics are enforced, not suggested.

---

## What Dx.Domain Demands

### Explicit Invariants

Every domain type declares what it refuses to accept.  
Invariants are executable contracts, not comments.

### Minimal, Frozen Kernel

The core is small, opinionated, and stable.  
Evolution happens at the edges, never in the spine.

### Errors as Values

Failures are modeled, typed, and composable.  
Surprises are architectural bugs.

### Architecture That Teaches

The design itself is pedagogy.  
Developers learn the right patterns by being unable to express the wrong ones.

---

## Scope & Evolution

- `Dx.Domain.Core` — invariants, results, errors, time, causation  
- `Dx.Domain.Values` — identities, primitives, enforced value objects  
- `Dx.Domain.Analyzers` — compile-time enforcement, migration pressure  
- `Dx.Domain.Persistence.*` — adapters, never dependencies  

Names may evolve. Principles do not.

This manifesto is not documentation.  
It is the guardrail against drift.

Every change answers one question:  
Does this uphold the refusal — or compromise it?
