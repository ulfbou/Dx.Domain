# Dx.Domain — Overview

Dx.Domain is a small, opinionated substrate providing invariants, results, errors, identities, and structural history — designed so that **incorrect domain modeling is difficult or impossible to express**.

If code **compiles**, **passes analyzers**, and the **Kernel accepts it**, the state is valid.

This document is the high-level public overview. Internal governance documents (Manifesto, Non-Goals, DPI, ADRs, Spec) live under `docs/` and are published.

---

## Philosophy

Dx.Domain enforces a strict architectural separation:

- **Annotations**: semantic vocabulary, attributes, markers
- **Primitives**: immutable identity/value types
- **Kernel**: runtime judge of invariants, results, errors, facts
- **Facts**: structural lineage, meaning-agnostic history

The Kernel is intentionally minimal, explicit, and highly disciplined.

---

## Enforcement

Correctness is not optional. Ten analyzers ([DXA010–DXA080](../public/packages/analyzers.md)) run on every build in S1–S3 and treat violations as warnings or errors. S0 is exempt per [ADR-0018](../adr/ADR-0018-kernel-public-surface.md). See [Architecture Overview](../public/architecture-overview.md#analyzer-governance).

---

## Rationale

This repository's governance establishes:
- Clear layer boundaries
- Mandatory analyzers enforcing correctness in S1–S3
- Result-based error semantics with public factories
- Explicit invariants
- Frozen Kernel surface

---

## Navigation
**Public:** [Overview](../learn/OVERVIEW.md) | [Architecture](../public/architecture-overview.md)
**Start:** [Getting Started](getting-started.md) | [Quickstart](../learn/quickstart.md)

