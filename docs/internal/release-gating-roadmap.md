# Dx.Domain 0.1.0-alpha Release Roadmap

**Status:** Proposed  
**Release:** `0.1.0-alpha`  
**Owner:** Release readiness, promotion, publication, and completion sequencing  
**Normative design:** [Release-gating specification](release-gating-specification.md)  
**Implementation authority:** [Release-gating implementation plan](release-gating-implementation-plan.md)

## 1. Purpose

This roadmap defines the remaining release-gating path for Dx.Domain `0.1.0-alpha` after completion of WS-001 through WS-005. It preserves the established workstream identities, defines the remaining WS-006 through WS-010 sequence, and establishes the exact meanings of `ACCEPT_READY`, `READY_FOR_PUBLICATION`, and `DONE`.

This roadmap sequences work and evidence. It does not replace the release-gating specification, ACCEPT READY criteria, Definition of Done, release process, documentation authority, or other owning authorities. If this roadmap conflicts with an owning authority, the owning authority governs and this roadmap must be corrected.

## 2. Release-state model

The release progresses through these distinct states:

```text
NOT_ACCEPT_READY
â†’ ACCEPT_READY
â†’ READY_FOR_PUBLICATION
â†’ publication attempted
â†’ DONE
```

The states are not interchangeable:

- `ACCEPT_READY` means the implementation, retained candidate, package-backed consumers, promotion controls, documentation, and required external prerequisites satisfy the complete pre-publication acceptance boundary.
- `READY_FOR_PUBLICATION` means the final immutable acceptance pass has correlated fresh evidence for one authorized commit, tag, manifest, candidate, and release decision.
- `DONE` means publication has occurred and the public packages, public-source consumers, Analyzer behavior, hosted documentation, GitHub Release identity, and completion evidence have been verified.

Publication, partial publication, or artifact existence alone does not imply `DONE`.

## 3. Governing release invariants

Every remaining workstream must preserve these invariants:

1. One authorized repository commit produces one retained candidate.
2. The retained candidate consists of exactly four intended packages.
3. Downstream stages consume the retained candidate and must not rebuild, repack, replace, or post-manifest sign it.
4. Package identity is established by manifest data including package ID, version, filename, byte size, SHA-256, and signing disposition.
5. Mandatory tests, package validation, consumer validation, documentation validation, promotion, publication, and post-publication verification remain correlated to the same release identity.
6. A failed process, missing result, malformed result, unavailable required fact, unexplained skip, stale evidence item, or ambiguous identity fails closed.
7. Human, machine, and summary reporting must not strengthen the evidence actually established.
8. DX carrier structural validity is not authenticity, trusted integrity, workspace agreement, or release provenance.
9. Every criterion, finding, action, package, framework, command, evidence item, and output destination must have an unambiguous identity.
10. Partial publication and partial mutation retain completed, failed, not-attempted, and uncertain outcomes without implying rollback.

## 4. Completed baseline

### 4.1 WS-001 and WS-002

WS-001 and WS-002 are accepted inputs to the reusable release-gating sequence. Their proof remains owned by their existing authorities and evidence. Remaining work must consume their results without duplicating or weakening them.

### 4.2 WS-003: Strict CI and mandatory tests

WS-003 established the shared release-gate foundation for strict command execution, mandatory-project inventory, test-result parsing, zero-test and skipped-test rejection, source-state checks, local and CI invocation, and retained evidence.

Its evidence remains a prerequisite. Any regression in mandatory execution, result parsing, source-state proof, or strict failure behavior blocks every later release state.

### 4.3 WS-004: Authoritative candidate and package validation

WS-004 established authoritative candidate production and independent package validation. It owns the exact four-package candidate, manifest identity, package structure, framework and dependency validation, README validation, Analyzer placement, Analyzer byte identity, mutation tests, and immutable candidate retention.

The WS-004 candidate is the only candidate eligible for WS-005 and all downstream work.

### 4.4 WS-005: Package-backed consumers and Analyzer verification

WS-005 established external package-backed consumer verification without source-project references or repository build inheritance. It covers the required target-framework matrix, representative public behavior, Analyzer activation through each carrier package, combined-package Analyzer loading, and effective-diagnostic boundaries.

WS-005 completion does not authorize publication. It proves that the retained WS-004 candidate can be consumed under the covered matrix.

### 4.5 Cross-cutting release-gate hardening already integrated

The completed baseline also includes repository-owned DX carrier transport, validated actionable feedback, feedback relevance, evidence staging outside the worktree, source-independent filtering, candidate-provider composition, explicit repository DX ignore policy, and selection reporting.

These capabilities remain subject to end-to-end release evidence. Their presence in source does not independently prove a release state.

