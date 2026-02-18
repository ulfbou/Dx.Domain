<!-- path: docs/internal/governance/docs-conventions.md -->
---
```yaml
id: docs-conventions
title: Dx.Domain Documentation Conventions
status: Accepted
audience: Contributors
owners:
  - Docs Lead
  - Kernel Owner
reviewers:
  - Analyzers Owner
  - Release Manager
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
applies_to:
  packages: [Dx.Domain.Kernel, Dx.Domain.Primitives, Dx.Domain.Facts, Dx.Domain.Annotations, Dx.Domain.Analyzers]
  layers: [Internal, Public]
canonical: docs/internal/governance/docs-conventions.md
related:
  - docs/internal/specs/kernel.refactorization.specification.md
  - docs/internal/governance/kernel-law.md
  - docs/internal/governance/api-freeze.md
  - docs/internal/rules/analyzers/rule-catalog.md
tags: [governance, docs, conventions, metadata, dpi]
```
---

# Dx.Domain — Documentation Conventions (Specification)

> Purpose — Define an enforceable, repeatable way of writing, placing, and maintaining documentation for Dx.Domain. This guide specifies where each document type lives, how it is named, what metadata it must carry, and how CI validates it. It aligns with Kernel Law, Analyzers Law, API‑freeze rules, DPI, and the normative kernel specification.

***

## 1. Scope & principles

1. **Docs‑as‑code:** all docs live in‑repo and follow the same PR rigor as code.  
2. **Two audiences:**  
   - Public: What/Why/How; consumer‑level guidance.  
   - Internal: Normative truths (Laws, Specs, ADRs, DPI, Rule Catalog).  
3. **Normative sources:** Kernel Spec, Admission Test, ADRs, Non‑Silence Axiom, Analyzers Law.  
4. **Kernel constraints in examples:** no I/O/logging; diagnostics as values; UTC‑only.  
5. **No semantic helpers:** examples must not introduce domain semantics not present in code.

***

## 2. Directory layout & placement

```
/docs/public/                   # Consumer-facing documentation
  index.md
  getting-started.md
  concepts/*.md
  packages/
    kernel.md
    primitives.md
    facts.md
    annotations.md
    analyzers.md
  guides/*.md
  api/                          # Generated API reference
/docs/internal/                 # Normative, contributor-only truth
  governance/
    kernel-law.md
    dependency-law.md
    api-freeze.md
    non-silence-axiom.md
    docs-conventions.md
    dpi.md                      # DPI requires metadata (see §4.3)
  specs/
    kernel.refactorization.specification.md
  rules/
    analyzers/
      rule-catalog.md
      scope-resolution.md
  adr/
    ADR-*.md
```

Do not move normative documents without a superseding ADR.

***

## 3. Document types & naming

### 3.1 Public docs

- `/docs/public/packages/{package}.md` — What/Why/How per package  
- `/docs/public/concepts/*.md` — invariants, results, errors, UTC‑time  
- `/docs/public/guides/*.md` — consumer workflows  
- `/docs/public/api` — generated XML‑doc output

### 3.2 Internal docs

- Governance: Kernel Law, Dependency Law, API Freeze, Non‑Silence Axiom, DPI  
- Specs: Kernel Specification (canonical source)  
- Analyzer rule catalog: rule intent, severity, lifecycle  
- ADRs: Immutable decisions (`ADR-YYYY-title.md`)

File naming: **kebab‑case**, single H1 per file.

***

## 4. Metadata model (YAML front matter)

All Markdown documents must begin with fenced YAML:

````md
---
```yaml
...metadata...
```
---
`````

### 4.1 Required keys for all documents

```yaml
id: unique-doc-id
title: Human Readable Title
status: Draft|Proposed|Accepted|Deprecated|Superseded
audience: Public|Contributors|Maintainers
owners: [Owner1, Owner2]
reviewers: [ReviewerA, ReviewerB]
last_reviewed: YYYY-MM-DD
next_review_date: YYYY-MM-DD
canonical: docs/.../file.md
related:
  - docs/internal/specs/kernel.refactorization.specification.md
tags: [governance, docs]
```

### 4.2 Optional keys

```yaml
applies_to:
  packages: [Dx.Domain.Kernel]
  layers: [Internal, Public]
rule_ids: [DXA010, DXA020, DXK002]
api_links:
  - docs/public/api/Dx.Domain.Kernel/index.html
snippet_test:
  tfl: "net8.0"
versioning:
  doc_version: "1.0"
redirect_from: ["/old/slug"]
toc: true
```

### 4.3 Additional required metadata for DPI documents

Documents that define or constrain DPI (e.g., `dpi.md`) must include:

```yaml
dpi_version: 1.0
derived_from:
  - MANIFESTO.md
  - NON_GOALS.md
applies_to:
  packages: [Dx.Domain.Kernel, Dx.Domain.Primitives, Dx.Domain.Analyzers]
  layers: [Internal]
interacts_with:
  - docs/internal/governance/kernel-law.md
  - docs/internal/governance/analyzers-law.md
  - docs/internal/governance/ApprovalWorkflow.md
  - docs/internal/governance/repo-conventions.md
