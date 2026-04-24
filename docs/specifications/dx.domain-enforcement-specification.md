## Dx.Domain Enforcement Specification

> **Status:** Normative  
> **Applies to:** Dx.Domain.Kernel, Primitives, Facts, Analyzers  
> **Authority:** This document supersedes interpretive claims in ADRs where discrepancies exist.

---

## 1. Purpose

This document defines **what is enforced**, **where enforcement applies**, and **what guarantees are and are not provided** by the Dx.Domain system.

It exists to prevent:
- over‑claiming of correctness,
- ambiguity about analyzer behavior,
- drift between architectural intent and mechanical enforcement.

All enforcement claims in Dx.Domain **must be derivable from this specification**.

---

## 2. Definition of Enforcement

In Dx.Domain, a constraint is considered **enforced** if and only if:

> A violation is **deterministically detected at build time** within the analyzer’s declared scope.

Enforcement **does not mean**:
- runtime prevention,
- semantic correctness of business logic,
- completeness across assemblies,
- resistance to intentional suppression.

---

## 3. Global Enforcement Boundary

### 3.1 Visibility Boundary

All analyzers operate exclusively on:

- **statically analyzable code paths**
- **within the current compilation unit**
- **with analyzers enabled**

They do **not** analyze:

- reflection (`Activator.CreateInstance`, `Type.InvokeMember`)
- serialization materialization (ORMs, JSON deserialization)
- dynamic invocation
- code in non‑participating assemblies

---

### 3.2 Execution Boundary

All enforcement is **compile‑time only**.

There is **no runtime policing** of:
- construction,
- time access,
- invariant correctness,
- failure propagation.

---

## 4. Enforcement Strength Classification

Every analyzer is classified using the following taxonomy.

| Classification | Meaning |
|----------------|--------|
| **Strong** | Sound within static scope. Cannot be bypassed without analyzer suppression or disabling. |
| **Moderate** | Heuristic or pattern‑based. Bypassable via indirection or abstraction. |
| **Weak** | Advisory or accuracy‑supporting. Improves DX; does not guarantee correctness. |

> No analyzer is considered strong unless its guarantees are **closed under static surface analysis**.

---

## 5. Enforced Domains and Guarantees

### 5.1 Construction Authority

**Intent:** Prevent uncontrolled creation of domain objects.

#### Enforcing Analyzers

| Rule | Strength | Notes |
|-----|----------|------|
| DXA010 | Moderate | Direct construction detection only |
| DXA011 | Moderate–Strong | Strong at public surface, partial semantically |
| DXA080 | Moderate | Presence of invariant enforcement only |

#### Composite Guarantee

> In statically analyzable consumer code, direct construction paths are constrained to approved facade or factory entry points.

#### Explicit Gaps

- reflection
- serialization
- internal kernel misuse
- indirect or delegated construction chains

---

### 5.2 Failure Semantics (Result)

**Intent:** Make domain failure explicit and non‑silent.

#### Enforcing Analyzers

| Rule | Strength |
|-----|----------|
| DXA020 | Moderate |
| DXA022 | Moderate |
| DXA030 | Moderate |

#### Guarantee

> Directly ignored Result values and common exception‑based control flow misuse are flagged within local, visible scope.

#### Non‑Guarantees

- correctness of handling logic
- transitive propagation correctness
- cross‑layer enforcement

---

### 5.3 Kernel Integrity

**Intent:** Prevent semantic expansion and API drift.

#### Enforcing Analyzers

| Rule | Strength | Conditions |
|-----|----------|------------|
| DXA040 | **Conditional Strong** | Requires API baseline |
| DXA060 | Moderate | Naming heuristic |

#### Guarantee

> Kernel public surface does not expand unintentionally **when a baseline is present and enforced**.

#### Failure Mode

> If no baseline exists, DXA040 provides **no protection**.

---

### 5.4 Temporal Authority

**Intent:** Eliminate non‑deterministic and timezone‑ambiguous time access.

#### Enforcing Analyzers

| Rule | Strength | Scope |
|-----|----------|-------|
| DXA050 | Strong | Domain layer only |

#### Guarantee

> All statically analyzable domain code uses controlled UTC time sources.

#### Explicit Gaps

- infrastructure code
- external inputs
- wrapped time abstractions

---

### 5.5 Analyzer Infrastructure Integrity

**Intent:** Improve accuracy of other analyzers.

#### Enforcing Analyzers

| Rule | Strength |
|-----|----------|
| DXA070 | Weak |

#### Guarantee

> Analyzer false‑positive rate is reduced where generators comply.

---

## 6. Non‑Transitivity Clause

No analyzer guarantees correctness **beyond its immediate detection scope**.

Specifically:
- Assigning a Result does not guarantee it is later handled.
- Detecting an invariant call does not guarantee invariant completeness.
- Flagging a factory does not guarantee all callers pass valid inputs.

All enforcement is **local and structural**, not global or behavioral.

---

## 7. Suppression Model

All analyzers may be bypassed via:

- `#pragma warning disable`
- `.editorconfig` overrides
- global suppression files

This is considered a **conscious opt‑out**, not an enforcement failure.

> Future rule **DXA090** governs suppression discipline; it does not eliminate suppression.

---

## 8. System Guarantee (Precise)

Dx.Domain guarantees:

> Within statically analyzable code paths of participating assemblies, violations of declared architectural constraints are detected at build time.

Dx.Domain does **not** guarantee:

- runtime correctness
- immunity to reflection or serialization
- compliance across unchecked assemblies
- semantic validity of domain logic

---

## 9. Design Position

Dx.Domain is:

> **Compiler‑assisted architectural governance**, not formal verification.

The system optimizes for:
- early failure
- local reasoning
- explicit discipline

It **rejects**:
- silent fallback
- hidden control flow
- unbounded correctness claims

---

## 10. Canonical Precedence

In the event of conflict:

```
Dx.Domain Enforcement Specification
↓
[LIMITATIONS.md](../learn/stability.md)
↓
[ENFORCEMENT_MODEL.md](../learn/enforcement_map.md)
↓
[ENFORCEMENT_MAP.md](../learn/enforcement_map.md)
↓
ADR-0001 … ADR-0016
↓
[README](../../readme.md) / comments / blog posts
```

---


