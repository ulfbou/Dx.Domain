<!-- path: docs/internal/governance/non-silence-axiom.md -->
---
```yaml
id: non-silence-axiom
title: Non‑Silence Axiom (Analyzer Governance)
status: Accepted
audience: Maintainers
owners: [AnalyzersOwner]
reviewers: [KernelOwner, ReleaseManager]
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
applies_to:
  packages: [Dx.Domain.Analyzers, Dx.Domain.Annotations]
  layers: [Internal]
canonical: docs/internal/governance/non-silence-axiom.md
related:
  - docs/internal/governance/analyzers-law.md
  - docs/internal/rules/analyzers/rule-catalog.md
tags: [governance, analyzers, non-silence, dx-diagnostics]
```
---

# Non‑Silence Axiom (Analyzer Governance)

**Purpose.** Prevent silent failure by ensuring analyzer diagnostics (DX*) remain active, visible, and enforced.

## Axiom
- **DX diagnostics cannot be globally suppressed or downgraded** in repository configuration (e.g., `.editorconfig`), except where explicitly allowed for test projects.
- **Non‑test projects** treat DX diagnostics as **errors**.
- Governance is imported **once** via `Directory.Build.targets` to keep enforcement deterministic.

## Configuration facts vs switches
- Analyzer inputs (e.g., `dx.scope.map`, `dx.scope.rootNamespaces`, `dx_generated_markers`) **classify** code; they do **not** disable rules.

## CI expectations
- Warnings‑as‑errors for DX* where mandated.
- No repository‑wide suppression allowed.
- PRs modifying analyzer severity or attempting global suppression are rejected unless an ADR authorizes a temporary exception.

## Acceptance criteria
- DX diagnostics present and failing builds when violations exist.
- Scope classification stable and deterministic across CI and local dev.
- Rule lifecycle followed for new rules (observational → enforced), with changelog and migration notes.

**References:**  
- Analyzers Law → `docs/internal/governance/analyzers-law.md`  
- Rule Catalog → `docs/internal/rules/analyzers/rule-catalog.md`
