# DX Remaining Work: WS-005 through Accept Ready
Status: Development planning
Owner: Remaining implementation outcomes and release-readiness evidence
Scope: Work remaining after completed WS-001 through WS-004, including acceptance criteria, release gates, Accept Ready, and Definition of Done

## Purpose

This document is the controlling execution description for the work remaining after WS-001 through WS-004. It translates Outcomes 5 through 12 from `development/implementation-plan.md` into release-oriented work streams with explicit completion evidence.

The accepted specifications remain authoritative. This document sequences implementation and evidence. It does not redefine product behavior, introduce a machine encoding, expose internal plans, or strengthen structural verification into authenticity or integrity.

## Established baseline

WS-001 through WS-004 are complete and are prerequisites rather than remaining work:

- WS-001 established the carrier semantic kernel and architecture harness.
- WS-002 established workspace context and no-follow observation.
- WS-003 established candidate discovery and pure selection.
- WS-004 established operation values and the immutable planning foundation.

Before WS-005 begins, the repository must demonstrate that these completed work streams still pass their governing architecture, semantic, conformance, fixture, and regression checks. A regression in WS-001 through WS-004 blocks every later work stream.

## Evidence vocabulary

The remaining work uses the following evidence distinctions:

- **Proved** means executable evidence demonstrates the claimed property against an Accepted authority.
- **Verified** means the specified verification operation completed within its stated boundary.
- **Structurally verified** means only that the carrier satisfies the Accepted structural carrier contract. It does not establish authenticity, trusted integrity, workspace agreement, workspace applicability, or safe application.
- **Inspected** means facts were projected from supplied content without strengthening their meaning.
- **Compared** means validated carrier content was related to an explicit workspace under the comparison contract.
- **Accept Ready** means all mandatory implementation, conformance, security, process, packaging, compatibility, and evidence gates in this document have passed for one identified candidate.

No report, diagnostic, machine result, release summary, or carrier metadata may use a stronger term than the evidence supports.

## Cross-cutting implementation rules

Every remaining work stream must:

1. Consume Accepted authorities and identify stable governing headings or named rules in tests.
2. Preserve semantic independence from CLI parsing, presentation, filesystem implementation, Git invocation, environment reads, and mutation capability where the architecture prohibits those dependencies.
3. Add real implementation, real tests, and real fixtures together. Placeholders and implementation-captured expected behavior are prohibited.
4. Keep compatibility characterization separate from current conformance.
5. Produce deterministic semantic results for identical explicit inputs and resolved environmental facts.
6. Fail closed on malformed requests, malformed carriers, unsupported capability, unavailable evidence, unsafe paths, changed preconditions, and incomplete delivery.
7. Retain enough evidence to explain the exact result without rerunning semantic rules.
8. Prevent human, machine, and numeric process presentations from strengthening, weakening, or contradicting the authoritative semantic result.
9. Account explicitly for every skip and unsupported capability. A skip does not prove support.
10. Keep release evidence correlated to one repository state, one candidate, one run identity, and the declared environment.

## WS-005: Inspection and structural verification

### Objective

Implement the read-only inspection projections and the bounded structural-verification operation over the carrier semantic kernel.

### Required implementation

- Acquire carrier bytes through an explicit read boundary.
- Implement summary, entry-list, read-only-list, decoded-entry-hash, and exact-entry-byte inspection projections.
- Implement structural verification over the Accepted DX v2.0.0 contract.
- Preserve invalid-carrier, unsupported-version, acquisition-failure, and missing-entry distinctions.
- Retain carrier locations and specific structural findings where available.
- Compose inspection and verification without workspace, discovery, Git, presentation, process, carrier-output, or workspace-write capability.

### Required proof

Tests must cover valid, malformed, unsupported-version, empty, multi-entry, read-only, text, base64, arbitrary-byte, duplicate-path, invalid-path, malformed-attribute, malformed-framing, malformed-payload, missing-entry, and carrier-acquisition-failure cases.

Exact-entry inspection must prove byte equality using lengths and SHA-256 where preservation is claimed. Side-effect tests must prove that inspection and verification create or modify no product artifact.

