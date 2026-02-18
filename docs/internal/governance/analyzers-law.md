<!-- path: docs/internal/governance/analyzers-law.md -->
---
```yaml
id: analyzers-law
title: Dx.Domain.Analyzers Law
status: Accepted
audience: Maintainers
owners: [AnalyzersOwner]
reviewers: [KernelOwner, ReleaseManager]
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
applies_to:
  packages: [Dx.Domain.Analyzers, Dx.Domain.Annotations]
  layers: [Internal]
canonical: docs/internal/governance/analyzers-law.md
related:
  - docs/internal/rules/analyzers/rule-catalog.md
  - docs/internal/governance/non-silence-axiom.md
  - docs/internal/specs/kernel.refactorization.specification.md
  - docs/internal/governance/kernel-law.md
tags: [analyzers, governance, rule-lifecycle, compatibility]
```
---

# Dx.Domain.Analyzers Law

**Purpose** — Define the *non‑negotiable* governance for **Dx.Domain.Analyzers**: rule identity and lifecycle, scope/authority behavior, compatibility discipline, configuration facts, CI enforcement, and documentation/test obligations. This Law preserves a predictable analyzer experience while enabling safe evolution without breaking existing consumers.  
It reinforces: **Rule IDs** (DXK*, DXA*), **scope model S0–S3**, **Non‑Silence Axiom**, and **API‑freeze links** where analyzers guard Kernel surface. 

---

## 1) Scope & Authority

1. **Scope model is authoritative**: S0 (Kernel), S1 (Domain Facades), S2 (Application), S3 (Consumer). Rules vary by scope; e.g., DXA010/020/022 apply to S1–S2; S0 is trusted.   
2. **Authority modes**: Definitional (Kernel), Structural (Primitives/Annotations), Constraining (Consumer), Observational (Kernel‑aware). Analyzers primarily operate in **Constraining** mode outside S0.   
3. **Configuration facts (not switches)**: `dx.scope.map`, `dx.scope.rootNamespaces`, and `dx_generated_markers` are recognized analyzer inputs; they classify, not disable. 

---

## 2) Non‑Silence Axiom (Enforcement)

Analyzers are **mandatory**; builds **must fail** when DX diagnostics signal contract violations. `.editorconfig` **cannot** suppress or downgrade DX diagnostics globally. Governance is imported once from `Directory.Build.targets` to ensure deterministic enforcement across repos. 

---

## 3) Rule Identity & Contracts

1. **Stable IDs**: DXK*** used for architectural/role violations (e.g., illegal dependencies); DXA*** for usage and flow discipline (e.g., construction authority, result discipline). IDs are **append‑only**, never repurposed.   
2. **Existing canon (examples, not exhaustive)**:
   - **DXA010** Construction Authority; **DXA020** Result Ignored; **DXA022** Result vs Throw; **DXA040** Kernel API Freeze.   
3. **Severity mapping** is part of the contract per scope; existing severities and default enforcement **must not change** without a formal compatibility process. 

---

## 4) Rule Lifecycle (Evolution Without Surprise)

Every *new* rule follows this lifecycle:

1. **Preview (Observational)** — Emits info‑level diagnostics in the *strictest* applicable scopes for **one minor release**; cannot be disabled; links to remediation docs. (Does not alter existing rules.)   
2. **Constrained (Enforced)** — Graduates to its target severity per scope in the **next** minor release; public changelog + migration notes required.   
3. **Stability** — Once enforced, the rule’s ID, title, and default severity are **frozen**. Future refinements are limited to bug‑fix precision, false‑positive reduction, and performance, not semantic drift. 

> This lifecycle respects the **Non‑Silence Axiom** by never allowing suppression, while still offering an *observational* ramp‑in for new rules. 

---

## 5) Configuration & Scope Resolution

- **Mandatory keys** (AnalyzerConfig / `.editorconfig`):  
  `dx.scope.map`, `dx.scope.rootNamespaces`, `dx_generated_markers`. These drive **ScopeResolver**, **ResultFlowEngine**, and **GeneratedCodeDetector** behavior.   
- **Optional**: Rule‑specific handler/terminalizer config for Result flow (registered via `HandlerRegistry`).   
- **Resolution precedence**: AnalyzerConfig facts → `[assembly: DxLayer("…")]` → assembly name → default S3 (fail‑open). 

