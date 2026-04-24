# ADR-0003: Scope Resolution & Rule Authority Modes

**Status**: Accepted  
**Date**: 2026-01-29  
**Spec Reference**: Scope Resolution; Authority & Phase Semantics. 

## Context
Rules must behave differently per layer (Kernel/Primitives/Annotations/Consumer). We need a single, deterministic way to classify scope and a formal authority model. 

## Decision
**Scope precedence** (authoritative → fallback):
1. `dx.layer` (AnalyzerConfig) / `DxResolvedRole`  
2. `[assembly: DxLayer("…")]` (name-only discovery)  
3. Assembly-name heuristic  
4. Default: Consumer (fail-open)  
Kernel/Primitives/Annotations are **never** treated as Consumer; test projects excluded from Consumer discipline. 

**Authority modes**:
- **Definitional** (Kernel)
- **Structural** (Primitives/Annotations)
- **Constraining** (Consumer)
- **Observational** (Kernel-aware but non-restrictive) 

## Consequences
- Same rule ID can be definitional/observational in Kernel, constraining in Consumer—without ambiguity.