Tests and presentation checks must prove that structural success is represented only as structure-valid. Calculated hashes must remain inspection facts unless a trusted expected value and trust source are identified independently.

### Acceptance criteria

WS-005 is accepted only when:

- all projections return authoritative typed results;
- exact-entry output preserves decoded bytes exactly;
- malformed and unsupported inputs remain distinct;
- read-only composition has no write capability;
- structural success cannot produce authenticity, integrity, applicability, safe-application, or workspace-agreement claims;
- mandatory tests pass with no unexplained skip.

### Stop conditions

Stop and correct the work stream if parsing accepts unexplained directive text, verification wording is stronger than the carrier specification, presentation reevaluates parsing, or any read operation can acquire write capability.

## WS-006: Workspace comparison

### Objective

Implement deterministic, non-mutating comparison between validated carrier content and an explicit workspace context.

### Required implementation

- Map carrier logical paths through the workspace boundary without changing carrier identity.
- Observe final and ancestor components without following links.
- Classify identical, missing, differing, directory, symlink, unsupported object, observation failure, containment conflict, and path-identity collision.
- Preserve read-only status while reporting the actual byte or existence relation.
- Support workspace-only facts only when explicitly requested and within an explicit scope.
- Reject a missing, non-directory, unreadable, or unsupported comparison root with the correct non-success result.
- Use one semantic comparison path for human and machine presentation.

### Required proof

Conformance cases must cover every comparison relation, final and ancestor links, broken links, containment, case and Unicode collision behavior for each claimed environment, unsupported objects without content access, permission failures, workspace-only scope, and deterministic ordering.

The same semantic cases must produce equivalent outcomes in every presentation mode. A nonexistent comparison root must never become a zero-difference success.

### Acceptance criteria

WS-006 is accepted only when:

- comparison performs no mutation and creates no application intent;
- links are not followed;
- missing and invalid roots fail closed;
- extra workspace paths never imply deletion intent;
- human and machine outputs derive from one retained comparison result;
- environment support claims are limited to environments with passing evidence.

### Stop conditions

Stop on symlink following, implicit deletion meaning, hidden workspace discovery, presentation-specific comparison logic, or a success result produced without completing required observations.

## WS-007: Carrier-creation planning and output execution

### Objective

Implement the complete workspace-to-carrier path from Accepted selection results through frozen payloads, preview, deterministic serialization, and complete delivery to an explicit output sink.

### Required implementation

- Load exact bytes only for paths already selected by WS-003 semantics.
- Preserve post-selection loading failures without reclassifying paths as excluded.
- Freeze payload bytes and accepted attributes before execution.
- Produce an immutable carrier-creation plan that records selected paths, payload readiness, findings, sink facts, preconditions, and executability.
- Render preview from that exact plan.
- Support separate non-filesystem and filesystem sink adapters.
- Revalidate volatile sink preconditions without rediscovery, reselection, reloading, or semantic replanning.
- Deliver complete carrier bytes or report incomplete, failed, or uncertain delivery.
- For filesystem sinks, enforce explicit replacement authority, parent authority, no-follow safety, supported target type, and self-reference rules.

### Required proof

Tests must cover readable text, empty and arbitrary-byte files; unreadable, missing, changed, linked, directory and unsupported selected sources; empty selection; payload freezing; deterministic serialization; sink success; broken or incomplete delivery; uncertain delivery; absent and existing filesystem outputs; replacement; directory, link and special-object conflicts; missing and changed parents; output self-reference; output inside a selected subtree without participation; and changed sink preconditions.

Agreement tests must prove preview and execution consume the same plan. Mutation tests must prove that the carrier-output executor has no workspace-write capability and cannot reload source content.

### Acceptance criteria

WS-007 is accepted only when:

- output destination does not affect selected membership;
- complete equivalent inputs produce byte-identical normative carrier output in every claimed environment;
- incomplete delivery can never be reported as successful creation;
- source payload changes after freezing cannot silently alter output;
- filesystem conflicts fail before destructive mutation;
- non-filesystem sinks receive no fabricated filesystem semantics;
- emitted carriers pass the canonical structural verifier.