---

## 6) Performance & Reliability

- **Budget**: < 5ms per method analysis on average; *no allocations* on the hot path where feasible.   
- **Fail‑open** on infra issues: analyzer infra failures must **not** break builds spuriously; diagnostics only when semantic certainty is reached. 

---

## 7) Documentation & Traceability

- **Rule Catalog** is canonical: intent, scope behavior, examples, and links to quick‑fix/migration guides.   
- **Changelog entries** for each new rule/severity change; **API baseline** for the analyzer assembly (PublicAPI.Shipped/Unshipped).  
- **Traceability**: Each rule change references its Refactor Proposal ID, compatibility matrix entry, and CI run. (See templates in this pack.)

---

## 8) Interlock with Kernel Governance

- **DXA040 API Freeze**: must flag unauthorized Kernel public surface additions; CI pairs the rule with API diff baseline gate.   
- Kernel laws and admission tests remain authoritative; analyzers **constrain** consumers to align with those laws (never the reverse). 

---

## 9) Acceptance Criteria

- Analyzer suite runs with **DX\*** as errors where mandated; no repository‑level suppressions.   
- Scope mapping resolves consistently (S0–S3) for all assemblies in CI.   
- New rule introductions comply with the lifecycle; docs + migration notes shipped. 

## 1) `ChangeRequest_AnalyzersLaw.md`

```markdown
---
id: cr-analyzers-law
title: Change Request — Establish "Analyzers Law"
author: <your-name>
date: 2026-02-18
links:
  analyzers-law: docs/internal/governance/analyzers-law.md
  rule-catalog: docs/internal/rules/analyzers/rule-catalog.md
  non-silence: docs/internal/governance/non-silence-axiom.md
  analyzer-source: src/Dx.Domain.Analyzers/README.md
---

# Change Request — Establish “Analyzers Law”

**Executive summary (one line):** Create a formal, enforceable governance document (“Analyzers Law”) defining rule lifecycle, scope/authority, compatibility discipline, CI gates, and documentation requirements for Dx.Domain.Analyzers.

**Intent (one paragraph):** This adds a normative governance layer that *codifies* existing behavior (DXA010/020/022/040, scope model S0–S3, Non‑Silence Axiom) without changing public APIs or default diagnostics. It clarifies how new rules are introduced (Preview→Constrained), how configuration facts classify scope, and how CI couples analyzers with API baselines and documentation updates. Sources respected: analyzer rule set and infrastructure (ScopeResolver, ResultFlowEngine, HandlerRegistry) and governance ADRs. 
```

**Checklist**

*   [x] No behavior or API changes
*   [x] Law file added with metadata & citations
*   [x] Links to rule catalog, Non‑Silence, Kernel spec

***

## 2) `PreservationStatement.md`

```markdown
---
id: preservation-statement
title: Preservation Statement — Dx.Domain.Analyzers
status: Accepted
---

# Preservation Statement

**Must not be altered without explicit approval:**

1) **Rule IDs and semantics**  
   - DXA010 (Construction Authority), DXA020 (Result Ignored), DXA022 (Result vs Throw), DXA040 (Kernel API Freeze), DXK00* family (role/dependency).  
   - *Reason:* Stable contracts consumed across many repos; changing IDs, default severities, or scopes risks breaking builds. 

2) **Scope model (S0–S3) & resolution facts**  
   - `dx.scope.map`, `dx.scope.rootNamespaces`, `dx_generated_markers` and resolution precedence.  
   - *Reason:* Deterministic classification and predictable analyzer behavior. 

3) **Non‑Silence Axiom**  
   - No global downgrade/suppression of DX diagnostics via `.editorconfig`.  
   - *Reason:* Governance integrity and deterministic builds. 

4) **Fail‑open & perf budgets**  
   - < 5ms/method average; infra failures must not hard‑fail analysis.  
   - *Reason:* Developer experience and CI stability. 

5) **Public Analyzer API surface**  
   - Analyzer types/namespaces that appear in PublicAPI baseline.  
   - *Reason:* ABI compatibility for downstream tooling; enforced by API baseline checks.
```

**Checklist**

