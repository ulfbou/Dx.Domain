# ADR-0000: Record Architecture Decisions

**Status**: Accepted  
**Date**: 2026-01-29  
**Supersedes / Relates**: N/A

## Context
The Dx.Domain project has reached a point where architectural choices must be made explicit, versioned, and auditable. Decisions must align with the **Dx.Domain Refactoring Specification (Normative)**, which defines the layer-aware, authority-aware model for Kernel/Primitives/Annotations and the Non‑Silence axiom for analyzers.

## Decision
All significant decisions will be captured as Architecture Decision Records (ADRs). Each ADR is:
- Immutable once accepted; changes require a **new ADR** that supersedes the prior one.
- Linked to the normative spec
- Versioned as part of the repository history and release process.

## Consequences
- Contributors have a stable, citable source of truth for rationale.
- Release notes and governance audits can reference ADR IDs directly.
