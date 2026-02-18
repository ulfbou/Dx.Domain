<!-- path: docs/internal/governance/repo-conventions.md -->
---
```yaml
id: repo-conventions
title: Repository Conventions — Branches, Commits, PRs, Issues & Releases
status: Accepted
audience: Contributors
owners: [Maintainers, ReleaseManager]
reviewers: [KernelOwner, AnalyzersOwner, DocsLead]
last_reviewed: 2026-02-18
next_review_date: 2026-05-18
applies_to:
  packages:
    - Dx.Domain.Kernel
    - Dx.Domain.Analyzers
    - Dx.Domain.Annotations
    - Dx.Domain.Primitives
    - Dx.Domain.Facts
  layers: [Internal, Public]
canonical: docs/internal/governance/repo-conventions.md
related:
  - docs/internal/governance/dpi.md
  - docs/internal/governance/kernel-law.md
  - docs/internal/governance/analyzers-law.md
  - docs/internal/governance/ApprovalWorkflow.md
  - docs/internal/governance/DocsUpdatePlan.md
  - docs/internal/governance/PreservationStatement.md
  - docs/internal/rules/analyzers/rule-catalog.md
  - docs/internal/specs/kernel.refactorization.specification.md
  - docs/internal/governance/docs-conventions.md
tags: [governance, branching, commits, pr, issues, labels, releases]
```
---

# Repository Conventions — Branches, Commits, PRs, Issues & Releases

This document defines **uniform conventions** for day‑to‑day delivery: branch names, commit messages, pull requests, issue/bug handling, labels, milestones, release tags, backports, and maintenance. All items here reinforce our governance (DPI, Kernel Law, Analyzers Law), documentation conventions, and CI/approval gates.

> **Scope.** Applies to all packages in this repo. When a convention conflicts with a law/spec, the **law/spec wins** and this document must be amended.

---

## 1) Branching Strategy (Trunk‑based with short‑lived branches)

- **Default branch:** `main` (always releasable).
- **Short‑lived branches** for work; merge promptly via PR (squash‑merge).
- **Release maintenance branches:** `release/x.y` (only when we need to ship hotfixes for `x.y.z` after `main` moved on).
- **No long‑lived `develop`** branch.

### 1.1 Branch naming

Use **lowercase**, **slashes** as separators, and **kebab‑case** segments. When applicable, include **issue/PR ID** or **DX rule ID**.

```

feature/<area>/<slug>               # new feature
fix/<area>/<issue-id>-<slug>        # bug fix linked to issue
docs/<area>/<slug>                  # docs-only changes
rule/<dx-id>-<slug>                 # analyzer rule work (e.g., rule/dxa020-fp-reduction)
refactor/<area>/<slug>              # internal refactors (no API/behavior change)
chore/<topic>/<slug>                # infra/CI/build
spike/<topic>/<slug>                # exploratory (must be short-lived)
release/<major>.<minor>             # maintenance branch for a minor line (e.g., release/1.3)
hotfix/<major>.<minor>.<patch>      # targeted fix branch from release/<major>.<minor>
backport/<target-branch>/<pr-id>    # backport of a merged PR

```

**Area** examples: `kernel`, `analyzers`, `docs`, `governance`, `persistence-adapters`, `generators`.

---

## 2) Commit Message & Title Conventions

We use a **Conventional Commits**–style prefix to simplify changelog and automation. Keep **subject lines ≤ 72 chars**; wrap bodies at ~100. Always link issues/PRs (e.g., `#123`), ADRs (by ID), and DX rule IDs if relevant.

### 2.1 Allowed types

