# Dx.Domain Release-Gating Implementation Plan

**Normative design:** [Release-gating specification](release-gating-specification.md)

## 1. Objective

Implement the minimum reusable release-gating infrastructure needed to make Dx.Domain ACCEPT READY, then extend it workstream by workstream until the release can be classified as `READY_FOR_PUBLICATION` and, after publication, `DONE`.

The plan favors direct correction of the production release path plus focused proving automation. It deliberately defers a generalized release-assurance platform.

## 2. Delivery principles

1. Correct known release-path deficiencies before generalizing infrastructure.
2. Add narrowly scoped proving tests alongside every correction.
3. Reuse one Python verification implementation locally and in CI/CD.
4. Keep Bash and PowerShell as no-logic launchers.
5. Keep GitHub Actions as orchestration rather than verification logic.
6. Require no arguments, credentials, or manual path discovery for default local verification.
7. Store executable code under `.dx/scripts/` and evidence under `.dx/verification/`.
8. Treat WS-001 and WS-002 as accepted inputs rather than duplicating their proof.
9. Do not create the final tag, publish packages, or run post-publication checks until the corresponding phase is authorized.

## 3. Workstream sequence

### 3.1 WS-003: Strict CI and mandatory tests

**Goal:** Establish that a successful strict-CI result means all mandatory builds and tests actually executed and passed.

Implement:

- the shared Python process and evidence core;
- repository and environment discovery;
- strict .NET command construction;
- explicit mandatory-project inventory;
- TRX parsing;
- zero-test and skipped-test rejection;
- source-immutability checks;
- Bash and PowerShell launchers;
- shared local and CI invocation;
- evidence upload on success and failure;
- removal of explicit warning relaxation; and
- removal of unconditional synthetic success.

