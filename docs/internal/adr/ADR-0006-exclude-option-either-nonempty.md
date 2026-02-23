# ADR-0006: Exclude Option/Either/NonEmpty from Kernel

**Status**: Accepted  
**Date**: 2026-01-29  
**Spec Reference**: Specification Preface (Exclusions). 

## Context
Functional wrappers like `Option`, `Either`, `NonEmpty` offer marginal semantic benefits but introduce surface area, governance costs, and analyzer burden in this architecture. 

## Decision
Exclude `Option`, `Either`, `NonEmpty` from the Kernel. Use **Result** and explicit invariants for domain control-flow semantics. 

## Consequences
- Smaller, clearer kernel; fewer analyzer/contract permutations to govern.
