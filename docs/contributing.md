# Contributing — Role/Layer & DXT Aware

**First Principles**
- Architecture is compiled: templates declare intent; foundations define semantics; analyzers enforce; CI ratchets.
- Kernel is small and frozen; expansion lives at the edges.

**Placement**
- Core semantics → Kernel/Primitives/Annotations.
- Discipline → Analyzers.
- Boilerplate removal → Generators.
- Adapters/I‑O → Infrastructure.

**PR Process**
1. Declare placement by Role/Layer.
2. Update DXT if templates change.
3. Update rule charters/tests if analyzer‑consumed semantics change.
4. DPI check: increases proofs, lowers accidental complexity; no kernel semantic expansion.
