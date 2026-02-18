<!-- path: docs/internal/governance/api-freeze.md -->
---
id: gov-api-freeze
title: API Freeze & Baselines
status: Accepted
audience: Maintainers
owners: [KernelOwner, ReleaseManager]
reviewers: [AnalyzersOwner]
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
applies_to:
  packages: [Dx.Domain.Kernel, Dx.Domain.Analyzers]
  layers: [Internal]
canonical: docs/internal/governance/api-freeze.md
related:
  - docs/internal/governance/kernel-law.md
  - docs/internal/rules/analyzers/rule-catalog.md
  - docs/internal/governance/analyzers-law.md
tags: [governance, api-freeze, baselines, approvals]
---

# API Freeze & Baselines

**Purpose.** Protect consumers from unapproved public surface drift. Any public API change in Kernel or Analyzers must be **observed, justified, and approved** before merge.

## Policy (summary)
- **Kernel surface is frozen by default.**
- **New public API** requires:
  1) DPI‑aligned justification and **`[ApprovedKernelApi("...")]`** (or equivalent justification attribute where applicable).  
  2) Public API baseline update (e.g., `PublicAPI.Shipped.txt`) included in the PR.
  3) Approvals: **Kernel Owner + Analyzers Owner + Release Manager**.
- **Breaking changes** (removals/renames/signature changes) are **rejected** unless an ADR authorizes them and a migration guide is included.

## CI gates
- API diff check fails the PR if:
  - There is a public surface delta without the justification attribute or required approvals.
  - Baseline files are missing or not updated.
- DXA040 must report unauthorized API additions; the build fails on DX diagnostics where mandated.

## Procedure
1. Author prepares a PR with:
   - Implementation + justification attribute(s).
   - Baseline updates (`PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt`).
   - Migration notes (if any) and changelog entry.
2. CI runs: build (DX diagnostics as errors), analyzer tests, public API diff, docs lint/snippets.
3. Approvals collected; then squash‑merge.

## Acceptance criteria
- API diff matches proposed change.
- DXA040 clean (or explicitly justified).
- Docs and changelog updated when consumer‑visible behavior changes.
- Approvals present per policy.

**References:**  
- Kernel Law → `docs/internal/governance/kernel-law.md`  
- Analyzers Law → `docs/internal/governance/analyzers-law.md`  
- Rule Catalog (DXA040) → `docs/internal/rules/analyzers/rule-catalog.md`