```

feat:        # new feature in public API or new analyzer capability (see impact matrix)
fix:         # bug fix (runtime, analyzer, or docs correctness)
docs:        # documentation only (no code)
refactor:    # code change that neither fixes a bug nor adds a feature (no behavior change)
perf:        # performance improvement (include microbench evidence if kernel/analyzers)
test:        # adding or correcting tests
build:       # build system or tooling (packaging, target frameworks, versions)
ci:          # CI workflow/config changes
chore:       # housekeeping (formatting, non-functional updates)
revert:      # reverts a previous commit
rules:       # analyzer rule work (introduce/adjust docs/tests for DXA\*/DXK\*)

```

### 2.2 Scope (optional but recommended)

```

feat(kernel): ...
fix(analyzers): ...
docs(governance): ...
rules(dxa020): reduce false positives in S2; add test coverage

```

### 2.3 Subject and body

- **Subject:** imperative, no trailing period, ≤72 chars.
- **Body (optional):** “what/why,” migration notes (if any), and **references**:
  - Issue/PR: `Refs #123` / `Fixes #456`
  - ADR/Spec/Law: `See ADR-0004`, `See kernel-law.md`
  - Analyzer rule(s): `Impacts: DXA020 (S1,S2)`

**Examples**

```

feat(kernel): expose DomainTime.FromUtcTicks for structural conversions
fix(analyzers): DXA020 false positive on Match-return; add flow test (Refs #612)
docs(governance): add API freeze steps; link DXA040 interlock
rules(dxa010): clarify construction authority in rule-catalog; add examples

```

> **Export/reporting note:** if you export commits externally, the canonical export order is *branch/date/time/subject*. (Keep that mapping in any tooling/scripts.)

---

## 3) Pull Requests (PRs)

- **Merge strategy:** **Squash‑and‑merge** only (keeps linear history and aids changelog generation).
- **PR Title:** mirrors the main commit subject rules (type(scope): subject).
- **PR Templates:** choose per area (enforced by `.github/PULL_REQUEST_TEMPLATE/*`):
  - `analyzers.md` for analyzer/rule/docs‑of‑rules changes.
  - `kernel-dpi.md` for Kernel/Abstractions and DPI‑governed changes.
- **Required content** (as per templates):
  - **Summary**: one paragraph.
  - **Compatibility Matrix link**: row in `docs/internal/governance/CompatibilityMatrix.csv`.
  - **Evidence**: tests passing, public API baseline check outcomes, docs lint/snippet compile.
  - **Approvals**: Analyzers Owner / Kernel Owner / Release Manager per `ApprovalWorkflow.md`.
  - **Links**: Analyzers Law, Rule Catalog, Non‑Silence Axiom (for analyzers); Kernel Law, Kernel Spec, Admission Test (for kernel).

**DoR (Definition of Ready) for PRs**

- Linked issue or Refactor Proposal ID (if non‑trivial).
- Labels set (area/type/scope/priority).
- Rule IDs included in title/body when analyzer rules are affected.

**DoD (Definition of Done) for PRs**

- CI green (tests, baselines, docs lint/snippets).
- Reviewer approvals obtained (as required by area).
- Compatibility Matrix updated.
- Docs updated (public + internal) where applicable.

---

## 4) Issues, Bugs, RFCs, and ADRs

### 4.1 Issue types

We track issues with **labels** and one of the standard templates (see §6):

- `type:bug` — incorrect behavior or diagnostics, regression, broken examples.
- `type:feat` — feature request (rare for Kernel; common for analyzer docs/rules).
- `type:refactor` — internal improvements (no API change).
- `type:docs` — documentation requests/corrections.
- `type:governance` — policy/process/gating work.
- `type:rfc` — proposals for meaningful changes that need design discussion → may result in an ADR.

**Bug minimum fields:** repro steps (code if applicable), expected vs actual, environment, scope (S0–S3 if analyzer), rule IDs involved (if any), links to failing CI.

### 4.2 RFC → ADR policy

- Non‑trivial architectural changes or governance updates **must** start as `type:rfc` and can be promoted to an **ADR** upon acceptance.
- ADR format: `docs/internal/adr/ADR-YYYY-<short-title>.md` (immutable; supersede by new ADR).

---