*   [x] Items enumerated with reasons
*   [x] Source citations added

***

## 3) `RefactorProposal_TEMPLATE.md`

```markdown
---
id: refactor-proposal-template
title: Refactor Proposal Template
status: Draft
---

# Refactor Proposal — <Short Title>

## 1. Problem Statement
Describe the specific deficiency (correctness, stability, performance, compatibility, ergonomics).

## 2. Proposed Change
Summarize code/doc changes. **No public API or diagnostic behavior changes** unless justified.

## 3. Why This Elevates the Framework
- Correctness: <measurable effect>
- Stability: <impact>
- Performance: <expected % or absolute budget>
- Compatibility: <no breaks / opt-in>
- Ergonomics: <reduced friction>

## 4. Risk Assessment
- Technical risks
- False positive/negative risk (for analyzers)
- Mitigations

## 5. Rollback Plan
Describe how to revert the change safely.

## 6. Estimated Effort
Story points: <N>

## 7. Test Plan
- Unit tests (analyzer diagnostics, code‑fix if any)
- Integration (scope resolution, solution‑level)
- Performance (per‑method analysis time)
- Compatibility (PublicAPI baseline, golden diagnostics)

## 8. Required Approvals
- Kernel Owner (if rule touches Kernel surface)
- Analyzers Owner
- Release Manager

## 9. Acceptance Criteria
Concrete, testable conditions to declare success.

## 10. Links
- Source docs/respected contracts (IDs, scope model, Non‑Silence)  
  (see docs/internal/rules/analyzers/rule-catalog.md; docs/internal/governance/non-silence-axiom.md) 
```

**Executive summary:** A reusable, test‑first template with approvals and rollbacks.  
**Checklist:** problem → proposal → benefits → risks → tests → approvals → AC.

***

## 4) `DocsUpdatePlan.md`

````markdown
---
id: docs-update-plan-analyzers-law
title: Documentation Update Plan — Analyzers Law
status: Proposed
owners: [DocsLead]
reviewers: [AnalyzersOwner]
---

# Documentation Update Plan

## Targets & Scope

1) **Analyzers Law (new)**  
   - *Path:* docs/internal/governance/analyzers-law.md  
   - *Edits:* N/A (new)  
   - *Acceptance:* Metadata present; citations to rule catalog & Non‑Silence; lifecycle stated. 

2) **Rule Catalog**  
   - *Path:* docs/internal/rules/analyzers/rule-catalog.md  
   - *Edits:* Ensure each rule lists scope behavior S0–S3, examples, performance notes, and migration links. 

3) **Public Analyzers Guide**  
   - *Path:* docs/public/packages/analyzers.md  
   - *Edits:* Consumer‑facing overview; `.editorconfig` examples for scope mapping (classification facts only). 

4) **Kernel Law (cross‑link)**  
   - *Path:* docs/internal/governance/kernel-law.md  
   - *Edits:* Cross‑link DXA040 & API baseline policy section. 

## Sample Diff Summary (illustrative)

```diff
- See analyzers overview for rule lifecycle.
+ See ../governance/analyzers-law.md for the normative rule lifecycle (Preview→Constrained).
````

## Acceptance Criteria

*   All pages have YAML front matter.
*   Links resolve; docs lints pass.
*   Examples compile under net8.0; snippets tested in CI. (Per repo TFMs) 

````

**Executive summary:** Update 4 docs to align with the Law; add cross‑links.  
**Checklist:** targets, edits, sample diff, acceptance.

---

## 5) `CompatibilityMatrix.csv`

```csv
"ChangeId","Area","Rule/ID","AffectedPackages","PublicTypesOrMembers","DefaultSeverity","ExpectedConsumerAction","Mitigation/Compatibility","Notes"
"RP-000","Docs only","N/A","N/A","N/A","N/A","None","N/A","Establishes Analyzers Law; no behavior change"
"RP-001","Analyzer infrastructure","ScopeResolver precision","Dx.Domain.Analyzers","Dx.Domain.Analyzers.ScopeResolver","N/A","None","Fail-open maintained; classification tests added","No runtime coupling to Kernel remains" 
"RP-002","Diagnostics docs","DXA010/020/022/040 docs","Dx.Domain.Analyzers","N/A","N/A","Review docs only","No rule behavior change","Rule catalog receives scope tables" 
````

