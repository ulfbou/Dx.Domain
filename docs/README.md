# Dx.Domain — Documentation Portal (Complete, Role/Layer & Template‑Aware)

**Audience:** Architects, framework contributors, service developers, and template authors.
**Status:** Authoritative. This portal aggregates *all* documents required to understand, adopt, operate, and extend Dx.Domain without relying on tribal knowledge.

## Start Here
- **Architecture Overview:** `architecture-overview.md`
- **Foundation Spec (Normative):** `foundation-spec.md`
- **Analyzer Charters (Index):** `analyzers/README.md`
- **Template Contract (DXT):** `dxt/invariants-schema.md`
- **Build & Governance:** `build/README.md`
- **Enforcement Guarantees (Repo-Local):** `enforcement-guarantees.md`
- **Contributing:** `contributing.md`
- **Principles:** `MANIFESTO.md`, `NON_GOALS.md`, `DPI.md`
- **ADRs:** `adr/` (accepted architectural decisions)

## The Closed Architectural Loop
```
Templates → Roles → Layers → Domain Semantics → Analyzers → CI
```
- Templates publish **intent** as data (DXT).
- Layers define **semantics** (Kernel, Primitives, Annotations, Facts).
- Analyzers enforce **alignment** at compile time.
- CI ratchets **policies** and validates non‑silence.