### Stop conditions

Stop on hidden output exclusion, automatic binary omission, implicit replacement, partial delivery reported as success, source reread during execution, or sharing of workspace-write capability.

## WS-008: Application planning and workspace mutation

### Objective

Implement safe carrier-to-workspace planning, preview, full preflight, deterministic execution, changed-precondition handling, and retained partial-failure evidence.

### Required implementation

- Build immutable application plans from validated carriers, explicit workspace context, no-follow observations, read-only declarations, existing-target authority, and parent-creation authority.
- Distinguish create, replace, satisfied, explicit skip, read-only satisfied, read-only discrepancy, conflict, unsupported, and observation failure.
- Perform complete preflight before the first mutation.
- Execute effects in deterministic order.
- Revalidate volatile preconditions before mutation and before each affected effect.
- Stop after a failed effect or invalidated precondition.
- Retain completed, failed, known-unchanged, not-attempted, and uncertain effects, including created parents.
- Make no rollback or whole-workspace transaction claim.

### Required proof

Tests must cover absent, identical and differing targets; absent policy and explicit fail, skip and overwrite; equal, missing and differing read-only targets; directory, link, unsupported object, containment, collision and permission outcomes; parent creation and incompatible parent appearance; complete-preflight failure with no writes; target and parent changes after planning; first-effect and later-effect failure; interruption; deterministic ordering; and no-change success.

Security tests must prove no access outside the workspace, no source or destination link following, no unsupported-object reads, and no silent collision merging.

### Acceptance criteria

WS-008 is accepted only when:

- planning performs no mutation;
- execution accepts validated planned intent rather than raw requests;
- complete-preflight failure leaves the workspace unchanged;
- explicit skip remains visible and cannot become complete success;
- read-only discrepancies remain blocking according to operation semantics;
- changed preconditions never cause silent replanning;
- partial failure is reconstructable from retained evidence;
- the workspace executor has no carrier-output capability.

### Stop conditions

Stop on raw-request execution, rediscovery, reselection, link following, implicit overwrite or skip, continuation after failure, missing uncertainty, or rollback implication.

## WS-009: Diagnostic interpretation and presentation agreement

### Objective

Implement presentation-neutral diagnostic interpretation and human rendering from authoritative results and retained findings.

### Required implementation

- Implement the Accepted diagnostic taxonomy and stable symbolic identifiers.
- Preserve one primary semantic result and its supporting findings.
- Use typed resource references for logical carrier paths, workspace coordinates, physical paths, filesystem sinks, non-filesystem sinks, ignore sources, and Git capabilities.
- Preserve blocking status, differences, explicit skips, read-only discrepancies, effect states, uncertainty, and partial-failure relationships.
- Keep suggestions separate from facts.
- Bound public cause chains and exclude implementation exception names from stable identity.
- Produce deterministic ordering where ordering carries meaning.

### Required proof

Tests must cover every mandatory diagnostic family in `spec/diagnostics.md`, including selection outcomes, creation failures, structural-verification limits, comparison differences, application outcomes, partial failure, ordering, stability, and multiple findings for one result.

Agreement tests must prove that human and machine-semantic projections preserve the same primary result, blockers, resources, effects, uncertainty, and completion meaning while allowing presentational differences.

### Acceptance criteria

WS-009 is accepted only when:

- diagnostics never rerun semantic rules;
- no exception class becomes a stable product identifier;
- no presentation makes a stronger or weaker claim than the retained result;
- partial-failure diagnostics reconstruct known post-operation state without implying rollback;
- quiet and verbose presentation cannot alter semantics.

### Stop conditions

Stop if diagnostic generation requires semantic callbacks, if wording changes meaning, if resource types are conflated, or if a concrete machine field shape is introduced without schema admission.

## WS-010: CLI and process adaptation

### Objective

Expose the canonical `dx` command contract, validated requests, streams, terminal safety, process-result mapping, and a machine-mode boundary that does not pre-empt the machine encoding decision.

### Required implementation