**Executive summary:** Tracks consumer impact; today all rows are “docs only / infra precision”, no action.  
**Checklist:** IDs, packages, severity, action, mitigation.

***

## 6) `CI-Jobs.yml` and `PR_TEMPLATE.md`

```yaml
# path: .github/workflows/ci-analyzers.yml
name: CI (Analyzers & Docs Governance)
on:
  pull_request:
    branches: [ main ]
jobs:
  build-and-validate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - name: Restore
        run: dotnet restore
      - name: Build (warnings as errors for DX*)
        run: dotnet build -warnaserror
      - name: Run analyzer tests
        run: dotnet test src/Dx.Domain.Analyzers.Tests -c Release --no-build
      - name: Public API baseline check (Analyzers)
        run: |
          dotnet tool restore
          # Example using PublicApiGenerator or PublicApiAnalyzer workflow:
          # Validate PublicAPI.Shipped.txt for Dx.Domain.Analyzers
          dotnet build src/Dx.Domain.Analyzers/Dx.Domain.Analyzers.csproj -p:GeneratePublicApi=true
      - name: Docs lint (broken links + metadata)
        run: |
          scripts/docs-lint.sh  # checks YAML front matter, links under docs/
      - name: Snippet compile (public docs)
        run: |
          scripts/docs-snippets-compile.sh docs/public net8.0
      - name: Block unapproved breaking changes
        run: scripts/api-diff-gate.sh  # fails on public API diff unless PR label "approved-breaking" present
```

```markdown
<!-- path: .github/PULL_REQUEST_TEMPLATE.md -->
# Dx.Domain — PR Template

**Refactor Proposal ID**: (link to `RefactorProposal_<id>.md`)  
**Change Type**: Docs | Analyzer Infra | Rule Introduction | Rule Bugfix | Other

## Summary
One paragraph summary of what changed and why.

## Compatibility Matrix
Link: `docs/internal/governance/CompatibilityMatrix.csv` (row for this change)

## Evidence
- [ ] Analyzer unit tests passing
- [ ] Scope resolution integration tests passing
- [ ] Public API baseline clean (or diff justified)
- [ ] Docs lint + snippet compile passing

## Approvals
- [ ] Analyzers Owner
- [ ] Kernel Owner (if touches Kernel surface checks, e.g., DXA040)
- [ ] Release Manager

## Links
- Analyzers Law: `docs/internal/governance/analyzers-law.md`
- Rule Catalog: `docs/internal/rules/analyzers/rule-catalog.md`
- Non‑Silence Axiom: `docs/internal/governance/non-silence-axiom.md`
```

**Executive summary:** CI blocks unapproved API diffs and enforces analyzers & docs gates.  
**Checklist:** build+test, API baseline, docs lint, snippet compile, approvals.

***

## 7) `TestPlan.md`

```markdown
---
id: analyzers-test-plan
title: Test & Validation Plan — Dx.Domain.Analyzers
status: Accepted
owners: [AnalyzersOwner]
---

# Test & Validation Plan

## 1. Unit Tests
- Diagnostic creation (IDs, titles, severities) for DXA010/020/022/040 remain unchanged.  
- Flow tests using **ResultFlowEngine** for DXA020 (ignored/handled/propagated paths). 
- Scope classification tests for S0–S3 via **ScopeResolver** using `dx.scope.map`. 
- Generated code exemptions via `dx_generated_markers`. 

## 2. Integration Tests
- Multi‑project solution with S0 (Kernel), S1, S2, S3 assemblies: ensure rule matrices fire only where expected. 
- API‑freeze interlock: ensure DXA040 fires on new Kernel public API without approval. 

## 3. Performance
- Benchmark per‑method analysis: **< 5ms** average; assert no regression. 

## 4. Docs Snippet Compile
- Compile C# snippets in public pages with `net8.0`; ensure no drift with examples. (See docs update plan.) 

## 5. Acceptance Criteria
- All above tests pass in CI; no new false positive buckets identified in sample repos; PublicAPI baseline unchanged.
```

**Executive summary:** Guard semantics, scope, perf, and docs.  
**Checklist:** unit/integration/perf/docs/AC.

***

