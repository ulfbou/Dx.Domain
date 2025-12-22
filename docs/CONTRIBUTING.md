# Contributing to Dx.Domain

**The Kernel is Frozen.**
If you have not read the Manifesto, Non‑Goals, and the Design Pressure Index (DPI), your contribution will be rejected. This package defines *primitives*, not features.

## Process (Non‑Negotiable)

1. **Read the constraints.** Understanding the "Frozen Kernel" doctrine is prerequisite.
2. **Classify your change.** Is it a Boundary Value, a Mechanical Primitive, or a Semantic Helper?
3. **Open a Pull Request.** You must complete the DPI checklist.
4. **Enforcement.** Reviewers will reject any PR that introduces semantic expansion, regardless of utility.

## Placement Guidance

### 1. The Gateway (`Dx`)
The `Dx` class is the **only** public entry point for creating Kernel primitives.
* **Public Facades:** `Dx.Result`.
* **Internal Mechanics:** `Dx.Invariant`, `Dx.Require`, `Dx.Faults`.

### 2. Boundary Values (`Result<T>`, `Fact`)
These types are public *data carriers*, but their creation logic is locked. 
* **Constraint:** Factories must be `internal`. Instantiation must flow through the `Dx` Gateway.

### 3. Core vs. Values
To maintain the "spine" of the domain, the boundary between Core and Values is defined as follows:
* **Core**: only executable invariants, error primitives, and minimal kernel primitives. 
  * *Amendment:* Identity and time/causation value types that participate directly in invariants, errors, or results (e.g., `TraceId`, `Causation`) are considered part of the core kernel.
* **Values**: strongly typed value objects and identity types that represent domain-specific concepts (e.g., `Email`, `Money`, `OrderId`).

## Mechanics vs. Semantics

The kernel forbids semantic expansion, not mechanical support. Static analysis and reviewers enforce this distinction rigorously.

### Architectural Visibility

In this project, **"Internal" refers to architectural visibility first and language visibility second.** A construct may be technically visible (via `internal` keyword, `InternalsVisibleTo`, or friend assemblies) for testing or tooling, but it must not be treated as part of the public domain vocabulary or leaked to external consumers.

### Allowed: Mechanical Support Code
Code that purely enforces structural correctness or handles low-level operations without implying business meaning.
* **Examples:** Null checks, range validation, ID generation, immutable transformations.
* **Amendment:** Mechanical support code may include low-level or performance-oriented implementations (e.g., `stackalloc`, `Span<T>`), provided they do not alter observable semantics.

### Forbidden: Semantic Helpers
Code that implies *how* the domain functions or interprets data.
* **Examples:** `ApplyEvent`, `HandleCommand`, `NextStep`, `IsValidForProcessing`.
* **Amendment:** If a helper’s name describes business meaning rather than mechanical action, it does not belong in the kernel.

## Enforcement Checklist

* [ ] **No Public Helpers.** Is the helper `internal`?
* [ ] **No Vocabulary Leak.** Does the code avoid words like "Aggregate", "Event", "Command", or "Saga"?
* [ ] **No Policy.** Does the code strictly model *what is* rather than *what should happen*?
* [ ] **DPI Passed.** Does the change lower or maintain the Design Pressure Index?
