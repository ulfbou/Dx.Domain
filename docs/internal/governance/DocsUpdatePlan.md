<!-- path: docs/internal/governance/DocsUpdatePlan.md -->
---
```yaml
id: docs-update-plan-analyzers-law
title: Documentation Update Plan — Analyzers Law
status: Proposed
audience: Contributors
owners: [DocsLead]
reviewers: [AnalyzersOwner]
```
---

# Documentation Update Plan — Analyzers Law

## Targets & Scope

1) **Analyzers Law (new)**  
   - **Path:** docs/internal/governance/analyzers-law.md  
   - **Edits:** N/A (new)  
   - **Acceptance:** metadata present; citations to Rule Catalog & Non‑Silence; lifecycle stated.

2) **Rule Catalog**  
   - **Path:** docs/internal/rules/analyzers/rule-catalog.md  
   - **Edits:** ensure each rule lists scope behavior S0–S3, examples, performance notes, and migration links.

3) **Public Analyzers Guide**  
   - **Path:** docs/public/packages/analyzers.md  
   - **Edits:** consumer‑facing overview; `.editorconfig` examples for **classification facts** only (no global downgrades).

4) **Kernel Law (cross‑link)**  
   - **Path:** docs/internal/governance/kernel-law.md  
   - **Edits:** cross‑link DXA040 & API baseline policy section.

## Sample Diff Summary (illustrative)

```diff
- See analyzers overview for rule lifecycle.
+ See ../governance/analyzers-law.md for the normative rule lifecycle (Preview→Constrained).
```