constraints:
  - "No semantic expansion"
  - "Mechanical correctness only"
  - "Prefer edges over core"
```

These fields capture DPI’s normative dependencies and governance scope.

***

## 5. Style & language conventions

1.  Examples must reflect Kernel constraints: no I/O/logging; diagnostics as data; UTC‑only.
2.  Avoid semantic APIs or patterns (Repository, Saga, Handler, Apply).
3.  Code examples must compile under `net8.0` unless otherwise specified.
4.  Use relative links inside `/docs`.
5.  Status badges must match the YAML.

***

## 6. URL, slug, and anchors

*   Slugs follow the file path.
*   Analyzer rule anchors are lowercase: `#dxa010`.
*   ADR anchors retain numeric identifier.

***

## 7. Cross‑referencing & canonicalization

*   Public docs summarize and link to internal truths.
*   Rule references must link to the Analyzer Rule Catalog.
*   ADRs are immutable; supersession requires a new ADR.
*   Kernel Law is the canonical authority for constraints.

***

## 8. Review & ownership

*   Metadata must declare owners + reviewers.
*   Internal normative documents require approvals from Kernel Owner + Analyzers Owner.
*   PR templates enforce governance (API Freeze, DPI, Rule Catalog, ADR links).

***

## 9. CI validation

CI must enforce:

*   Broken link checks
*   Snippet compile checks
*   YAML metadata validation
*   Analyzer rule ID verification
*   Internal/public doc placement rules
*   Non‑Silence Axiom: no suppression in docs build

***

## 10. Examples

### 10.1 Public package page (Kernel)

````md
---
```yaml
id: pkg-kernel
title: Dx.Domain.Kernel
status: Accepted
audience: Public
owners: [DocsLead]
applies_to:
  packages: [Dx.Domain.Kernel]
  layers: [Public]
rule_ids: [DXA022]
api_links:
  - docs/public/api/Dx.Domain.Kernel/index.html
```
---

# Dx.Domain.Kernel

The Kernel defines results, diagnostics, invariants, and UTC‑only time semantics.
````

### 10.2 Internal governance page (API Freeze)

````md
---
```yaml
id: gov-api-freeze
title: API Freeze & Baselines
status: Accepted
audience: Maintainers
owners: [KernelOwner, ReleaseManager]
reviewers: [AnalyzersOwner]
rule_ids: [DXA040]
related:
  - docs/internal/specs/kernel.refactorization.specification.md
  - docs/internal/governance/kernel-law.md
```
---

# API Freeze & Baselines

Kernel public surface is frozen. New APIs require DXA040 approval.
````

### 10.3 DPI document (full metadata example)

````md
---
```yaml
id: dpi
title: Design Pressure Index (DPI)
status: Accepted
audience: Contributors
owners: [KernelOwner]
reviewers: [AnalyzersOwner, DocsLead]
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
canonical: docs/internal/governance/dpi.md
tags: [dpi, governance, invariants]

dpi_version: 1.0
derived_from:
  - MANIFESTO.md
  - NON_GOALS.md
applies_to:
  packages: [Dx.Domain.Kernel, Dx.Domain.Primitives, Dx.Domain.Analyzers]
  layers: [Internal]
interacts_with:
  - docs/internal/governance/kernel-law.md
  - docs/internal/governance/analyzers-law.md
  - docs/internal/governance/ApprovalWorkflow.md
  - docs/internal/governance/repo-conventions.md
constraints:
  - "No semantic expansion"
  - "Mechanical correctness only"
  - "Prefer edges over core"
```
---

# Dx.Domain Design Pressure Index (DPI)

...content...
````

***

## 11. Checklists

### Public page checklist

*   [ ] YAML front matter present
*   [ ] Examples follow Kernel constraints
*   [ ] Links to rule catalog when referencing DXA rules
*   [ ] API links included

### Internal page checklist

*   [ ] Normative sources referenced, not duplicated
*   [ ] Rule IDs validated
*   [ ] Owners and reviewers set
*   [ ] DPI metadata included if applicable

***

## 12. Versioning & changelogs

*   Public docs must reflect package TFMs and breaking changes.
*   Internal normative changes require ADR or documented review notes.
*   Docs versioning (`doc_version`) is optional but recommended.

***

## 13. Migration from existing content

*   Kernel Spec remains canonical; public pages summarize.
*   Analyzer public quickstart references internal rule catalog + editorconfig.
*   DPI must now include mandatory metadata as described above.

***

## 14. Appendix — Directory-level metadata (optional)

```yaml
defaults:
  audience: Public
  owners: [DocsLead]
  reviewers: [KernelOwner]
  snippet_test:
    tfl: "net8.0"
overrides:
  "packages/analyzers.md":
    reviewers: [AnalyzersOwner]
  "guides/*":
    tags: [guide]
```

***

This document is normative for documentation structure, metadata, review, and validation across the Dx.Domain repository. Kernel Law, Analyzers Law, and DPI remain higher‑order authorities when conflicts arise.

***