## 5) Labels, Milestones, and Assignees

### 5.1 Labels (suggested baseline)

**Area**  
`area:kernel`, `area:analyzers`, `area:annotations`, `area:primitives`, `area:facts`, `area:docs`, `area:governance`, `area:ci`, `area:build`

**Type**  
`type:bug`, `type:feat`, `type:refactor`, `type:docs`, `type:rfc`, `type:chore`

**Priority/SLA**  
`prio:P0` (urgent/breakage), `prio:P1` (next release), `prio:P2` (backlog), `prio:P3` (nice‑to‑have)

**Impact**  
`impact:breaking`, `impact:behavioral`, `impact:docs-only`

**Scope (Analyzers)**  
`scope:S0`, `scope:S1`, `scope:S2`, `scope:S3`

**Rules**  
`rule:DXA010`, `rule:DXA020`, `rule:DXA022`, `rule:DXA040`, `rule:DXK002`, `rule:DXK007`, …

**Governance**  
`needs-approval`, `approved`, `approved-breaking`

**Status**  
`status:blocked`, `status:ready-for-review`, `status:needs-info`, `good-first-issue`

### 5.2 Milestones

- Use **SemVer** milestones: `vX.Y.Z` or `vX.Y` (for a minor train).
- Assign issues/PRs to the **earliest feasible milestone**; re‑balance during triage.

### 5.3 Assignees / CODEOWNERS

- Keep **CODEOWNERS** synced with governance roles (Kernel Owner, Analyzers Owner, Docs Lead).  
- PRs touching **laws/specs/rules** must auto‑request the relevant owners.

---

## 6) Templates (Issues & PRs)

- **PR templates** live under `.github/PULL_REQUEST_TEMPLATE/`:
  - `analyzers.md`, `kernel-dpi.md`, and a small chooser at `.github/PULL_REQUEST_TEMPLATE.md`.
- **Issue templates** (suggested):
  - `bug_report.md` — includes scope (S0–S3) and rule IDs if analyzer.
  - `feature_request.md` — intended outcome, acceptance criteria, migration concerns.
  - `rfc.md` — problem statement, alternatives, impact analysis; link to prospective ADR.
  - `docs_request.md` — page(s) to change, code snippet expectations, links.

> Keep templates concise; require links/evidence early to speed triage.

---

## 7) Release & Tagging Conventions

### 7.1 Tags

- **Runtime/Docs repo tags:** `vX.Y.Z`
- **Optional package‑specific tags** (if you cut independently): `analyzers-vX.Y.Z`, `kernel-vX.Y.Z` (use sparingly; prefer repo‑wide tags).

### 7.2 SemVer policy (aligned with laws/specs)

- **MAJOR**:
  - Breaking public API change (Kernel/Analyzers public surface).
  - Changing default **severity** of an **existing** analyzer rule in existing scopes (S1/S2) in a way that can break builds without opt‑in.
- **MINOR**:
  - New analyzer rule introduced as **Preview (Observational)** or promoted to **Enforced** (per **Analyzers Law** lifecycle) *with clear migration notes*.
  - New public API that is additive and accepted via the **API freeze** process (DXA040 approvals).
- **PATCH**:
  - Bug fixes, false‑positive reductions, perf improvements, docs corrections, CI/build tweaks.

### 7.3 Release branches & backports

- Create `release/x.y` only when you must support hotfixes after `main` advances.
- Hotfix process:
  1. Branch from `release/x.y`: `hotfix/x.y.z`
  2. PR → `release/x.y`; tag `vX.Y.Z` upon merge.
  3. **Backport** to `main`: `backport/main/<pr-id>` (automated or manual cherry‑pick).

---

## 8) Changelog & Documentation