## 5. Remaining workstream sequence

## 5.1 WS-006: CI/CD identity, promotion, and publication safety

### Goal

Make strict CI an unavoidable precondition and preserve the single retained WS-004 candidate unchanged through every promotion and publication boundary.

### Required implementation

- Verify the authorized tag and complete commit identity.
- Require successful strict CI for that exact commit.
- Close manual dispatch, environment, rerun, and workflow paths that could bypass mandatory checks.
- Promote one named candidate artifact with one authoritative manifest.
- Verify manifest and artifact identity at every downstream boundary.
- Prohibit downstream rebuilding, repacking, package mutation, and post-manifest signing.
- Iterate packages from the verified manifest rather than a duplicated package list.
- Retain a separate publication outcome for every package.
- Detect and classify zero-success, partial-success, and complete publication outcomes.
- Simulate non-production failure after zero, one, two, and three successful package operations.
- Support immutable retry without blind duplicate skipping.
- Suppress GitHub Release creation and documentation deployment after failed or partial publication.
- Preserve exact repository, commit, tag, run, candidate, manifest, and artifact correlation.

### Required proof

- A downstream stage rejects a candidate with a changed filename, byte size, SHA-256, manifest, commit, tag, or run correlation.
- No authorized path can publish without the exact strict-CI prerequisite.
- Every downstream stage proves that it consumed the retained candidate rather than rebuilt output.
- Failure simulations retain per-package completed, failed, not-attempted, and uncertain outcomes.
- Retry proves identity before acting and never treats an unverified duplicate as success.
- GitHub Release and documentation deployment remain suppressed after any failed or partial publication result.

### Acceptance criteria

WS-006 is complete only when:

- the exact strict-CI result is an unavoidable prerequisite;
- no manual or alternate path bypasses identity and manifest verification;
- candidate bytes remain unchanged across every tested boundary;
- partial publication is explicit and cannot become successful publication;
- immutable retry behavior is proved;
- all owned evidence is uniquely identified and retained.

### Stop conditions

Stop on rebuild, repack, post-manifest mutation, ambiguous artifact selection, first-match manifest discovery, blind duplicate skipping, incomplete package outcomes, or any bypass around strict CI.

## 5.2 WS-007: Documentation reconciliation and acceptance

### Goal

Make public, internal, package, and generated documentation describe the actual locked four-package `0.1.0-alpha` candidate and prove executable examples against candidate packages.

### Required implementation

- Describe exactly the four intended published packages.
- Provide package-selection guidance that distinguishes package roles and direct dependencies.
- Use exact `0.1.0-alpha` installation commands.
- State target-framework support by package role and no more broadly than verified evidence.
- Describe Analyzer distribution through the four carrier packages.
- Remove standalone Analyzer installation guidance.
- Provide package-specific README content.
- State direct package dependencies accurately.
- Include meaningful positive and negative examples.
- Compile applicable examples against the retained candidate packages.
- Validate links, routes, DocFX inputs, and the public navigation boundary.
- Complete structured human semantic review.
- Reconcile any statement that calls `0.1.0-alpha` already shipped with the actual lifecycle state governed by this roadmap.

### Required proof

- Documentation inventory proves that no active page describes a fifth published package or standalone Analyzer installation.
- Installation commands use the exact release version and correct package IDs.
- Package and target-framework claims agree with the candidate manifest and consumer evidence.
- Package READMEs agree with public documentation.
- Applicable samples restore and compile against candidate packages without repository project references.
- Link, route, DocFX, and navigation checks pass.
- Human review confirms that examples, limitations, package selection, Analyzer behavior, and release state are not misleading.

### Acceptance criteria

WS-007 is complete only when:

- public and package documentation agree with the retained candidate;
- active documentation contains no obsolete package or Analyzer guidance;
- executable examples pass against candidate packages;
- release-state wording is internally consistent;
- all mandatory documentation checks and semantic review pass.

### Stop conditions

Stop on source-project-backed examples, unverified target-framework claims, conflicting package counts, stale standalone Analyzer guidance, broken public routes, or contradictory shipped-versus-prepublication status.

## 5.3 WS-008: External prerequisites

### Goal

Collect, validate, and retain the external facts and authorized decisions required before publication without exposing credentials or secrets.

### Required implementation

- Verify control of each package ID and availability of the intended version.
- Retain evidence that the publishing identity has required permissions.
- Retain GitHub ruleset, required-check, environment, and production-approval evidence.
- Record release-owner attestations.
- Record applicable security and legal dispositions.
- Record and validate the accepted signing policy and candidate signing disposition.
- Bind every external item to the release, candidate, evidence run, subject, issuer, capture time, and freshness rule required by its authority.

### Required proof

