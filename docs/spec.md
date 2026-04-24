# Dx.Domain Specification

> **Status:** Normative  
> **Applies to:** Dx.Domain.Kernel, Primitives, Facts, Analyzers  
> **Authority:** This document supersedes interpretive claims in ADRs where discrepancies exist.

*Source: docs/specifications/dx.domain-enforcement-specification.md*

## Definition of Enforcement
A constraint is considered **enforced** if and only if a violation is **deterministically detected at build time** within the analyzer's declared scope.

Enforcement does **not** mean runtime prevention, semantic correctness, completeness across assemblies, or resistance to intentional suppression.

## Normative Rules

| Rule | ADR | Enforced By | Strength |
|------|-----|-------------|----------|
| Temporal Authority (UTC) | ADR-0001 | DXA050 | Strong |
| Empty CorrelationId Permitted | ADR-0002 | Runtime invariant | Partial |
| Construction Authority | ADR-0003 | DXA010/DXA011/DXA080 | S1–S3 only |
| Result<T> Uses Struct | ADR-0004 | Compiler + DXA040 | Strong |
| No Public Setters | ADR-0005 | Design guideline | Moderate |
| Result as Failure Model | ADR-0006 | DXA040 | Strong |
| System Hardening Sequence | ADR-0007 | Process | Process |
| DXA011 Public Factory | ADR-0008 | DXA011 | S1–S3 only |
| DXA020 Result Ignored | ADR-0009 | DXA020 | Strong |
| DXA022 No Throw | ADR-0010 | DXA022 | Strong |
| DXA030 Unapproved Handler | ADR-0011 | DXA030 | Strong |
| DXA040 Surface Freeze | ADR-0012 | DXA040 | Strong |
| DXA050 Temporal Helpers | ADR-0013 | DXA050 | Strong |
| DXA060 Forbidden Vocabulary | ADR-0014 | DXA060 | Strong |
| DXA070 Generated Code | ADR-0015 | DXA070 | Strong |
| DXA080 Facade Enforcement | ADR-0016 | DXA080 | Strong |
| Suppression Governance | ADR-0017 | Process | Process |
| Kernel Public Surface | ADR-0018 | DXA040 + exemption | Strong |

*Source: docs/adr/index.md*