- **Changelogs**: one file per package under `docs/public/changelog/` (`kernel.md`, `analyzers.md`, etc.). Group by release tag with sections: **Added**, **Changed**, **Fixed**, **Removed**, **Security**.
- **Docs updates** are **mandatory** for:
  - Any rule lifecycle event (Preview→Enforced): update **Rule Catalog**, public guide, and migration notes.
  - Any Kernel public API addition (after DXA040 approval): update **Kernel Law** references and public package page.
- **Docs lint & snippet compile** must pass in CI.

---

## 9) Analyzer Rule Lifecycle & Conventions (Quick Re‑state)

- **Preview (Observational)** for one minor: info‑level, **not suppressible**, link to remediation.
- **Constrained (Enforced)** next minor: final severity per scope; changelog + migration guide.
- **Stability**: after enforcement, rule ID/title/default severity are **frozen**; refine only for precision/perf.

**PR/Issue conventions when touching rules:**

- Branch name should include rule ID: `rule/dxa020-<slug>`.
- PR title should include `rules(dxa020): ...` or `fix(analyzers): DXA020 ...`.
- Update **CompatibilityMatrix.csv** and **Rule Catalog**.

---

## 10) CI & Gate Expectations

- **Warnings as errors** for DX* where mandated by governance.
- **Public API baseline** checks for Kernel and Analyzers assemblies.
- **Docs checks**: front matter metadata, link integrity, and snippet compile.
- **No repository‑level suppression** of DX diagnostics (Non‑Silence Axiom).
- **Approvals** per `ApprovalWorkflow.md` for risky/breaking or Kernel/Analyzers surface changes.

---

## 11) Example: end‑to‑end flow (Analyzer rule false positive)

1. **Issue** opened: `type:bug`, `area:analyzers`, `rule:DXA020`, `scope:S2`, `prio:P1`.
2. **Branch**: `fix/analyzers/1234-dxa020-fp-when-match`.
3. **Commits**:  
   - `test(analyzers): add S2 flow sample reproducing DXA020 FP (Refs #1234)`  
   - `fix(analyzers): dxa020 ignore terminalized matches; add flow guard`
4. **PR** with template **analyzers.md**:
   - Evidence: tests green; no API diff; docs snippet compile.
   - CompatibilityMatrix row updated.
5. **Approvals**: Analyzers Owner.
6. **Merge** (squash); **Tag** next patch release; **Changelog** entry under `analyzers.md`.

---

## 12) Example: new Kernel API (additive, approved)

1. **RFC** → ADR; **DXA040** justification (`[ApprovedKernelApi("...")]`).
2. Branch: `feature/kernel/add-<api-name>`.
3. PR with **kernel-dpi.md** template:
   - Pass Kernel Admission checklist; **API baseline** shows additive change.
   - Docs: update Kernel package page and cross‑links.
4. Approvals: Kernel Owner + Analyzers Owner + Release Manager.
5. Merge; **MINOR** tag; changelog + migration notes.

---

## 13) Governance Alignment (references)

- **DPI**: changes must strengthen invariants or reduce misuse; not convenience.  
- **Kernel Law**: no I/O/logging/ambient context; UTC‑only time; failures as values; API freeze.  
- **Analyzers Law**: rule IDs stable; lifecycle; Non‑Silence Axiom; scope S0–S3; performance budgets.  
- **Docs Conventions**: YAML front matter; internal vs public split; snippet compile.

---

## 14) Checklists

**Branch & Commit (author)**  
- [ ] Branch name follows convention  
- [ ] Commit type(scope): subject (≤72 chars)  
- [ ] Body includes references (issues/ADR/rules) if needed

**PR (author)**  
- [ ] Correct PR template used  
- [ ] CompatibilityMatrix row added/updated  
- [ ] Evidence attached (tests, baselines, docs lint/snippets)  
- [ ] Docs updated (internal & public as needed)

**Review (maintainers)**  
- [ ] DPI/Kernel Law/Analyzers Law satisfied  
- [ ] No governance violations (Non‑Silence, API freeze)  
- [ ] Approvals complete per `ApprovalWorkflow.md`

---
