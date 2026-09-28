# Enforcement Model

## Overview

This document explains how enforcement works in Dx.Domain. For normative claims and formal guarantees, see [Dx.Domain Enforcement Specification](enforcement-specification.md).

## Definition

A constraint is **enforced** if and only if a violation is deterministically detected at build time within the analyzer's declared scope.

Enforcement does **not** mean:
- runtime prevention of violations
- semantic correctness of domain logic
- completeness across all code paths
- immunity to intentional suppression

## Analyzer Strength Classification

Dx.Domain classifies analyzers by the confidence and scope of their guarantees:

### Strong Enforcement

- **Detection:** Deterministic at build time, cannot be bypassed except by disabling the analyzer or justified suppression
- **Scope:** Statically visible code paths within participating compilations
- **Example:** DXA050 (temporal authority) in domain layers—all time access must use controlled UTC sources
- **Characteristic:** Sound analysis; violations cannot silently escape

### Moderate Enforcement

- **Detection:** Build-time warnings that catch common misuse patterns
- **Scope:** Heuristic-based; may require indirection to bypass
- **Examples:** DXA010 (construction), DXA020 (result handling), DXA065 (documentation)
- **Characteristic:** Practical coverage with known gaps in reflection, serialization, dynamic invocation

### Weak Enforcement

- **Detection:** Advisory; improves diagnostics but does not guarantee correctness
- **Scope:** Metadata and infrastructure support
- **Example:** DXA070 (generated code tagging) improves false-positive filtering
- **Characteristic:** Best-effort; success depends on external cooperation

## Guarantee Boundaries

All enforcement is **local and structural**, not global or behavioral:

- An ignored Result flagged by DXA020 may still be unhandled downstream
- An invariant detected by DXA010 may not be complete
- A facade factory flagged by DXA080 may still receive invalid inputs

This is by design: mechanical analysis cannot verify business logic.

## Scope Boundaries

Analyzers do **not** detect violations in:
- reflection and dynamic invocation
- ORM or JSON materialization
- code compiled without analyzer support
- behavior in non-participating assemblies

This is a fundamental limit of static analysis.

For detailed specifications, see [Dx.Domain Enforcement Specification](enforcement-specification.md).
