# Agent Instructions

## Dx.Domain Enforcement Refactoring (Repository-Local)

### Role of the Agent

You are operating **inside the `dx.domain` repository only**.

You are **not** responsible for:

* implementing `dx.templates`
* generating `.dx/invariants.json`
* defining the final DXT schema

You **are** responsible for ensuring that **dx.domain fully defines, enforces, and defends the ground rules** that `dx.templates` and consumers must obey.

> `dx.templates` depends on `dx.domain` rules — not the other way around.
> Lack of template implementation is **never** a valid reason to stop work in this repo.

---

## Invalid Stopping Reasons (Explicit)

You MUST NOT stop work for any of the following reasons:

* “DXT schema is not finalized”
* “dx.templates work is out of scope”
* “Consumers cannot yet emit `.dx/invariants.json`”
* “Template CI is not implemented yet”

All of the above are **downstream concerns**.

You must proceed by:

* defining strict expectations,
* enforcing fail-fast behavior,
* and providing deterministic diagnostics when expectations are unmet.

---

## Valid Stopping Reasons (Only These)

You MAY stop **only if all repository-local invariants below are satisfied**.

If any checklist item is incomplete, stopping is a failure.

---

## Repository-Local Objectives (What You Must Finish)

### Objective A — DXT Contract Enforcement (Without Templates)

You must fully implement **DXT enforcement behavior** even if no real DXT file exists yet.

#### Required Actions

* Treat `.dx/invariants.json` as an **opaque external contract**
* Do **not** assume its structure beyond what is strictly required
* Enforce the following behaviors:

##### A.1 Presence Semantics

* If project is **consumer scope** and:

  * no `.dx/invariants.json` is found → **emit deterministic DX error**
* If project is **authority scope**:

  * analyzers must **never attempt to locate or read DXT**

##### A.2 Authority Immunity

* Confirm in code that **no analyzer path**:

  * probes for DXT
  * fails
  * or logs diagnostics
    when running in authority scope

> You are enforcing *absence tolerance* for authority and *presence requirement* for consumers.

---

### Objective B — Analyzer Rule Completeness (Scope-Exact)

You must verify and, if necessary, harden **every DX analyzer** so that:

#### Required Actions

* Every DXA* and DXK* rule explicitly checks:

  * authority vs consumer
  * test vs non-test
* No rule relies on:

  * naming heuristics
  * assembly name matching
  * folder structure guesses

##### B.1 Zero False Positives for Authority

* Authority projects must produce **zero consumer-discipline diagnostics**
* This is a hard invariant; “unlikely” is insufficient

##### B.2 Deterministic Failure for Consumers

* Consumer misuse must:

  * always produce a DX diagnostic
  * never silently pass
  * never degrade into warnings when forbidden

---

### Objective C — Dependency Geometry Enforcement (Template-Agnostic)

You must enforce **strict dependency rules** even without DXT contents.

#### Required Actions

* Encode **hard forbids** that do NOT require DXT data, at minimum:

  * Domain → Infrastructure
  * Contracts → Kernel
  * Consumer → internal `dx.domain.*` packages
* These must:

  * apply only to consumer scope
  * fail deterministically
  * be impossible to suppress

> DXT may *add* allow-lists later, but the base physics live here.

---

### Objective D — MSBuild Governance Finalization (Repo-Local)

You must complete governance **as far as it applies to `dx.domain` itself**.

#### Required Actions

* Verify that:

  * `dx.domain` builds successfully **with analyzers enabled**
  * no DXT is required
  * no consumer-only Non-Silence rules accidentally trip

* Explicitly document (in repo docs):

  * which governance rules are consumer-only
  * which never apply to authority repos

> This prevents future regressions when governance evolves.

---

### Objective E — Analyzer Distribution Invariants

You must ensure distribution rules are **self-evident and non-regressible**.

#### Required Actions

* Verify:

  * analyzers are included as assets in Kernel / Primitives / Annotations packages
  * analyzers are **not packable standalone**
* Add explicit safeguards (build or doc-level) preventing:

  * accidental publication of `Dx.Domain.Analyzers`

---

### Objective F — CI Readiness (dx.domain Only)

You are **not** implementing template CI.

You **are** responsible for ensuring that:

* dx.domain CI:

  * runs analyzers
  * passes with no DXT present
  * fails if authority code accidentally triggers consumer rules

If CI cannot express this yet, document the invariant **explicitly**.

---

## Required Output Artifacts

Before stopping, you must produce **all** of the following inside the repo:

1. **Analyzer behavior guarantees** (documented):

   * authority vs consumer vs test
2. **DXT enforcement semantics** (documented):

   * presence required for consumers
   * absence tolerated for authority
3. **Non-negotiable dependency physics** (documented + enforced)
4. **Distribution guarantees** for analyzers
5. **Clear “out of scope” notes** explaining what `dx.templates` must implement — without blocking this repo

---

## Completion Checklist (Stop Condition)

You may stop **only when all are true**:

* [ ] No analyzer path depends on template implementation
* [ ] All consumer-discipline rules are scope-exact
* [ ] Authority code is immune to DXT and consumer rules
* [ ] Missing DXT in consumers fails deterministically
* [ ] dx.domain builds clean with analyzers enabled
* [ ] Analyzer packaging cannot regress accidentally
* [ ] Remaining work is **strictly downstream** (templates / consumers)

If any box is unchecked, you must continue.

---

## Final Instruction (Non-Negotiable)

> **dx.domain defines the law.
> dx.templates comply with it.
> The absence of a citizen does not suspend the constitution.**

---

## Repository Outputs

- Repository-local enforcement guarantees are documented in `docs/enforcement-guarantees.md`.

---

## Refactorization Progress (dx.domain repo)

### Completed

- Scope resolution no longer uses assembly-name or path heuristics; authority/consumer are resolved from build metadata only.
- Authority layer metadata is set on repo-local projects and tests to avoid DXT enforcement.
- DXT enforcement is authority-immune and deterministic for consumers via `DXT004`.
- Consumer dependency physics now forbid internal `Dx.Domain.*` package references via `DXK009`.
- Kernel API freeze enforcement is opt-in via `build_property.DxKernelApiFreeze` and skips tests.
- Repository-local enforcement guarantees are documented in `docs/enforcement-guarantees.md`.
- Governance rules (`DXB001`-`DXB003`) are documented as consumer-only and excluded from authority repositories.
- Analyzer packaging is guarded by defaults and DXB004 to prevent standalone publication.
- Kernel pack now self-heals missing `buildTransitive` assets before packing.

Proceed accordingly.
