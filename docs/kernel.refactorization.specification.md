# Dx.Domain.Kernel Refactorization Specification

## Kernel & Abstractions v1.0 (Authoritative)

---

## 0. Status of This Document

This specification is **normative**.

* Any implementation claiming compliance with Dx.Domain **must** conform to this document.
* Any deviation is a defect, not an interpretation difference.
* Kernel and Abstractions are considered **foundational infrastructure** and are expected to remain stable across major framework evolution.

---

## 1. Core Philosophy: The Substrate of Correctness

Dx.Domain does **not** model business logic.

It provides the **substrate upon which incorrect business logic is structurally difficult to express**.

The architectural invariant is:

> **If it compiles, passes analyzers, and the Kernel accepts it, the state is valid.**

This is achieved through a strict separation of concerns:

* **Dx.Domain.Abstractions** defines *semantic vocabulary* for intent and invariants.
* **Dx.Domain.Kernel** defines *runtime judgment* for validity and failure.
* **Dx.Domain.Analyzers** (out of scope for this document) enforce the contract at design time.

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
```

### Dependency Rules (Strict)

* **Dx.Domain.Abstractions**

  * Has **no dependencies** on Kernel, Analyzers, or user code.
* **Dx.Domain.Kernel**

  * **Must depend on Dx.Domain.Abstractions**
  * **Must not depend on Dx.Domain.Analyzers**
* **Dx.Domain.Analyzers**

  * Depends on **Dx.Domain.Abstractions**
  * May analyze Kernel and User Domain code
* **User Domain Code**

  * Depends on Kernel and Abstractions
  * Never depends on Analyzers at runtime

### Packaging vs. Assembly Clarification

* A **NuGet meta-package** (e.g., `Dx.Domain`) may bundle:

  * Kernel
  * Abstractions
  * Analyzers
* **Assemblies remain strictly decoupled**

  * `Dx.Domain.Kernel.dll` never references analyzers
  * Analyzer assemblies are compiler-only artifacts

---

## 3. Dx.Domain.Abstractions

### *The Vocabulary*

### 3.1 Purpose

Dx.Domain.Abstractions defines **semantic intent** that is visible to:

* The compiler
* Roslyn analyzers
* Source generators

It encodes *meaning*, not behavior.

---

### 3.2 Hard Constraints

Abstractions **must not contain**:

* Runtime logic
* Control flow
* Extension methods
* Exceptions
* Validation helpers
* Identity value types
* Result types
* Time, randomness, or environment access

Abstractions are **pure metadata**.

---

### 3.3 Assembly Contents (Authoritative)

#### 3.3.1 Marker Interfaces (Semantic Roles)

Marker interfaces exist **only** to communicate semantic role.

```csharp
IAggregateRoot
IEntity
IValueObject
IDomainEvent
IDomainPolicy
IDomainFactory
IIdentity
```

Rules:

* No members
* No inheritance chains
* No default implementations
* Used exclusively for:

  * Generic constraints
  * Analyzer targeting
  * Kernel integration

---

#### 3.3.2 Attributes (Semantic Assertions)

Attributes declare **intent**, never behavior.

Mandatory attributes include (non-exhaustive):

```csharp
[AggregateRoot]
[Entity]
[ValueObject]
[DomainEvent]
[Invariant]
[Identity]
[Factory]
[Policy]
```

Rules:

* Attributes must be `sealed`
* Parameters must be primitives (`string`, `bool`, `enum`)
* Attributes must never encode lifecycle or execution semantics

---

#### 3.3.3 Metadata Records (Analyzer Contracts)

Metadata records describe **structural shape** for analyzers and generators.

Examples:

```csharp
AggregateMetadata
IdentityMetadata
InvariantMetadata
FactoryMetadata
```

Rules:

* `record` only
* Immutable
* No reflection helpers
* No lazy evaluation
* No runtime behavior

**Primitive Discipline:**
If identifiers or samples are required, use **string representations**, not `Guid`, to preserve zero-runtime semantics.

---

#### 3.3.4 Diagnostic Canon

Abstractions define the **canonical diagnostic vocabulary**.

```csharp
DxRuleIds
DxCategories
DxSeverities
```

Rule families (minimum):

* DXA01x — Construction authority
* DXA02x — Result discipline
* DXA03x — Identity violations
* DXA04x — Immutability violations
* DXA05x — Vocabulary pollution
* DXA06x — Analyzer-only semantic leaks

This canon is:

* Versioned
* Append-only
* Shared by all analyzers

---

## 4. Dx.Domain.Kernel

### *The Judge*

### 4.1 Purpose

Dx.Domain.Kernel provides the **exclusive runtime authority** for:

* Validity
* Failure
* Identity
* Invariants
* Transitions
* Structural history

Kernel types **judge values**.
They never act on infrastructure, I/O, or environment.

---

### 4.2 Mandatory Dependency on Abstractions

Kernel **must reference Dx.Domain.Abstractions** in order to:

* Implement marker interfaces (`IIdentity`, etc.)
* Consume attributes (`[Invariant]`, `[Factory]`)
* Align runtime behavior with analyzer semantics

This dependency is **strict and one-way**.

---

### 4.3 Laws of the Kernel (Final)

#### Law 1: No Ambient Context

Kernel code must never access:

* Global state
* Thread-local storage
* HttpContext
* Service locators

All context is explicit and immutable.

---

#### Law 2: Restricted Generation

Kernel may generate values **only if**:

1. No business semantics
2. No ordering meaning
3. Algorithm invariant across runtimes

Allowed:

* `Guid.NewGuid()` for infrastructure identity
* Cryptographic randomness

Forbidden:

* Sequential IDs
* Time-based ordering
* Database-derived identifiers

---

#### Law 3: Diagnostics as Data

Invariant violations produce **structured data**, not logs.

`InvariantError` must include:

* Code
* Message
* Member
* File
* Line
* Correlation context

Kernel never logs.

---

#### Law 4: Functional Purity

Kernel extension methods are:

* Pure
* Side-effect-free
* Referentially transparent

Side effects belong to the caller.

---

#### Law 5: Kernel Types Are Final

No public inheritance from Kernel primitives.

---

#### Law 6: Kernel Has No Opinions

Kernel contains:

* No retries
* No policies
* No telemetry
* No orchestration

---

### 4.4 Assembly Contents (Authoritative)

#### 4.4.1 Identity Primitives

Examples:

```csharp
ActorId
TraceId
CorrelationId
SpanId
FactId
```

Requirements:

* `readonly struct`
* No public constructors
* Guarded creation only
* Implements `IIdentity` (from Abstractions)
* Implements `IParsable<T>` and `ISpanFormattable`
* No implicit conversions

---

#### 4.4.2 Result Algebra (Canonical)

```csharp
Result
Result<T>
Result<T, TError>
```

Rules:

* Exclusive flow-control mechanism
* Failures are values, not exceptions
* No implicit casting
* Allocation-aware

---

#### 4.4.3 Error Model

```csharp
DomainError
InvariantError
```

Rules:

* Errors are immutable data
* No logging
* Carry causation where applicable

---

#### 4.4.4 Invariant Enforcement

Two explicit mechanisms:

```csharp
Invariant.That(...)   // Panic (exception)
Require.That(...)     // Recoverable failure
```

Rules:

* Panic throws only `InvariantViolationException`
* Recoverable failures return `Result.Failure`
* Full diagnostic context is mandatory

---

#### 4.4.5 Fact System (Structural History)

```csharp
Fact<TPayload>
Causation
TransitionResult<TState>
```

Facts are:

* Structural
* Lineage-aware
* Meaning-agnostic

Facts are **not domain events**.

---

#### 4.4.6 Functional Extensions

Permitted:

```csharp
Map
Bind
Tap
Ensure
```

Rules:

* Pure
* Explicit
* No hidden control flow

---

### 4.5 Explicit Non-Goals

Kernel will **never** contain:

* Repositories
* Event dispatch
* Persistence
* Serialization
* Clocks
* Time abstractions
* Infrastructure adapters

---

## 5. Tooling Contract (Analyzer Alignment)

Analyzers consume **Abstractions** and analyze **Kernel + User Code**.

Mandatory enforcement includes:

1. Immutability of `[ValueObject]` and `[DomainEvent]`
2. Aggregate root identity enforcement
3. Factory-only construction
4. Result usage discipline
5. Vocabulary pollution detection

Analyzers are **external judges**, never runtime participants.

---

## 6. Final Assessment

With the corrected dependency graph and mandatory Kernel → Abstractions linkage:

* The architecture is sound
* The semantic contract is enforceable
* Runtime purity is preserved
* Tooling authority is externalized correctly

This document is now the **authoritative definition of Dx.Domain**.

No further structural changes are required before v1.0 release.
