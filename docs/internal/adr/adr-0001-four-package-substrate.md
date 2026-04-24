# ADR-0001: Four-Package Substrate (Annotations, Primitives, Kernel, Facts)

**Status**: Accepted  
**Date**: 2026-01-29  
**Spec Reference**: Dx.Domain Refactoring Specification §§2, 4–6; Dependency Graph & Package Rules. 

## Context
We require a durable substrate with clear responsibilities and strict dependencies:
- **Annotations** define pure vocabulary and metadata.
- **Primitives** provide immutable, side-effect-free value types.
- **Kernel** is the runtime judge of invariants, results, errors, and facts.
- **Facts** hold structural, lineage-aware history and remain meaning-agnostic. 

## Decision
Lock the four-package substrate as a long-term architectural contract. Assemblies remain decoupled; analyzers are compiler-only. Kernel **must** depend on Annotations; Kernel **must not** depend on Analyzers. Meta-packages may bundle, but references remain strict.

## Consequences
- Stable, auditable boundaries that support analyzers and generators without runtime entanglement.
- Upgrades and enforcement become predictable across repos.
