# Dx.Domain Design Pressure Index (DPI)

The **Design Pressure Index (DPI)** is the mechanical translation of the
[Manifesto](MANIFESTO.md) and [Non‑Goals](NON_GOALS.md) into day‑to‑day decisions.

Every pull request applies pressure to the kernel. DPI decides whether that
pressure **strengthens** the spine or **fractures** it.

> When in doubt, the change does not go into Core.

---

## 1. Core Admission Checklist

A change may enter **`Dx.Domain` core** only if **all** of the following are true:

1. **Invariant Enforcement**  
   It enforces a *domain invariant* that cannot be reliably enforced elsewhere
   (persistence, UI, services, etc.). If the invariant can live at an edge,
   it does not belong in the kernel.

2. **Reduction of Accidental Complexity**  
   It removes boilerplate, ambiguity, or opportunities for misuse. New API
   surface must *decrease* cognitive load, not redistribute it.

3. **Compiler‑Assisted Correctness**  
   It increases what the compiler can prove: stricter types, fewer illegal
   states, clearer lifetimes, narrower domains. If correctness is purely
   runtime, it belongs in adapters, analyzers, or generators.

4. **Teaches Through Friction**  
   It makes the wrong thing harder or impossible. Failure modes are explicit, not
   silent. Friction is acceptable if it prevents incoherent models or APIs.

5. **Manifesto Alignment**  
   It clearly strengthens at least one demand in the
   [Manifesto](MANIFESTO.md) (e.g., explicit invariants, errors as values,
   architecture that teaches).

6. **Non‑Goal Compliance**  
   It violates none of the [Non‑Goals](NON_GOALS.md). If it drifts toward
   “convenience library,” “pattern museum,” or “kitchen sink,” it is rejected.

7. **No Semantic Expansion**  
   It provides mechanical support only (construction, validation, enforcement),
   without introducing new domain nouns, lifecycles, workflows, policies, or coordination.

If any item fails, the change is **not core**.

---

## 2. Edge Placement Rules

When a proposal does not qualify for the core, it may still belong in the
repository – just **not in `Dx.Domain`**.

Use these placement rules:

- **Adapters** (`Dx.Domain.Persistence.*`, HTTP, messaging, frameworks)  
  - Translate domain values, results, and errors to external concerns
    (databases, transports, frameworks). 
  - Never redefine invariants. 
  - Never smuggle transport or persistence details into core types.

- **Generators** (`Dx.Domain.Generators`)  
  - Remove repetition without altering semantics. 
  - Generate code that *strengthens* invariants or results, not bypasses them. 
  - Never introduce magic behavior that is invisible in the surface API.

- **Analyzers** (`Dx.Domain.Analyzers`)  
  - Encode rules the compiler cannot express on its own. 
  - Push consumers toward idiomatic use of results, invariants, and errors. 
  - Prefer diagnostics that are specific, actionable, and mechanically
    enforceable. 
  - Static analysis must distinguish between **semantic expansion** (flag) and
    **mechanical enforcement** (do not flag if internal and non‑extensible).

- **Outside `Dx.Domain`**  
  - Anything that does not enforce invariants or increase correctness of
    domain modeling lives elsewhere (application code, other libraries).

If a feature feels “helpful” but not “structurally necessary,” it probably
belongs at an edge – or not in this repo at all.

---

## 3. DPI in Code Review

When reviewing a change, ask in order:

1. **What invariant or correctness gap does this close?**  
   If the answer is “ergonomics,” “convenience,” or “it feels nicer,” the
   change is suspect.

2. **Could this be implemented as an adapter, analyzer, or generator instead?**  
   Prefer edges over core whenever possible.

3. **What new illegal states or misuses does this *eliminate*?**  
   Every addition should shrink the space of representable bugs.

4. **Does this contradict any explicit Non‑Goal?**  
   If so, the proposal is not a feature request – it is a request to change
   the project’s identity.

5. **Is this mechanical support or semantic expansion?**  
   Purely mechanical, internal helpers that do not add vocabulary are acceptable;
   anything that adds new domain nouns, workflow, policy, or coordination is not.

Document DPI reasoning in the PR description when the decision is subtle.

---

## 4. Decision Rule

- If it **strengthens** invariants, clarifies results, or tightens error
  semantics – and cannot live elsewhere – it may enter the core.
- If it mainly improves convenience, discoverability, or framework
  integration, it belongs at the **edges**.
- If it cannot be justified in terms of the Manifesto and Non‑Goals, it lives
  **outside this repo**.

Rejection is not failure. **Misplacement is.**

---