## 8) `ApprovalWorkflow.md`

```markdown
---
id: analyzers-approval-workflow
title: Approval & Sign-off Workflow — Dx.Domain.Analyzers
status: Accepted
---

# Approval & Sign-off Workflow

## Roles
- **Analyzers Owner** — primary authority on rule set and infrastructure.
- **Kernel Owner** — signs off when analyzer changes influence Kernel gates (DXA040 or scope S0 behavior). 
- **Release Manager** — enforces process, schedules staged rollouts.

## Required Artifacts
- Refactor Proposal (completed template)
- Passing CI artifacts (tests, API baseline, docs)
- Compatibility Matrix entry
- Migration notes (if any)

## Path to Merge
1. Author raises PR with proposal ID and artifacts.
2. CI green including API baseline and docs lint.
3. Approvals: Analyzers Owner (+ Kernel Owner if applicable) + Release Manager.
4. Merge with label `approved` (or `approved-breaking` only when matrix indicates and migration exists).
```

**Executive summary:** Clear roles & artifacts; ensure DXA040 interlock.  
**Checklist:** artifacts, CI green, approvals, labels.

***

## 9) `TraceabilityLog.md`

```markdown
---
id: analyzers-traceability-log
title: Traceability Log Template — Dx.Domain.Analyzers
status: Accepted
---

# Traceability Log

| Date       | ChangeId | Title                         | PR # | Reviewers                         | Decision | Notes |
|------------|----------|-------------------------------|------|-----------------------------------|----------|-------|
| YYYY-MM-DD | RP-000   | Establish Analyzers Law       | 123  | AnalyzersOwner; ReleaseManager    | Accepted | Docs-only |
| YYYY-MM-DD | RP-001   | ScopeResolver precision tweak | 130  | AnalyzersOwner; KernelOwner       | Accepted | No behavior change |

**Reviewer checklist (per entry):**
- [ ] Matches Refactor Proposal
- [ ] Compatibility Matrix updated
- [ ] CI artifacts attached
- [ ] Docs updated/linked (Rule Catalog, Law)
```

**Executive summary:** Single file to record intent → decision with reviewer checklist.  
**Checklist:** columns present; reviewer tick‑list included.

***

## 10) Examples (code & baselines)

### 10.1 Diagnostic descriptor (no semantic change)

```csharp
// Example, not changing shipped IDs or severities:
static readonly DiagnosticDescriptor DXA020 =
    new(
        id: "DXA020",
        title: "Result Ignored",
        messageFormat: "A Result produced by '{0}' is ignored.",
        category: "Dx.Domain.Analyzers",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Ensure Result<T> is handled, propagated, or terminalized."
    );
```

> DXA020 contract and severity remain unchanged; tests assert ID, title, severity, and scope behavior.

### 10.2 Public API baseline entry (Analyzers)

    # path: src/Dx.Domain.Analyzers/PublicAPI.Shipped.txt
    ~namespace Dx.Domain.Analyzers
    Dx.Domain.Analyzers.DXA010Analyzer
    Dx.Domain.Analyzers.DXA020Analyzer
    Dx.Domain.Analyzers.DXA022Analyzer
    Dx.Domain.Analyzers.DXA040ApiFreezeAnalyzer

> API baseline is append‑only; adding a public type requires proposal + approvals; removals are breaking and blocked by CI.

***

### One‑line executive summaries

*   **Analyzers Law:** Codifies rule lifecycle, scope, and compatibility without behavior changes. 
*   **Change Request:** Adopt the Law file; align docs, CI, and approval workflow.
*   **Preservation Statement:** Freeze rule IDs/semantics, scope model, Non‑Silence, perf budgets. 
*   **Templates & Plans:** Provide repeatable, test‑first refactor governance; no API drift.

***

### Final checklist (acceptance)

*   [x] **Analyzers Law** with metadata and citations
*   [x] **All deliverables** (CR, Preservation, Proposal template, Docs plan, Compat matrix, CI YAML, PR template, Test plan, Approval workflow, Traceability log)
*   [x] **No changes** to existing analyzer behavior or public API
*   [x] **Citations** to rule canon, scope model, and governance sources maintained (DXA010/020/022/040; S0–S3; Non‑Silence Axiom) 
