<!-- path: docs/internal/governance/ChangeRequest_AnalyzersLaw.md -->
---
```yaml
id: cr-analyzers-law
title: Change Request — Establish "Analyzers Law"
author: <your-name>
date: 2026-02-18
links:
  analyzers-law: docs/internal/governance/analyzers-law.md
  rule-catalog: docs/internal/rules/analyzers/rule-catalog.md
  non-silence: docs/internal/governance/non-silence-axiom.md
  analyzer-source: src/Dx.Domain.Analyzers/README.md
```
---

# Change Request — Establish “Analyzers Law”

**Executive summary (one line):** Create a formal, enforceable governance document (“Analyzers Law”) defining rule lifecycle, scope/authority, compatibility discipline, CI gates, and documentation requirements for Dx.Domain.Analyzers.

**Intent (one paragraph):** This adds a normative governance layer that *codifies* existing behavior (DXA010/020/022/040, scope model S0–S3, Non‑Silence Axiom) without changing public APIs or default diagnostics. It clarifies how new rules are introduced (Preview→Constrained), how configuration facts classify scope, and how CI couples analyzers with API baselines and documentation updates.
