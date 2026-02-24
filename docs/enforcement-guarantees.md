# Dx.Domain Enforcement Guarantees (Repository-Local)

This document defines the **repository-local, non-negotiable guarantees** implemented inside `dx.domain` without relying on templates.

## Analyzer Behavior Guarantees

- **Authority (S0)** projects are exempt from consumer-discipline analyzers.
- **Consumer (S3)** projects always run consumer-discipline analyzers.
- **Test projects** still require DXT presence but are exempt from consumer-only discipline rules via scope resolution.
- Scope is driven by `DxLayer` or `DxLayerAttribute` build metadata, not by naming heuristics.
- Kernel API freeze enforcement (DXA040) activates only when `build_property.DxKernelApiFreeze=true`.

## DXT Enforcement Semantics

- `.dx/invariants.json` is treated as an **opaque external contract**.
- If a project is **consumer scope** and DXT is missing, analyzers emit a deterministic DX error (`DXT004`).
- Authority layers **never** probe for or require DXT.

## Dependency Physics (Template-Agnostic)

The following hard forbids apply to **consumer scope** projects regardless of any DXT allow-list:

- Domain → Infrastructure
- Contracts → Kernel
- Consumer → internal `Dx.Domain.*` packages (`Dx.Domain.Analyzers`, `Dx.Domain.Generators`, `Dx.Domain.Persistence`, `Dx.Domain.Transport`, etc.)

Violations must fail the build via DX diagnostics.

## Analyzer Distribution Guarantees

- Analyzers are shipped only as transitive assets in `Dx.Domain.Kernel`, `Dx.Domain.Primitives`, and `Dx.Domain.Annotations`.
- `Dx.Domain.Analyzers` is **not packable** and cannot be published accidentally (defaults enforce non-packable + DXB004 guard).

## MSBuild Governance (dx.domain)

- Authority repositories build with analyzers enabled and **do not require DXT**.
- Consumer-only non-silence rules are explicitly scoped to non-test consumer solutions (see `builds/policy/Dx.DomainAnalyzerGovernance.targets`).
- Governance rules (`DXB001`-`DXB003`) never apply to authority repositories.

## CI Readiness (dx.domain)

- CI must run analyzers and build successfully without DXT present.
- CI must fail if authority code triggers consumer-only diagnostics.

## Out of Scope (Template Responsibilities)

Templates must:

- Emit `.dx/invariants.json` at the solution root.
- Populate DXT with canonical role mapping and dependency allow-lists.
- Import MSBuild governance once at the solution root.

These are downstream responsibilities and do not block enforcement in this repository.