Acceptance is defined by [the specification's WS-003 criteria](release-gating-specification.md#23-initial-acceptance-criteria).

### 3.2 WS-004: Authoritative candidate and package validation

**Goal:** Produce exactly four packages once and prove their identities and contents independently.

Implement:

- clean-tree candidate production;
- exact release version and build order;
- scoped output cleanup;
- authoritative Analyzer build;
- exact four-package output;
- candidate manifest with package ID, version, filename, size, SHA-256, and signing disposition;
- independent ZIP and nuspec validation;
- framework and dependency validation;
- package README validation;
- exact Analyzer placement;
- Analyzer byte identity across packages and against the authoritative build output;
- mutation tests; and
- immutable candidate artifact upload.

Relevant authority:

- ACCEPT READY package criteria (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`)
- Definition of Done candidate set (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)
- Definition of Done Analyzer contract (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)

### 3.3 WS-005: Package-backed consumers and Analyzer verification

**Goal:** Prove real consumption of the retained candidate without source-project references or repository build inheritance.

Implement:

- generated external consumer workspaces;
- isolated NuGet configuration and caches;
- Annotations compilation under `netstandard2.0`;
- Kernel, Primitives, and Facts restore/build/run under `net8.0`, `net9.0`, and `net10.0`;
- combined four-package consumers under all three runtime frameworks;
- representative public behavior;
- package-backed Analyzer activation through each carrier package;
- combined-package Analyzer load validation; and
- exact effective-diagnostic counting.

Relevant authority:

- ACCEPT READY consumer fixtures (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`)
- Definition of Done consumer matrix (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)
- Definition of Done Analyzer activation (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)

### 3.4 WS-006: CI/CD identity, promotion, and publication safety

**Goal:** Make strict CI an unavoidable precondition and preserve one immutable candidate through publication.

Implement:

- authorized-tag and full-commit verification;
- strict-CI dependency for the exact commit;
- closure of manual bypass paths;
- one named candidate artifact;
- manifest verification at every downstream boundary;
- prohibition of rebuilding, repacking, or post-manifest signing;
- manifest-based package iteration;
- per-package outcome records;
- partial-publication failure handling;
- non-production failure simulation after zero, one, two, and three successful operations;
- immutable retry;
- removal of blind duplicate skipping; and
- suppression of GitHub Release and documentation deployment after failed or partial publication.

Relevant authority:

- ACCEPT READY CI-to-CD control (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`)
- ACCEPT READY artifact identity (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`)
- ACCEPT READY failure handling (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`)
- Definition of Done workflow controls (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)

### 3.5 WS-007: Documentation reconciliation and acceptance

**Goal:** Make public and package documentation accurately describe the locked four-package release and compile against candidate packages.

Implement:

- exactly four published-package descriptions;
- package-selection guidance;
- exact `0.1.0-alpha` installation commands;
- role-specific target-framework claims;
- Analyzer distribution through the four carrier packages;
- removal of standalone Analyzer installation guidance;
- package-specific READMEs;
- direct dependency statements;
- meaningful success and failure examples;
- package-backed example compilation;
- link, route, and DocFX validation; and
- structured human semantic review.

Relevant authority:

- ACCEPT READY global documentation (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`)
- ACCEPT READY package documentation (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`)
- Definition of Done global documentation (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)
- Definition of Done package documentation (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)
- [Documentation authority](documentation-authority.md)

### 3.6 WS-008: External prerequisites

**Goal:** Collect and validate the external facts and authorized decisions required before publication without exposing secrets.

Implement:

- package-ID control and version availability checks;
- publishing identity permission evidence;
- GitHub ruleset, required-check, and environment evidence;
- production approval evidence;
- release-owner attestations;
- security and legal dispositions where applicable; and
- signing-policy acceptance.

Relevant authority:

- ACCEPT READY external inputs (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`)
- Definition of Done external prerequisites (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)

### 3.7 WS-009: Final immutable acceptance

**Goal:** Execute the complete pre-publication gate for one clean tagged commit and one retained candidate.

Implement:

- final identity correlation;
- criterion completeness and dependency checks;
- freshness validation;
- blocker closure;
- evidence-manifest generation;
- human-attestation validation; and
- the final `READY_FOR_PUBLICATION` decision.

Relevant authority:

- Definition of Done final decision (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)
- Blocking issues (`.dx/dx-domain-v0.1.0-alpha-blocking-issues.md`)

### 3.8 WS-010: Post-publication completion

**Goal:** Verify the public release and emit the final `DONE` decision.

Implement:

- public package discovery;
- absence of standalone Analyzer and Generator packages;
- public package identity checks;
- public-source consumer restoration;
- public package-backed Analyzer activation;
- combined-package duplicate boundary checks;
- public metadata checks;
- hosted documentation checks;
- GitHub Release tag, commit, and asset checks; and
- remediation or replacement-version records for failures.

Relevant authority:

- Definition of Done post-publication checks (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)

### Release-gate final carrier
**Goal:** Make one verified DX v2.0 `.dx.txt` carrier the unconditional final product of every `run.py` execution.

Implement outcome normalization, selected evidence export, external temporary staging, packing and verification with repository-owned `scripts/release-gate/dx.py`, authoritative `release-gate/handoff.json`, one stdout `DX_RELEASE_GATE_RESULT=` envelope, operational-error handoffs, and carrier-failure exit-code precedence.

Change only:

```text
scripts/release-gate/release_gate/feedback.py
scripts/release-gate/run.py
scripts/release-gate/release_gate/orchestrator.py
scripts/release-gate/tests/test_feedback.py
scripts/release-gate/tests/test_feedback_contract.py
scripts/release-gate/tests/test_feedback_relevance.py
```

Do not change:

```text
scripts/release-gate/dx.py
scripts/release-gate/contracts/consumer-matrix.json
scripts/release-gate/release_gate/consumers.py
scripts/release-gate/release_gate/diagnostics.py
```

Acceptance requires real carrier generation and inspection for passing, failing, incomplete, and operational-error executions; external staging; `$DX` delivery; DX verification; transported `handoff.json`; actionable normalized findings with decisive evidence; sufficient success evidence; one stdout result envelope; preserved gate-specific exit codes; exit code `3` for carrier failure; and no external collector, `dxs`, ignore bypass, or second packer.

## 4. Features deferred until after the alpha release

Defer these unless a completed workstream demonstrates a concrete need:

- generalized plugin architecture;
- generic release-policy language;
- generic GitHub Actions interpretation;
- cross-repository support;
- database-backed evidence;
- dashboards;
- historical trend analysis;
- distributed execution;
- broad cross-run caching; and
- AI-based release decisions.

## 5. Delivery rule

Each workstream shall deliver both:

1. the production mechanisms required by the release contract; and
2. focused deterministic verification proving the positive path and the material negative paths.

A workstream is not complete when scripts merely exist. It is complete when the scripts have executed against the applicable repository state, retained evidence exists, and every owned criterion has a justified result.

## References

- [Release-gating specification](release-gating-specification.md)
- ACCEPT READY criteria (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`)
- Definition of Done (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)
- Blocking issues (`.dx/dx-domain-v0.1.0-alpha-blocking-issues.md`)
- [Alpha release process](release-process.md)
- [CI/CD pipeline](ci/ci-cd-pipeline.md)
- [CI/CD quick reference](ci/ci-cd-quick-reference.md)