- Implement canonical commands `pack`, `inspect`, `verify`, `compare`, and `apply`.
- Validate complete request grammar before invoking semantics.
- Resolve relative operands once against the initial working directory.
- Implement carrier input from a file and `-` from standard input.
- Route carrier bytes, exact entry bytes, textual primary data, diagnostics, help, and version to their specified streams.
- Implement stdout and filesystem carrier sinks.
- Enforce terminal refusal for carrier bytes and exact entry bytes unless explicitly authorized.
- Implement help, version, no-argument help, quiet, verbose, dry-run, and machine-mode eligibility.
- Implement the complete numeric result mapping 0 and 2 through 11.
- Preserve incomplete delivery and broken pipe as environmental output-delivery failures.

### Machine-mode decision gate

Before releasing machine mode, choose and Accept a concrete serialization encoding and one interoperable public structure. Admit the corresponding schema under `schema/README.md`, or explicitly remove machine mode from the release claim until that work is complete. A release must not advertise machine mode while leaving its encoding and structure unspecified.

If machine mode is admitted, implement exactly one complete representation on stdout, schema validation, prose-semantic validation, and human/machine/process agreement. Internal plans and implementation identities must remain absent.

### Required proof

Process tests must cover every command, operand and option rule; duplicate scalar and contradictory request rejection; stdin ownership; byte-oriented input and output; stream purity; output sink mapping; broken pipes; terminal refusal and override; help and version isolation; every inspection projection; dry-run non-mutation; quiet and verbose invariance; every numeric process result; locale and undeclared-environment independence; current-directory changes after resolution; and caught interruption.

### Acceptance criteria

WS-010 is accepted only when:

- invalid requests are rejected before semantic execution;
- process code owns no semantic decision;
- byte outputs are uncontaminated;
- machine mode is either fully admitted and validated or excluded from the release claim;
- human, machine where admitted, and numeric outcomes agree;
- no undocumented public numeric value is returned;
- output-delivery failure can replace underlying success and cannot be hidden.

### Stop conditions

Stop on semantic logic in request parsing, implicit authority from omitted options, human text mixed with primary bytes, terminal byte delivery without authority, unspecified machine serialization shipped as a contract, or contradictory result and process value.

## WS-011: Compatibility transitions

### Objective

Implement only the migrations and replacements accepted by `spec/compatibility.md`, without allowing historical behavior to bypass current safety, planning, or process semantics.

### Required implementation

- Provide a temporary `dx.py` compatibility entry that invokes the canonical `dx` contract and emits the required deprecation finding.
- Map deprecated `unpack` to canonical `apply` request semantics without bypassing planning or workspace safety.
- Accept deprecated positional output only when `-o` or `--output` is absent, map it to the filesystem sink, and emit a deprecation finding.
- Accept deprecated `--json` only as an alias for `--machine` on machine-eligible invocations, emit a deprecation finding, and retain no historical JSON shape.
- Retain the standard-input `-` convention.
- Demonstrate the intentional absence of replaced aliases, defaults, output numbering, duplicated-command tolerance, historical process values, exception-class codes, implicit ignore behavior, implicit exclusions, hidden output exclusion, binary skip/fail semantics, default target skip, unconditional read-only skip, comparison letters, integrity overclaim, unsafe symlink behavior, and evidence-poor partial writes.

### Required proof

Each migration requires an end-to-end transition suite proving canonical semantic mapping, required deprecation meaning, process agreement, and inability to bypass safety. Each intentional replacement requires a test proving the Accepted current behavior and absence of accidental fallback. Historical characterization remains separately labeled and cannot establish conformance.

### Acceptance criteria

WS-011 is accepted only when:

- every admitted historical behavior has exactly one compatibility classification;
- every migration uses the canonical implementation path;
- every deprecation is represented consistently in human and admitted machine output;
- no migration preserves an unspecified historical machine shape;
- DX v1.3.1 input remains unclaimed unless separately accepted.

### Stop conditions

Stop on duplicate semantic implementation for a compatibility spelling, safety bypass, retention without authority, or accidental acceptance of unspecified historical behavior.

## WS-012: Packaging and canonical executable exposure

### Objective