- Missing, stale, mismatched, unauthorized, or unverifiable external evidence blocks acceptance.
- Evidence identifies the relevant package, repository, environment, identity, permission, or decision without disclosing secrets.
- Required checks and approvals refer to the exact protected branch, workflow, environment, or release identity.
- Signing evidence agrees with the candidate manifest and proves that no post-manifest signing mutation is expected.

### Acceptance criteria

WS-008 is complete only when every mandatory external prerequisite has a valid retained disposition, all freshness and identity relationships pass, and no unresolved external blocker remains.

### Stop conditions

Stop on screenshots or prose without attributable identity where structured evidence is required, secret disclosure, unverifiable package control, stale approvals, missing production authority, or signing-policy conflict.

## 5.4 WS-009: Final immutable acceptance

### Goal

Evaluate one clean authorized tagged commit and its one retained candidate, then issue the final pre-publication decision.

### Required implementation

- Correlate repository, full commit, authorized tag, branch or detached-HEAD state, strict-CI run, candidate manifest, package bytes, consumer results, documentation evidence, and external prerequisites.
- Validate criterion completeness and criterion dependencies.
- Validate evidence freshness.
- Prove closure of every blocking issue.
- Generate an immutable evidence manifest.
- Validate required human attestations.
- Reject duplicate criterion, finding, action, or evidence identities.
- Reject stale, mixed-run, ambiguous, or first-match-selected evidence.
- Produce the final `READY_FOR_PUBLICATION` decision only when every mandatory pre-publication criterion passes.

### ACCEPT READY gate

A candidate is `ACCEPT_READY` only when all of the following are true for the same correlated release identity:

1. WS-001 and WS-002 remain valid accepted inputs.
2. WS-003 mandatory strict-CI evidence passes for the exact commit.
3. WS-004 proves exactly four retained package artifacts and their manifest identities.
4. WS-005 proves package-backed consumption and required Analyzer behavior for the retained candidate.
5. WS-006 proves immutable candidate promotion and publication-safety behavior.
6. WS-007 proves reconciled documentation and candidate-backed examples.
7. WS-008 proves all mandatory external prerequisites and authorized dispositions.
8. Every mandatory criterion has a unique result and retained evidence.
9. No mandatory test is skipped, missing, malformed, or silently bypassed.
10. Every allowed skip or unsupported condition is explicit and supports no release claim.
11. Source-state, run, commit, candidate, package, framework, and environment identities are complete and mutually consistent.
12. No unresolved blocker, partial publication, stronger-than-proved claim, or ambiguous evidence relation exists.

`ACCEPT_READY` means the complete acceptance boundary is satisfied. It is necessary but not by itself sufficient for publication.

### READY FOR PUBLICATION gate

The candidate is `READY_FOR_PUBLICATION` only when:

- it is already `ACCEPT_READY`;
- all acceptance evidence remains fresh;
- the final authorized tag and full commit are verified;
- the evidence manifest is complete and immutable;
- required human attestations are valid;
- all blockers remain closed;
- no repository, candidate, workflow, documentation, dependency, or external-prerequisite change has invalidated prior evidence.

### Acceptance criteria

WS-009 is complete only when one of these explicit outcomes is retained:

- `READY_FOR_PUBLICATION`, with complete correlated evidence; or
- a non-ready result identifying every blocker and the evidence that prevented readiness.

An absence of detected errors, a successful script exit, a parseable carrier, or existing package files is not sufficient.

### Stop conditions

Stop on stale evidence, mixed runs, incomplete prerequisites, duplicate evidence identity, missing criterion dependencies, unresolved blockers, inconsistent artifact disposition, or a decision that cannot be reconstructed from retained evidence.

## 5.5 WS-010: Post-publication completion

### Goal

Verify the actual public release and issue the final `DONE` decision.

### Required implementation

- Discover all intended public packages from the public source.
- Prove the absence of prohibited standalone Analyzer and Generator packages for the release.
- Verify public package ID, version, filename where applicable, byte identity where available, and metadata.
- Restore and execute representative consumers using public package sources rather than candidate or repository-local sources.
- Verify public package-backed Analyzer activation through each carrier package.
- Verify combined-package effective-diagnostic and duplicate boundaries.
- Verify hosted documentation, public installation guidance, and public routes.
- Verify GitHub Release tag, full commit, release metadata, and assets against the accepted release identity.
- Retain remediation or authorized replacement-version records for every public discrepancy or partial-publication outcome.

### Required proof

- All four intended packages are publicly discoverable with expected identities.
- Prohibited standalone Analyzer and Generator packages are absent.
- Public-source consumers restore, build, and run under the claimed matrix.
- Analyzer behavior matches the accepted public contract.
- Hosted documentation describes the published release accurately.
- GitHub Release identity and assets correspond to the authorized tag, commit, and accepted candidate.
- Any failure remains non-Done and has an explicit remediation or replacement-version disposition.

