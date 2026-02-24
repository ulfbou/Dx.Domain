# Dx.Domain Authority Admission Test (v001)

This checklist defines the **non-negotiable conditions** for any change to the Dx.Domain foundations. It ensures the framework remains a "closed-loop" architectural compiler where authority layers are pure and self-hosting.

It is normative for:

* `Dx.Domain.Annotations` (Vocabulary)
* `Dx.Domain.Primitives` (Identities)
* `Dx.Domain.Kernel` (Logic & Invariants)
* `Dx.Domain.Analyzers` (Enforcement)

Any PR touching these assemblies **must** satisfy this document.

---

## 1. Assembly & Dependency Jurisdiction

For every PR, verify the strict **Authority Hierarchy**:

* **Annotations** (`Dx.Domain.Annotations`):
* [ ] **ZERO** dependencies.
* [ ] Contains only: Marker interfaces, Attributes, and Diagnostic Canon (`DxRuleIds`, etc.).


* **Primitives** (`Dx.Domain.Primitives`):
* [ ] References **only** `Dx.Domain.Annotations`.
* [ ] Contains fundamental identity types (e.g., `ActorId`, `CorrelationId`).


* **Kernel** (`Dx.Domain.Kernel`):
* [ ] References `Annotations` and `Primitives`.
* [ ] Does **not** reference analyzers, generators, or infrastructure.


* **Analyzers** (`Dx.Domain.Analyzers`):
* [ ] References `Annotations` (for vocabulary) and Roslyn/runtime only.
* [ ] **Short-circuits** all consumer rules (DXA*) if `DxLayer != Consumer`.



---

## 2. Annotations Purity Test (Vocabulary-Only)

For any change under `Dx.Domain.Annotations`:

* [ ] **0% Logic:** No methods, control flow (`if`, `switch`), or extension methods.
* [ ] **No Runtime Primitives:** No `Result` types, no Exceptions, no `Guid` wrappers.
* [ ] **Attribute Constraints:** All attributes are `sealed` and parameterized only by primitives (`string`, `bool`, `enum`).
* [ ] **Metadata:** All metadata types are immutable `record`s with no runtime behavior.

---

## 3. Primitives Admission Test (Identities)

For types in `Dx.Domain.Primitives`:

* [ ] Implemented as a `readonly struct`.
* [ ] **Guarded Creation:** No public constructors; creation via internal factories (e.g., `InternalNew`).
* [ ] **Purity:** No implicit casts to/from `Guid` or `string`.
* [ ] **Generation:** Uses only allowed generators (e.g., `Guid.NewGuid()`); no business-semantic or sequential logic.
* [ ] **Standards:** Implements `IParsable<T>` and `ISpanFormattable`.

---

## 4. Kernel Scope Test (The Logic Judge)

For any change in `Dx.Domain.Kernel`:

* [ ] **No Ambient Context:** No `HttpContext`, static state, or service locators.
* [ ] **No Side Effects:** No I/O, persistence, networking, or logging.
* [ ] **Sealed by Default:** Public types are `sealed` or `readonly struct`. No public base types for external inheritance.
* [ ] **Judge-Only:** Kernel code **judges values**; it never coordinates infrastructure.

---

## 5. Result Algebra & Error Canon

* [ ] **Result Purity:** All `Map`, `Bind`, `Tap`, and `Ensure` extensions are side-effect-free.
* [ ] **Error Immutability:** `DomainError` and `InvariantError` are immutable data holders with no environment access.
* [ ] **The Canon:** `DxDomain.Codes` remains a pure catalog of stable constants. New codes follow the grouping scheme (e.g., `Invariant.*`, `Domain.*`).

---

## 6. Invariant & Require Test

* [ ] `Invariant.That(...)`: Throws **only** `InvariantViolationException`. Constructs a full `InvariantError` (code, line, member, context).
* [ ] `Require.That(...)`: Never throws; returns `Result.Failure`.
* [ ] **No Logging:** Diagnostics are treated as **Data**, not logs. Side effects in these paths are forbidden.

---

## 7. Facade Exclusion Rule

* [ ] `Dx.Domain.Kernel` contains **no** ergonomic facades (e.g., static `DxDomain` entry points).
* [ ] All "Sugar APIs" live in outer assemblies (e.g., `Dx.Domain.Runtime`) which may depend on the Kernel.

---

## 8. Fact System Admission

For `Fact<TPayload>`, `Causation`, and `TransitionResult`:

* [ ] Types are structural holders of history/lineage, not behavior.
* [ ] **No Messaging:** No event dispatch or integration logic.
* [ ] Facts are explicitly **not** domain events.

---

## 9. Analyzer Short-Circuit Test

For any change affecting analyzer logic:

* [ ] **Authority Exemption:** Verify the analyzer skips `DXA*` rules if `DxLayer` is `Kernel`, `Primitives`, or `Annotations`.
* [ ] **Self-Hosting:** The framework must build and pass tests with analyzers enabled **without requiring a `.dx/invariants.json` file**.

---

## 10. Mandatory Review Acknowledgement

Every PR touching these authority layers **must** include:

* [ ] A link to this file in the PR description.
* [ ] A statement confirming: *"This PR adheres to the Authority Admission Test (v001). No consumer-discipline rules were suppressed; the layer-short-circuit logic was verified."*