Produce installable, reproducible DX artifacts exposing the canonical `dx` executable and verify the installed product rather than only the source checkout.

### Required implementation

- Select and document packaging metadata and package layout consistent with current evidence.
- Expose canonical `dx` invocation.
- Include the temporary `dx.py` transition only as required by compatibility authority.
- Generate artifacts from a controlled source state.
- Record artifact filename, byte size, SHA-256, package inventory, version, runtime dependencies, build environment, and source identity.
- Test clean installation into an isolated environment.
- Run representative and mandatory conformance, process, compatibility, and security checks against the installed artifact.
- Verify version agreement across package metadata, executable output, and release evidence.
- Review runtime dependencies under the standard-library-first rule.

### Required proof

Artifact builds must be repeatable under the declared environment. Evidence must distinguish reproducible within that environment from unverified cross-platform reproducibility. Installation tests must prove no undeclared source-tree dependency and no reliance on a dirty workspace.

Artifact inventory must prove that required code, metadata, schemas when admitted, notices, and documentation are present and that development-only or secret material is absent.

### Acceptance criteria

WS-012 is accepted only when:

- artifacts are generated successfully from the declared controlled source state;
- size and SHA-256 are recorded and independently recomputed;
- clean installation succeeds;
- canonical `dx` help and version work from the installed artifact;
- mandatory installed-artifact tests pass;
- package inventory and dependency review pass;
- no support, portability, integrity, or compatibility claim exceeds retained evidence.

### Stop conditions

Stop on unverifiable artifacts, version disagreement, install dependence on repository-local files, dirty-source dependence, unresolved consequential package-layout decision, missing mandatory schema for an advertised machine contract, or unsupported environment claims.

## Cross-work-stream release hardening

The following corrections are mandatory before Accept Ready even if their originating code predates the redesigned implementation:

### Bounded verification language

Every API, CLI, diagnostic, machine result, test name, release report, and workflow field must distinguish structural verification from trusted integrity. A structure-only result must never be promoted to `VERIFIED`, integrity-valid, authentic, safe, or workspace-equal.

### Complete parser consumption

Every carrier directive must be consumed completely. Unknown attributes, duplicate attributes, malformed quoting, invalid values, and unexplained trailing text must produce invalid-carrier results. No parser path may ignore unmatched directive text.

### Consistent compare preconditions

Human and machine comparison modes must share implementation and results. Missing or invalid roots, unreadable roots, failed observations, and unsupported environments must fail closed.

### Complete source-state correlation

Any gate claiming source immutability must compare the declared complete repository state, including commit identity, staged changes, unstaged tracked changes, untracked files, and any explicitly admitted exception. A narrower check must use a narrower claim.

### Unique evidence identity

Every run, stage, criterion, subject, package, framework, command, finding, action, and evidence destination must have an unambiguous identity. Duplicate identities or destinations are release failures. Evidence is immutable within a run and must not be overwritten by another finding.

### Authoritative Analyzer execution evidence

If Analyzer loading or multiplicity is part of the release, it must be proved from authoritative structured build evidence. Repeated diagnostic prose across stdout, stderr, text logs, or framework passes is not proof of repeated Analyzer execution.

### Unified test-result semantics

One repository-owned TRX interpretation must be used throughout release gating. Missing or malformed results are operational failures, not zero tests. Multi-target execution must retain framework identity and prove every required target.

### Run and candidate correlation

All retained evidence must share one run identity and identify repository, commit, candidate manifest, artifact hashes, configuration, and declared environment. The gate must consume explicit evidence paths and must not select the first matching manifest found in a directory tree.

### Fail-closed operational behavior

Fetch, diff, process-start, result-parse, output-delivery, and evidence-write failures must remain failures. They must not become no-change, no-difference, zero-test, or successful verification results.

## Accept Ready gate

Accept Ready is a candidate-level decision. It is not inferred from document maturity, source compilation, test count, carrier parseability, artifact existence, or the absence of reported errors alone.

A candidate is Accept Ready only when all conditions below are satisfied for the same correlated run.

### Authority and scope gate