### Definition of Done

Dx.Domain `0.1.0-alpha` is `DONE` only when all of the following are true:

1. WS-009 produced `READY_FOR_PUBLICATION` for the exact candidate that was published.
2. Publication used the retained immutable candidate without rebuild, repack, substitution, or post-manifest mutation.
3. All four intended packages are publicly available with the expected identities and metadata.
4. No prohibited standalone Analyzer or Generator package was published for this release.
5. Public-source consumer restore, build, execution, and Analyzer activation pass for every claimed framework and package role.
6. Combined-package Analyzer behavior satisfies the accepted effective-diagnostic boundary.
7. Hosted documentation, package READMEs, installation commands, package guidance, and public routes match the public artifacts.
8. GitHub Release tag, full commit, metadata, and assets agree with the accepted release identity.
9. Every publication operation has a retained outcome and no partial or uncertain publication state remains unresolved.
10. The final evidence index is complete, uniquely identified, immutable, and sufficient to reconstruct `ACCEPT_READY`, `READY_FOR_PUBLICATION`, publication, and `DONE`.
11. Support, compatibility, signing, portability, and integrity claims do not exceed the retained evidence.
12. WS-010 emits the final evidence-backed `DONE` decision.

### Stop conditions

Do not emit `DONE` when a public package is missing, unexpected, mismatched, or unverifiable; a prohibited package exists; public consumers fail; Analyzer behavior differs; hosted documentation is stale; GitHub Release identity is inconsistent; publication is partial or uncertain; or remediation remains open.

## 6. Mandatory evidence integrity

Every release decision must use an evidence index that directly identifies:

- repository identity;
- full commit and authorized tag;
- complete declared source state and admitted exceptions;
- run identity and configuration;
- strict-CI prerequisite run;
- candidate manifest;
- package IDs, versions, filenames, byte sizes, SHA-256 values, and signing dispositions;
- mandatory test results with project and framework correlation;
- consumer and Analyzer results;
- documentation validation and human review;
- CI/CD identity and failure-simulation evidence;
- external prerequisites and freshness;
- blocker closure;
- publication outcomes per package;
- hosted documentation and GitHub Release verification;
- final lifecycle decision and artifact disposition.

Evidence selection must use explicit identities and paths. Directory order, filename coincidence, glob order, or the first matching manifest must never determine authoritative evidence.

## 7. Release-reporting rules

Release reporting must preserve these distinctions:

- carrier structure valid;
- package bytes matched an expected trusted hash;
- source state unchanged within the declared boundary;
- candidate accepted;
- publication authorized;
- publication completed;
- public release verified.

A narrower fact must never be reported as a broader one. In particular:

- DX structural verification is not content integrity;
- calculated hashes without trusted expected values are not identity proof;
- artifact existence is not package validation;
- package validation is not consumer validation;
- consumer validation is not publication authorization;
- publication success is not post-publication completion;
- `ACCEPT_READY` is not `READY_FOR_PUBLICATION`;
- `READY_FOR_PUBLICATION` is not `DONE`.

## 8. Deferred until after the alpha release

The following remain deferred unless a completed workstream proves a concrete requirement:

- generalized plugin architecture;
- generic release-policy language;
- generic GitHub Actions interpretation;
- cross-repository support;
- database-backed evidence;
- dashboards;
- historical trend analysis;
- distributed execution;
- broad cross-run caching;
- AI-based release decisions.

Deferred capabilities do not block the alpha when they are outside the accepted release boundary and no release claim depends on them.

## 9. Delivery rule

Each remaining workstream must deliver:

1. the production mechanism required by the release contract;
2. focused deterministic verification for the positive path and material negative paths;
3. retained evidence from execution against the applicable repository and candidate state;
4. explicit acceptance or blocking results for every owned criterion.

A workstream is not complete because scripts or workflows exist. It is complete only when the production path has executed, retained evidence exists, material negative paths have been proved, and every owned criterion has a justified result.

## 10. References

- [Release-gating specification](release-gating-specification.md)
- [Release-gating implementation plan](release-gating-implementation-plan.md)
- ACCEPT READY criteria: `.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`
- Definition of Done: `.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`
- Blocking issues: `.dx/dx-domain-v0.1.0-alpha-blocking-issues.md`
- [Alpha release process](release-process.md)
- [Documentation authority](documentation-authority.md)
- [CI/CD pipeline](ci/ci-cd-pipeline.md)
- [CI/CD quick reference](ci/ci-cd-quick-reference.md)
