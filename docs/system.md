# Dx.Domain System Model

> **Classification:** Retained repository summary. Canonical consumer documentation is under [`docs/public`](public/index.md), and conflicting claims must defer to the repository's [documentation authority](internal/documentation-authority.md).

## Purpose

Dx.Domain is a small, compiler-assisted substrate for explicit invariants, results, errors, identities, and structural facts.

*Canonical source: [Public architecture](public/architecture.md)*

## Packages

- **Annotations:** semantic vocabulary and metadata
- **Primitives:** immutable identity and value types
- **Kernel:** results, errors, invariants, and requirements
- **Facts:** structural fact and causation values, not a domain-event bus
- **Analyzers:** compile-time diagnostics installed explicitly by consumers during alpha

*Canonical source: [Public architecture](public/architecture.md)*

## Dependency rules

Facts depends on Primitives, Kernel, and Annotations. Kernel and Primitives depend on Annotations. Repository runtime projects reference analyzers as compiler analyzers; that repository configuration does not establish transitive NuGet behavior.

*Canonical source: [Public architecture](public/architecture.md)*

## Scopes S0-S3

- **S0:** substrate assemblies owned by Dx.Domain
- **S1:** consumer domain and construction boundaries
- **S2:** application orchestration
- **S3:** infrastructure and adapters

Analyzer behavior is scope-aware. Rule-specific applicability belongs in the [canonical diagnostic reference](public/reference/diagnostics/index.md) and must agree with implementation and tests.

## Stability and enforcement

Runtime APIs and analyzer behavior are provisional for `0.1.0-alpha`. Architectural principles may be stable, but a surface is not described as frozen without a verified API baseline. Standard compiler suppression mechanisms remain technically available; repository policy may separately govern their use.