- All behavior claimed by the candidate is owned by Accepted product, architecture, specification, compatibility, security, contribution, schema, workflow, or release authority.
- No Draft document or historical implementation behavior is used as release authority.
- Every unresolved issue affecting claimed conformance is either resolved or explicitly outside the release boundary.
- Documentation and implementation make the same support and compatibility claims.

### Implementation gate

- WS-001 through WS-012 are complete for the claimed release boundary.
- Architecture dependency and capability-isolation checks pass.
- Read and planning paths have no mutation capability.
- Carrier-output and workspace-write capabilities remain separate.
- Executors consume validated operation-specific plans.
- No internal plan is exposed as a public contract.

### Conformance gate

- All mandatory semantic and operation conformance suites pass.
- Exact-byte carrier preservation and deterministic serialization pass in each claimed environment.
- Structural-verification limits are tested and preserved in presentation.
- Comparison, creation, application, diagnostics, CLI/process, and compatibility transitions pass end to end.
- Every mandatory requirement has executable evidence or an explicitly bounded non-claim.

### Security gate

- No path escapes the explicit workspace root.
- No source, final-target, ancestor, or output symlink is followed where prohibited.
- Unsupported objects are classified without content access.
- Complete preflight and changed-precondition behavior pass.
- No hidden overwrite, skip, parent creation, ignore activation, Git activation, output exclusion, or fallback authority exists.
- Temporary resources, permissions, diagnostics, and debug evidence disclose no secret or irrelevant host state.

### Test-evidence gate

- The canonical mandatory test command completes successfully.
- No mandatory test is skipped.
- Every allowed skip records a reason and supports no release claim.
- Test results are parseable, framework-correlated, run-correlated, and internally consistent.
- Regression, conformance, compatibility, architecture, security, environment, process, and installed-artifact tests retain distinct evidence classes.

### Machine-contract gate

If machine mode is included in the release claim:

- serialization encoding and interoperable object structure are Accepted;
- an active concrete schema exists under schema governance;
- schema syntax and meta-schema validation pass;
- positive and negative structural fixtures pass;
- prose-semantic validation passes;
- human, machine, and numeric outcomes agree;
- internal plans and implementation details are absent.

If these conditions are not met, machine mode must be excluded from the release claim and public release surface.

### Packaging gate

- The candidate manifest identifies exactly the intended artifacts.
- Every artifact filename, version, size, SHA-256, and inventory is verified.
- Clean installation passes in the declared environment.
- Canonical `dx` invocation and version operate from the installed artifact.
- Required licenses, notices, metadata, documentation, and schemas are present.
- No unexpected artifact, stale output, source-tree dependency, secret, cache, or development-only file is included.

### Compatibility gate

- Every accepted migration passes its transition suite.
- Every intentional replacement proves absence of historical fallback.
- Deprecation diagnostics have the required meaning.
- No unspecified DX v1.3.1 or historical machine-shape compatibility is claimed.

### Environment and portability gate

- The release names the exact platform, Python runtime, filesystem, terminal context, and optional capability context supported by evidence.
- Linux, Python 3.12, and overlayfs claims are backed by passing environment evidence.
- Other environments remain unverified unless equivalent evidence is retained.
- Portable design is not presented as verified support.

### Evidence-integrity gate

- Evidence belongs to one run and one candidate.
- Criterion and finding identities are unique.
- Evidence destinations are unique and immutable.
- Every decision can be reconstructed from retained authoritative results.
- Missing evidence, ambiguous identity, duplicate identity, stale evidence, or mixed-run evidence blocks acceptance.
- The final Accept Ready decision maps to a consistent promotion disposition and cannot simultaneously prohibit the promotion it authorizes.

### Accept Ready result

The gate may produce `ACCEPT_READY` only when every mandatory gate above passes. Any failure, error, unexplained skip, unsupported claimed capability, ambiguous evidence relation, or stronger-than-proved claim produces a non-ready decision.

`ACCEPT_READY` means the identified candidate is eligible for the next explicitly governed promotion or publication step. It does not itself claim publication, deployment, broad platform support, authenticity, signing, formal attestation, or production operations excluded by the product boundary.

