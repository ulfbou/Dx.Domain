# Enforcement Model

## Purpose
Defines what "enforced" means in Dx.Domain and the limits of each guarantee. This document interprets the classifications in SPEC.md; it does not create new rules.

## Definition
A constraint is enforced if and only if a violation is deterministically detected at build time within the analyzer's declared scope.

## Strength Levels

### Strong
- **Detection:** build error
- **Scope:** statically analyzable code in current compilation
- **Bypass:** only by disabling analyzer or using justified suppression
- **Guarantee:** violation cannot reach runtime undetected under normal build

### Heuristic
- **Detection:** build warning in S1–S3
- **Scope:** statically analyzable code, S1–S3 only
- **Bypass:** reflection, serialization, dynamic, suppression
- **Guarantee:** detects common misuse, not exhaustive

### Partial
- **Detection:** runtime invariant check
- **Scope:** execution path
- **Bypass:** bypassing Kernel APIs
- **Guarantee:** fails fast at runtime, not compile time

### Process
- **Detection:** human review, CI policy
- **Scope:** repository governance
- **Bypass:** not mechanically preventable
- **Guarantee:** auditability only

### Moderate
- **Detection:** design guideline
- **Scope:** documentation
- **Bypass:** not mechanically enforced
- **Guarantee:** convention only

## Boundaries
Enforcement applies only within statically analyzable scope and declared analyzer coverage. It does not imply runtime prevention, semantic correctness, completeness across assemblies, or resistance to intentional suppression.

*Source: SPEC.md definition, ADR coverage levels*
