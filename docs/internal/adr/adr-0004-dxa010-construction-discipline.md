# ADR-0004: DXA010 — Construction Discipline

**Status**: Accepted  
**Date**: 2026-01-29  
**Spec Reference**: Rule §8.1 DXA010. 

## Context
Construction must be centralized behind **approved boundaries**. Consumers may use their own facades; analyzers must enforce the boundary, not hardcode a specific API. 

## Decision
- **Consumer-only** (Constraining). Kernel/Primitives/Annotations are out of scope.  
- Domain type detection precedence: **markers → base interfaces → namespace**.  
- Approval via marker attributes (name-only), AnalyzerConfig allow-lists, or known patterns; inputs **classify**, not suppress.  
- Mandatory exemptions: exception types, result/failure infra, assembly metadata, internal helpers. 

## Consequences
- Centralized construction enforced universally, without framework-prescriptive APIs.