## Definition of Done

The initial DX prototype is Done only when all statements below are true for one accepted candidate.

### Product and authority

- The shipped product remains within `VISION.md` and its explicit non-goals.
- Current behavior conforms to all Accepted specifications within the release boundary.
- Architecture and Accepted ADRs are realized without unresolved authority conflict.
- Compatibility classifications are implemented exactly where admitted.

### Complete implementation

- WS-001 through WS-012 have accepted implementation and evidence.
- Canonical operations `pack`, `inspect`, `verify`, `compare`, and `apply` are executable through `dx`.
- Planning, preview, execution, diagnostics, process adaptation, compatibility transitions, and packaging are integrated through the accepted boundaries.
- No mandatory product path depends on historical `dx.py` organization.

### Correctness and safety

- Carrier bytes and decoded entry bytes are preserved according to the carrier contract.
- Selection is deterministic and consumes explicit facts.
- Workspace mapping is contained and no-follow.
- Read operations and dry runs do not mutate product resources.
- Mutation consumes validated plans, revalidates preconditions, and retains partial-failure evidence.
- Success cannot conceal conflict, unsupported state, observation failure, discrepancy, explicit unsatisfied skip, changed precondition, incomplete delivery, or partial mutation.

### Public contract

- CLI grammar, streams, terminal safety, process values, help, version, and environment resolution conform to the Accepted process specification.
- Diagnostic identifiers and meanings are stable and implementation-independent.
- Human, machine where admitted, and numeric outcomes agree.
- Machine mode is either fully specified, schema-validated, and shipped, or explicitly absent from the release claim.

### Verification and evidence

- The complete mandatory suite passes against source and installed artifact.
- No mandatory check is skipped or silently bypassed.
- Every claimed environment and compatibility boundary has retained evidence.
- Exact artifact and carrier identities are recorded with byte sizes and SHA-256 values.
- Release evidence is uniquely identified, immutable for the run, and sufficient to reconstruct the decision.
- Structural verification is never represented as trusted integrity.

### Packaging and release readiness

- The final artifacts are reproducibly generated in the declared environment.
- Clean installation and installed-artifact conformance pass.
- Artifact inventory, dependency review, version agreement, and documentation synchronization pass.
- The final candidate is `ACCEPT_READY` under this document's gate.
- Release notes state exact support, compatibility, limitation, migration, and non-goal boundaries without stronger claims.

### Closure condition

Done is a property of the accepted candidate and its retained evidence. It is not established by merging code, completing documents, passing only unit tests, producing a parseable carrier, or creating package files.

Any later change to source, dependency, build environment, artifact bytes, Accepted behavior, compatibility classification, or release evidence creates a new candidate and requires the affected gates to run again.

## Required final evidence index

The accepted candidate must retain an index that directly identifies:

- repository identity and commit;
- complete source-state capture and admitted exceptions;
- run identity and configuration;
- governing authority versions or repository paths;
- mandatory command outcomes;
- architecture, semantic, conformance, regression, compatibility, security, environment, process, and installed-artifact results;
- test-result files and framework correlation;
- candidate manifest;
- artifact inventory, sizes, and SHA-256 values;
- package installation evidence;
- machine schema and validation evidence when machine mode is claimed;
- supported environment evidence;
- known limitations and bounded non-claims;
- final decision and promotion disposition.

The index must use explicit paths and identities. Directory search order, filename coincidence, or the first matching file must not determine authoritative evidence.

## Sequencing and carrier boundaries

The preferred remaining sequence is WS-005 through WS-012 in order. A work stream may be split only into smaller, independently executable vertical carriers that preserve its boundary and produce real evidence. A carrier must not combine unrelated work streams merely to reduce carrier count.

Each delivered implementation carrier must contain a non-empty permanent change, include only new and changed files, preserve complete file contents, exclude modified read-only files, use DX v2.0.0, use a `.dx.txt` filename, and pass the authoritative carrier verification available in the repository before delivery.

The final Accept Ready pass is not an implementation carrier. It is an evidence-backed decision over the complete correlated candidate after all remaining work streams are accepted.
