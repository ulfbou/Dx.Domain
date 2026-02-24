# Analyzer Suite — Index & Guidance

**Charter Format:** Intent • Applies to • Never applies to • Classification • Examples • Remediation.

## Core Role/Law Rules (DXK*)
- DXK001 — Assembly Role Required
- DXK002 — Role Dependency Matrix
- DXK003 — Domain Purity (no forbidden namespaces)
- DXK004 — Primitive Obsession (prefer domain primitives)
- DXK005 — Illegal Exception Flow (consumer misuse)
- DXK006 — Outbox Fact Boundary (only IDomainFact)
- DXK007 — Contract Hygiene (Contracts ↛ Kernel)
- DXK008 — Observability Invariant (Host must propagate CorrelationId)

## Consumer Discipline (DXA*)
- DXA010 — Construction Discipline (facade boundary)
- DXA011 — Public Factory Exposure (no public constructors/factories on domain types)
- DXA020 — Result Ignored (must handle/return)
- DXA022 — Result vs Throw (domain outcome must be Result.Failure)
- DXA030 — Unapproved Handler (Result only to approved handlers)
- DXA040 — Kernel Public Surface Freeze (S0 safety net)
- DXA050 — Temporal Helper Usage (policy/temporal logic in adapters)
- DXA060 — Forbidden Vocabulary (avoid pattern museums)
- DXA070 — Generated Code Tagging (mark generated code)
- DXA080 — Facade Invariant Enforcement Missing

## Generators (DX100*, DX700*)
- DX1001 — Referential Transparency
- DX1002 — Monotonic Knowledge
- DX1003 — No Semantic Guessing
- DX1004 — No Hidden Coupling
- DX7001 — Undeclared External Input (sandbox)
- DX7002 — Non‑Deterministic Cacheable Stage

See `charters/` for detailed rule specs and `layer-awareness.md` for kernel/consumer scoping.
