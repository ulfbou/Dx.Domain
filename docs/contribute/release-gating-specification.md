# Dx.Domain Release-Gating Specification

**Scope:** `ACCEPT READY`, `READY FOR PUBLICATION`, and post-publication `DONE` verification

## 1. Purpose

This specification defines the architecture, behavior, interfaces, evidence model, and implementation sequence for Dx.Domain release gating.

The release gate shall:

1. verify mechanically decidable ACCEPT READY (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`) and Definition of Done (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`) criteria;
2. reject contradicted criteria with specific motivations and retained evidence;
3. distinguish failure from unavailable, premature, external, or human-owned evidence;
4. reuse the same verification implementation locally and in CI/CD;
5. require minimal effort from local developers;
6. prevent local and CI verification semantics from drifting apart;
7. preserve one immutable release identity through final acceptance and publication;
8. avoid AI as a required part of release verification; and
9. retain human decisions only where judgment, authority, policy, or approval is genuinely required.

The release gate is not a replacement build system or a second release pipeline. It invokes authoritative repository mechanisms, records their behavior, independently validates their outputs, and aggregates the resulting evidence.

## 2. Governing principles

### 2.1 One verification implementation

Local verification and CI/CD verification shall use the same:

- Python modules;
- criterion definitions;
- project inventories;
- command construction;
- timeout implementation;
- test-result parsers;
- package validators;
- acceptance rules;
- evidence schemas; and
- exit-code conventions.

Bash, PowerShell, and GitHub Actions shall not duplicate verification logic.

### 2.2 Local-first usability

The normal local developer experience shall require only one command.

On Bash-capable systems:

```bash
.dx/scripts/release-gate/run.sh
```

On PowerShell:

```powershell
.\.dx\scripts\release-gate\run.ps1
```

The default command shall:

1. discover the repository root;
2. discover a supported Python executable;
3. detect the operating system and architecture;
4. detect local .NET SDK availability;
5. select the default local verification profile;
6. execute the appropriate checks;
7. write complete evidence automatically;
8. print a concise result with actionable failures; and
9. return a meaningful process exit code.

The developer shall not normally need to provide the repository path, package output paths, evidence directory, run ID, commit SHA, test-project paths, target frameworks, build order, package identities, package version, timeout values, CI-specific arguments, or output-format options.

### 2.3 Fail closed without false rejection

The gate shall fail closed when a mandatory criterion is violated or required executable evidence fails to run.

The gate shall preserve these distinctions:

- A missing future-phase fact is not automatically a failure.
- An absent final tag during implementation readiness is not a failure.
- An absent required final tag during `READY FOR PUBLICATION` is a failure.
- A post-publication criterion before publication is not yet applicable.
- Missing external evidence is not proof that the external condition is false.
- Missing test results after a supposedly successful mandatory test invocation is an error.
- Zero discovered mandatory tests is a failure.
- A human-owned approval shall not be synthesized by automation.

### 2.4 Mechanisms before orchestration

Implementation shall proceed in this order:

1. trustworthy strict build and test execution;
2. authoritative candidate production;
3. independent package verification;
4. package-backed consumer verification;
5. Analyzer activation and duplicate-execution verification;
6. CI-to-CD identity and promotion controls;
7. publication failure handling;
8. documentation reconciliation;
9. external prerequisites;
10. final evidence aggregation; and
11. post-publication completion.

No release workflow shall be considered correct merely because its workflow file contains the desired job names.

### 2.5 No required AI dependency

All mandatory mechanical verification shall run without AI.

AI may assist maintainers with interpreting unexpected failures, reviewing semantic documentation, drafting remediation, and exploring evidence. AI output shall not be required to build, test, package, inspect archives, calculate hashes, run consumers, count diagnostics, validate manifests, correlate release identities, simulate publication failures, or decide mechanically defined criteria.

## 3. Release phases

### 3.1 Development verification

Development verification provides quick local feedback, validates affected repository areas, and identifies likely release-gating failures early. It does not constitute ACCEPT READY or final release evidence.

### 3.2 ACCEPT READY

`ACCEPT READY` establishes that all implementation, workflow, fixture, collector, and documentation mechanisms required for final acceptance exist and are executable, and that no known implementation gap prevents final Definition of Done (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`) execution.

It does not require final tag creation, final tagged CI execution, production publication, public package propagation, or post-publication completion.

### 3.3 READY FOR PUBLICATION

`READY FOR PUBLICATION` establishes that one immutable release candidate has passed every mandatory pre-publication criterion and that required external prerequisites and approvals are complete.

It requires:

- the authorized tag;
- the approved full commit SHA;
- clean-tree evidence;
- strict CI evidence for that commit;
- an authoritative build-once candidate;
- candidate manifest and package hashes;
- consumer and Analyzer verification;
- runtime contract verification;
- documentation acceptance;
- promotion-control evidence; and
- required approvals.

### 3.4 DONE

`DONE` establishes that publication completed without unresolved partial failure and that all applicable post-publication checks passed.

## 4. Criterion statuses

Every criterion shall use one of these statuses:

- `PASS`: The required fact was positively established using acceptable evidence.
- `FAIL`: Evidence positively contradicts the criterion.
- `NOT_PROVEN`: Required evidence is missing, incomplete, stale, malformed, or not correlated to the relevant run identity.
- `NOT_APPLICABLE`: The criterion does not apply under a documented contract condition.
- `NOT_YET_APPLICABLE`: The criterion belongs to a later phase.
- `EXTERNAL_EVIDENCE_REQUIRED`: The criterion depends on authenticated external state not present in the current run.
- `HUMAN_DECISION_REQUIRED`: Automation collected the supporting facts, but an authorized person must decide or approve the criterion.
- `ACCEPTED_INPUT`: The criterion is covered by accepted retained evidence, including WS-001 or WS-002.
- `ERROR`: The verifier could not evaluate the criterion because its own execution, parser, schema, dependency, or environment failed.

An `ERROR` shall never be reported as a product failure without making the distinction explicit. The aggregator shall never convert `NOT_PROVEN`, `EXTERNAL_EVIDENCE_REQUIRED`, `HUMAN_DECISION_REQUIRED`, or `ERROR` into `PASS`.

## 5. Overall decisions

The gate shall emit one phase-specific primary decision.

### 5.1 Development

- `PASS`
- `FAIL`
- `INCOMPLETE`
- `ERROR`

### 5.2 ACCEPT READY

- `ACCEPT_READY`
- `NOT_ACCEPT_READY`
- `ACCEPT_READY_NOT_PROVEN`
- `ERROR`

### 5.3 Pre-publication

- `READY_FOR_PUBLICATION`
- `NOT_READY_FOR_PUBLICATION`
- `READINESS_NOT_PROVEN`
- `ERROR`

### 5.4 Post-publication

- `DONE`
- `NOT_DONE`
- `COMPLETION_NOT_PROVEN`
- `ERROR`

## 6. Language and tooling architecture

### 6.1 Python verification core

Python shall own all substantive verification behavior, including:

- repository discovery;
- execution planning;
- subprocess management;
- timeout enforcement;
- process-tree termination;
- environment inspection;
- Git-state capture;
- .NET command construction;
- evaluated MSBuild collection;
- TRX parsing;
- package, ZIP, and nuspec inspection;
- SHA-256 calculations;
- manifest generation and verification;
- external consumer generation;
- Analyzer diagnostic validation;
- workflow structure validation;
- evidence creation;
- criterion evaluation;
- result aggregation; and
- report generation.

The core should prefer the Python standard library. A third-party dependency may be introduced only when it clearly improves correctness and cannot be implemented reliably with the standard library at reasonable cost.

### 6.2 Bash launcher

Bash shall be a thin operator launcher. It may locate the repository root and Python executable, forward arguments, preserve the Python exit code, and report setup failures.

It shall not invoke `dotnet build`, `dotnet test`, or `dotnet pack` directly; parse structured evidence; define inventories; or decide criteria. Interactive launcher commands shall not begin with `set -...` behavior.

### 6.3 PowerShell launcher

PowerShell shall provide equivalent native Windows usability. It may locate Python, resolve the repository path, forward arguments, preserve the Python exit code, and report setup failures. It shall not implement independent verification logic.

### 6.4 GitHub Actions

GitHub Actions shall orchestrate checkout, toolchain setup, invocation of the shared Python entry point, evidence upload, job dependencies, environment approvals, and artifact preservation.

Workflow files shall not independently define package allowlists, test inventories, target-framework inventories, package-version rules, test-count policies, package-content checks, or criterion decisions.

## 7. Repository layout

Tracked executable scripts shall live under `.dx/scripts/`. Generated evidence shall live under `.dx/evidence/`.

```text
.dx/
├── scripts/
│   └── release-gate/
│       ├── release_gate/
│       │   ├── __init__.py
│       │   ├── model.py
│       │   ├── configuration.py
│       │   ├── repository.py
│       │   ├── process.py
│       │   ├── evidence.py
│       │   ├── git_state.py
│       │   ├── dotnet.py
│       │   ├── trx.py
│       │   ├── msbuild.py
│       │   ├── packages.py
│       │   ├── manifests.py
│       │   ├── consumers.py
│       │   ├── diagnostics.py
│       │   ├── workflows.py
│       │   ├── external_state.py
│       │   ├── criteria.py
│       │   └── aggregation.py
│       ├── contracts/
│       │   ├── release-contract.json
│       │   ├── mandatory-projects.json
│       │   ├── package-contract.json
│       │   ├── consumer-matrix.json
│       │   └── required-behaviors.json
│       ├── run.py
│       ├── run.sh
│       └── run.ps1
└── verification/
    └── release-gate/
        └── <run-id>/
```

The repository's ignore rules shall track `.dx/scripts/**` while continuing to ignore generated `.dx/evidence/**` evidence.

## 8. Local developer experience

### 8.1 Default profile

The default local profile shall be `local`. It shall run the fastest complete set of checks that:

- requires no credentials;
- publishes nothing;
- does not require the final release tag;
- does not require external approval;
- does not require the developer to locate artifacts manually; and
- gives trustworthy feedback on the current working tree.

Initially, it shall execute WS-003:

1. environment preflight;
2. repository discovery;
3. Git-state capture;
4. required SDK verification;
5. strict restore;
6. strict Release build;
7. mandatory runtime tests;
8. mandatory Analyzer tests;
9. TRX verification;
10. zero-test and skipped-test rejection;
11. tracked-source immutability verification; and
12. concise report generation.

Later fast checks may be added only when they preserve the low-friction local experience.

### 8.2 Repository discovery

The runner shall search upward from the launcher's location and current working directory until it finds the repository root. Recognition shall use an explicit combination of repository markers, such as `.git`, `Dx.Domain.sln`, `Directory.Build.props`, and `version.json`.

### 8.3 Environment discovery

The runner shall automatically inspect the operating system, architecture, Python version, Git version, .NET SDK inventory, repository root, current branch, full HEAD SHA, working-tree status, and CI environment markers.

When a required tool is missing, the command shall stop with the missing capability, the required version where applicable, and an exact remediation route where practical.

### 8.4 Credentials and interactivity

The default local profile shall require no GitHub authentication, NuGet API keys, signing certificates, environment secrets, or publication permissions.

Default commands shall never prompt for confirmation, paths, credentials, overwrite approval, package selection, test selection, or retry approval. Undiscoverable required information shall produce a precise prerequisite failure.

### 8.5 Evidence directory

The runner shall generate a run ID automatically using a format equivalent to:

```text
YYYYMMDDTHHMMSSZ-<short-head-sha>-<random-suffix>
```

Evidence shall be written to:

```text
.dx/evidence/release-gate/<run-id>/
```

The developer shall not need to choose or clean evidence directories.

### 8.6 Console output

Default output shall remain concise while complete stdout and stderr are retained as evidence.

```text
Dx.Domain release gate
Profile: local
Commit: 0123456789abcdef...
Working tree: modified
Evidence: .dx/evidence/release-gate/20260916T070000Z-01234567-a1b2/

PASS  Environment
PASS  Restore
PASS  Strict Release build
FAIL  Runtime tests
      Expected at least 1 discovered test, observed 0.
PASS  Analyzer tests
PASS  Tracked-source immutability

Decision: FAIL
Report: .dx/evidence/release-gate/.../report.md
```

### 8.7 Dependency-aware execution

The runner shall stop when continuation cannot produce valid evidence. Independent checks may continue after another independent check fails when doing so is safe and useful. The execution plan shall express dependencies explicitly instead of relying on a global stop-on-first-error convention.

## 9. Profiles

### 9.1 `local`

The no-argument default. It requires no credentials, performs no publication, uses current working-tree state, and runs release-equivalent mechanical checks.

### 9.2 `ci`

Uses the same criteria as `local`, plus CI metadata capture, immutable commit expectations, CI annotations, and evidence upload. It may be stricter about tree cleanliness and commit identity but shall never be weaker.

### 9.3 `accept-ready`

Runs all implemented ACCEPT READY verification. It remains noninteractive and secret-free unless an authenticated external subprofile is explicitly selected.

### 9.4 `candidate`

Produces or validates the authoritative candidate. It requires a clean tree, complete SDK inventory, the exact release contract, and authoritative build ordering. It discovers its output location automatically.

### 9.5 `prepublish`

Runs final pre-publication acceptance and requires the final tag, identity correlation, authoritative candidate, external prerequisites, and required attestations.

### 9.6 `postpublish`

Runs public-source completion verification. It is never part of the no-argument local profile.

## 10. Command interface

The explicit Python invocation shall be:

```bash
python3 .dx/scripts/release-gate/run.py
```

This is equivalent to:

```bash
python3 .dx/scripts/release-gate/run.py --profile local
```

Advanced options may include:

- `--profile`;
- `--repo`;
- `--evidence-dir`;
- `--run-id`;
- `--candidate-dir`;
- `--retain-workspaces`;
- `--console-verbosity`;
- `--command-timeout`;
- `--inactivity-timeout`; and
- `--no-color`.

Options shall not relax mandatory release criteria. No authoritative profile may support `--skip-tests`, `--ignore-warnings`, `--allow-zero-tests`, `--skip-package-validation`, or `--ignore-hash-mismatch`.

## 11. Machine-readable contracts

### 11.1 Release contract

The release contract shall define the version, tag, authorized package IDs, exact filenames, signing disposition, target frameworks, Analyzer asset path, build order, and permitted release phases.

For `0.1.0-alpha`, exactly these packages are authorized:

- `Dx.Domain.Annotations`;
- `Dx.Domain.Kernel`;
- `Dx.Domain.Primitives`; and
- `Dx.Domain.Facts`.

Standalone publication of `Dx.Domain.Analyzers` and `Dx.Domain.Generators` is prohibited.

### 11.2 Mandatory-project contract

The mandatory-project contract shall identify every required project, path, project type, target frameworks, minimum discovered-test count, skipped-test policy, build prerequisites, and expected result format.

The initial mandatory test projects are:

- `tests/Dx.Domain.Tests/Dx.Domain.Tests.csproj`; and
- `tests/Dx.Domain.Analyzers.Tests/Dx.Domain.Analyzers.Tests.csproj`.

Discovery may report unexpected test projects but shall not silently alter the mandatory inventory.

### 11.3 Consumer matrix

The consumer contract shall define carrier package, target framework, restore/build/run actions, behavior fixture, expected output, expected Analyzer behavior, and combined-package cases.

### 11.4 Required-behavior inventory

Material public behaviors shall receive stable identifiers. Maintainers approve the inventory once; repeated execution is automated thereafter.

## 12. Shared process execution

Every external command shall use one Python process runner.

### 12.1 Required observations

The runner shall record:

- executable;
- arguments;
- working directory;
- selected non-secret environment;
- start and finish timestamps;
- duration;
- exit code;
- timeout state;
- termination actions; and
- stdout and stderr evidence paths.

### 12.2 Timeouts and termination

Every command shall have a hard timeout. Commands with regular progress may additionally have an inactivity timeout. Defaults shall be centralized by command class.

On timeout, the runner shall terminate the full process tree, first gracefully and then forcibly after a bounded grace period. Timeout and termination failures shall have distinct classifications.

### 12.3 Output and classifications

Console output shall remain bounded. Complete output shall be retained.

Execution classifications shall include:

- `SUCCESS`;
- `NONZERO_EXIT`;
- `TIMEOUT`;
- `INACTIVITY_TIMEOUT`;
- `EXECUTABLE_NOT_FOUND`;
- `START_FAILURE`;
- `TERMINATION_FAILURE`; and
- `OUTPUT_CAPTURE_FAILURE`.

## 13. Evidence model

### 13.1 Run manifest

Every run shall create `run.json` before substantive execution. It shall record the schema version, run ID, profile, phase, timestamp, repository root, full HEAD SHA, branch or detached-head state, tag when applicable, working-tree state, platform identity, tool versions, verifier identity, and contract hashes.

### 13.2 Evidence layout

```text
<run-id>/
├── run.json
├── environment.json
├── git-before.json
├── git-after.json
├── commands/
├── build/
├── tests/
├── packages/
├── consumers/
├── diagnostics/
├── external/
├── criteria.json
├── report.json
└── report.md
```

### 13.3 Integrity and provenance

Evidence shall be written atomically where practical. Every criterion result shall identify its criterion ID, status, motivation, verifier, inputs, evidence references, observed values, and expected values.

Evidence shall never record tokens, passwords, NuGet API keys, private certificate material, or secret values. It may record secret names, presence, actor identity, object IDs, permission status, approval state, and collection timestamps.

## 14. WS-003 strict CI requirements

### 14.1 Preconditions

The runner shall verify repository discovery, supported Python, Git, required .NET SDKs, a valid mandatory-project contract, resolvable contract paths, and a writable evidence directory.

A local run may use a modified tree but must report it. Authoritative CI and candidate production require a clean tracked tree.

### 14.2 Cleanup

The gate shall clean only reviewed generated locations. It shall not perform broad destructive Git cleanup. The cleanup contract shall identify locations such as `bin`, `obj`, `TestResults`, `artifacts`, and `_site` and record affected directories before deletion.

### 14.3 Restore and build

Restore failure shall block dependent build and test operations. The shared .NET command builder shall enforce:

```text
Configuration=Release
TreatWarningsAsErrors=true
ContinuousIntegrationBuild=true
```

The gate shall reject explicit warning relaxation, warning-producing successful builds, missing required project builds, unsupported SDK fallback, and unexpected source modification.

### 14.4 Mandatory tests

Every mandatory project shall run explicitly and produce TRX in a run-specific directory. Each result shall record project, target framework, command, exit code, discovered, executed, passed, failed, and skipped counts, duration, and TRX identity.

A mandatory project passes only when:

- its command exits successfully;
- the configured minimum test count is discovered;
- no tests fail;
- no mandatory tests are skipped;
- a valid current-run TRX exists;
- TRX counts are consistent; and
- tracked source remains unchanged.

Console success text is insufficient.

### 14.5 Synthetic success prohibition

No unconditional property or message may establish that quality gates passed. Any summary status must be derived from actual command and evidence results.

### 14.6 CI reuse

GitHub Actions shall invoke the same implementation:

```bash
python3 .dx/scripts/release-gate/run.py --profile ci
```

The workflow shall not retain a competing restore, build, or test definition for the same gate.

## 15. Subsequent workstreams

### 15.1 WS-004 candidate production and verification

WS-004 shall implement clean-tree candidate production, exact build order, four-package output, manifest generation, Analyzer output identity, immutable artifact retention, exact package validation, nuspec and framework validation, README validation, Analyzer placement and byte identity, and mutation tests.

### 15.2 WS-005 package-backed consumers and Analyzer verification

WS-005 shall create isolated consumers automatically, with no source-project references or inherited repository build settings. It shall run the required framework matrix, representative runtime behavior, Analyzer activation through every carrier package, combined-package activation, load-failure detection, and effective-diagnostic uniqueness checks.

### 15.3 WS-006 CI/CD and publication safety

WS-006 shall enforce exact tag and commit identity, strict CI dependency, immutable artifact use, manifest verification at every boundary, prohibition of rebuilding or repacking, exact manifest-based publication, per-package outcomes, fail-safe downstream behavior, partial-publication simulation, and immutable retry.

### 15.4 WS-007 documentation

WS-007 shall combine mechanical documentation checks, compilation of examples against candidate packages, package README inspection, DocFX verification, and a constrained human semantic-review record. Documentation shall match the four-package contract and shall not instruct consumers to install a prohibited standalone Analyzer package.

### 15.5 WS-008 external prerequisites

WS-008 shall collect GitHub controls, required checks, environment configuration, package-ID availability and control, publishing identity, and required approvals. It is excluded from the default local profile.

### 15.6 WS-009 final acceptance

WS-009 shall correlate all evidence to one immutable release identity and emit `READY_FOR_PUBLICATION`, `NOT_READY_FOR_PUBLICATION`, `READINESS_NOT_PROVEN`, or `ERROR`.

### 15.7 WS-010 post-publication

WS-010 shall verify public package discovery, exact public inventory, public-source restoration, package-backed Analyzer activation, public metadata, hosted documentation, GitHub Release identity, asset identity, and remediation of completion failures.

## 16. Local and CI equivalence

For any criterion executed in both environments:

```text
same inputs + same repository state + same toolchain = same criterion decision
```

CI may additionally require a clean tree, immutable SHA, expected event, expected ref, evidence upload, and annotations. Local verification may permit and report modifications and classify the run as non-authoritative.

Local verification shall not silently relax warnings-as-errors, test discovery, failure counts, skipped-test policy, package identity, or hash verification.

Evidence paths shall use repository-relative forward-slash notation. Any platform-specific exception must be explicit, contract-based, evidence-backed, and non-silent.

## 17. Performance and reuse

The default local command shall avoid duplicate work:

- restore shall not run twice without a documented reason;
- projects shall not be rebuilt before `--no-build` tests;
- package archives shall be inventoried once per run;
- hashes shall be reused safely within the current run;
- TRX shall be parsed once; and
- Git state shall be captured only at defined boundaries.

Cross-run reuse shall initially be conservative or absent. Evidence shall not be reused when the commit, tracked tree, relevant contract, verifier identity, material SDK identity, candidate manifest, or package bytes differ.

Every failure shall state what failed, the affected criterion, expected and observed values, the producing command or input, the evidence path, and the next corrective action.

## 18. Safety and non-destructive behavior

The gate shall not edit product source, repair workflows automatically, move tags, publish from the default local profile, access local secrets by default, rebuild after candidate validation, modify packages, sign after manifest finalization, suppress warnings, retry with weaker options, interpret missing evidence as success, delete unrelated files, or run broad destructive Git cleanup.

Temporary fixtures shall be isolated and removable. Failed fixtures may be retained under the evidence directory.

## 19. Human decisions

Automation shall not own semantic documentation adequacy, material-claim completeness, release-owner approval, security disposition, legal or licensing approval, signing-policy acceptance, acceptance of alpha limitations, or replacement-version authorization after an immutable partial publication.

Human attestations shall be structured and tied to criterion ID, authorized actor, role, timestamp, full release commit, candidate-manifest SHA-256, reviewed evidence, decision, and motivation. They shall not be required by the default local profile.

## 20. Exit codes

The runner shall use stable exit codes:

- `0`: selected profile passed;
- `1`: one or more criteria failed;
- `2`: required evidence was not proven;
- `3`: verifier or environment error;
- `4`: invalid arguments or configuration;
- `5`: external evidence required; and
- `6`: human decision required.

For mixed outcomes, precedence shall be verifier/configuration error, criterion failure, not proven, external evidence required, human decision required, then success. The JSON report remains authoritative.

## 21. Verification of the gate

The gate shall have automated tests for:

- command success and nonzero exit;
- hard and inactivity timeouts;
- child-process termination;
- missing executables;
- output capture and secret filtering;
- atomic evidence writes;
- malformed contracts and TRX;
- missing TRX;
- zero discovered tests;
- skipped-test rejection;
- inconsistent test counts;
- tracked-source mutation;
- unsupported SDK inventory;
- path normalization; and
- equivalent local and CI decisions.

Later workstreams shall add package and publication mutation tests.

## 22. Initial WS-003 deliverable

The initial implementation shall provide:

```text
.dx/scripts/release-gate/release_gate/__init__.py
.dx/scripts/release-gate/release_gate/model.py
.dx/scripts/release-gate/release_gate/configuration.py
.dx/scripts/release-gate/release_gate/repository.py
.dx/scripts/release-gate/release_gate/process.py
.dx/scripts/release-gate/release_gate/evidence.py
.dx/scripts/release-gate/release_gate/git_state.py
.dx/scripts/release-gate/release_gate/dotnet.py
.dx/scripts/release-gate/release_gate/trx.py
.dx/scripts/release-gate/release_gate/criteria.py
.dx/scripts/release-gate/contracts/release-contract.json
.dx/scripts/release-gate/contracts/mandatory-projects.json
.dx/scripts/release-gate/run.py
.dx/scripts/release-gate/run.sh
.dx/scripts/release-gate/run.ps1
```

It shall also include unit tests, the required `.gitignore` exception, corrected strict CI invocations, removal of synthetic quality success, shared GitHub Actions invocation, evidence upload, and local usage documentation.

## 23. Initial acceptance criteria

WS-003 is accepted only when:

- the default local verification requires one command and no arguments;
- no credentials or manual path setup are required;
- evidence is created automatically under `.dx/evidence/`;
- Bash, PowerShell, and GitHub Actions invoke the same Python entry point;
- wrappers contain no criterion logic;
- inventories and strict build properties are defined once;
- warnings are errors;
- mandatory projects execute explicitly;
- zero discovered tests fail;
- skipped mandatory tests fail;
- missing or malformed TRX fails;
- test counts and full command evidence are recorded;
- timeouts terminate process trees;
- tracked-source mutations are detected;
- every criterion references evidence;
- verifier errors remain distinct from product failures;
- evidence uploads even when CI fails; and
- local and CI criterion semantics are identical.

## 24. Deferred features

The following are deferred until justified by completed workstreams:

- generalized plugins;
- a generic policy language;
- a generic GitHub Actions interpreter;
- database-backed evidence;
- dashboards;
- remote execution services;
- distributed queues;
- cross-repository support;
- historical trend analysis;
- broad incremental caching; and
- AI-based release or documentation decisions.

## 25. Architectural decision

Dx.Domain release verification shall use a Python verification core shared identically between local execution and GitHub Actions, with thin Bash and PowerShell launchers and machine-readable contracts.

The no-argument local command shall require no credentials and no manual artifact or evidence management. Implementation shall prioritize trustworthy strict execution, minimal local effort, exact local/CI reuse, noninteractive operation, bounded execution, immutable evidence, actionable rejection reasons, no duplicated acceptance logic, no required AI dependency, and incremental delivery aligned with the actual release blockers.

## 26. Final carrier and LLM handoff contract

### 26.1 Required final product

Every completed `run.py` execution shall attempt to produce exactly one final, verified DX v2.0 `.dx.txt` carrier containing all relevant information required for the next actor, especially an LLM, to continue without access to the originating filesystem.

This requirement applies when the selected profile passes, fails, is incomplete, requires external evidence, requires a human decision, or encounters an operational error.

The carrier is the final product. It is not:

- an optional attachment;
- failure-only feedback;
- a wrapper around a path to local evidence;
- a reduced summary that requires the originating repository; or
- secondary to `feedback.json` or another local report.

Persistent evidence may remain under `.dx/evidence/release-gate/<run-id>/` as execution evidence and a retention source. It is not the consumer interface.

The gate decision and carrier-production result are separate facts. A gate decision may be passing or non-passing while the carrier result is created or failed. The process shall not report successful completion unless the required carrier was created and verified.

### 26.2 Unconditional carrier generation

The default invocation generates the carrier. No authoritative profile may complete without attempting carrier generation.

If command-line compatibility is retained:

- `--feedback dx` may remain accepted but is redundant;
- `--feedback summary` shall be rejected for authoritative profiles; and
- `--feedback none` shall be rejected for authoritative profiles.

Carrier generation shall also be attempted after an operational error whenever the repository-owned `scripts/release-gate/dx.py` remains executable.

### 26.3 Exactly one final deliverable

The directory identified by `$DX` receives exactly one new final deliverable per execution:

```text
release-gate-<profile>-<decision>-<run-id>.dx.txt
```

No separate JSON or Markdown handoff file is required outside the carrier.

### 26.4 Transported handoff tree

The carrier transports a purpose-built `release-gate/` tree containing only material relevant to the actual run:

```text
release-gate/
├── handoff.json
├── summary.md
├── report.json
├── criteria.json
├── run.json
├── source-state/
│   ├── git-before.json
│   └── git-after.json
├── candidate/
│   └── candidate-manifest.json
├── contracts/
├── findings/
│   ├── index.json
│   └── <finding-id>/
│       ├── finding.json
│       ├── stdout.txt
│       ├── stderr.txt
│       └── diagnostics.log
└── stages/
    ├── strict-ci.json
    ├── candidate.json
    └── consumers.json
```

Only applicable files are included. Missing optional sections are omitted and shall not be represented by placeholders.

### 26.5 Authoritative entry point

`release-gate/handoff.json` is the authoritative machine-readable entry point. It shall tell the next actor:

- what was executed;
- which repository, branch, commit, and run were examined;
- which profile ran;
- the final decision and process exit code;
- whether execution completed normally;
- whether tracked source remained unchanged;
- what passed, failed, remained incomplete, or was blocked;
- which findings are primary and which are consequential;
- where decisive evidence exists inside the carrier;
- which exact next actions are permitted;
- which artifacts must be preserved;
- whether rerunning is appropriate and which profile applies; and
- whether the carrier payload and carrier structure were verified.

The handoff shall contain fields equivalent to:

```json
{
  "schema": "dx-domain.release-gate-handoff/1.0",
  "carrier": {
    "format": "DX v2.0",
    "filename": "release-gate-accept-ready-not-accept-ready-<run-id>.dx.txt",
    "payload_manifest_sha256": "<sha256>",
    "verification": "VERIFIED"
  },
  "execution": {
    "run_id": "<run-id>",
    "profile": "accept-ready",
    "operation_status": "COMPLETED",
    "decision": "NOT_ACCEPT_READY",
    "exit_code": 1
  },
  "repository": {
    "commit": "<full-sha>",
    "branch": "<branch>",
    "source_unchanged": true
  },
  "summary": {
    "objective_achieved": false,
    "primary_failure_count": 0,
    "actionable_finding_count": 0,
    "blocked_operation_count": 0
  },
  "findings": [],
  "blocked_work": [],
  "next_actions": [],
  "artifacts": {},
  "evidence_index": [],
  "continuation": {
    "recommended_action": "CORRECT_AND_RERUN",
    "rerun_profile": "accept-ready",
    "preserve_candidate": false
  }
}
```

The carrier cannot embed the SHA-256 of the complete carrier containing that value. The whole-carrier SHA-256 belongs in the console result envelope. `handoff.json` may instead contain the SHA-256 of a canonical payload manifest.

### 26.6 Relevant and complete evidence

The carrier includes every fact and evidence file required to:

1. understand the decision;
2. identify the cause;
3. distinguish primary from consequential failures;
4. formulate a correction; and
5. execute the next scoped step without the originating filesystem.

Include, when applicable:

- complete normalized findings;
- exact expected and observed values;
- exact failed command;
- command working directory;
- exit classification and exit code;
- relevant stdout and stderr;
- relevant diagnostic logs;
- applicable contract fragments or complete small contract files;
- candidate manifest and package identities;
- source-immutability evidence;
- stage decisions and dependencies;
- verifier or transport errors; and
- explicit next actions.

Do not include:

- NuGet caches;
- `bin/` or `obj/` trees;
- duplicated successful command logs;
- complete generated consumer workspaces unless their source is required to reproduce a failure;
- recursively duplicated stage reports;
- evidence from unrelated runs; or
- arbitrary files selected only because they exist beneath the evidence root.

A passing run still includes sufficient run identity, repository identity, criteria and decisions, applicable package identities, source immutability, stage summaries, hashes or manifests, verification statements, and the next authorized operation to establish continuity.

### 26.7 Substantive failure projection

For every non-passing child stage, the final handoff shall:

1. create a top-level stage finding;
2. project every primary failed child criterion into the final findings;
3. include consequential findings only when they explain blocked work;
4. deduplicate repeated evidence;
5. preserve exact expected and observed values; and
6. include the decisive evidence files.

Every actionable finding shall contain:

- `finding_id`;
- `stage`;
- `criterion_id`;
- `kind` as `PRIMARY` or `CONSEQUENTIAL`;
- `status`;
- `expected`;
- `observed`;
- `impact`;
- a next-action code and instruction; and
- evidence paths inside the carrier.

Child stage reports shall be normalized instead of recursively embedded.

### 26.8 External staging and repository-owned DX tooling

Persistent execution evidence remains under `.dx/evidence/`. The final handoff tree shall be assembled in a purpose-built temporary export directory outside the Git worktree.

The runner shall:

1. select relevant evidence;
2. assemble the export tree outside the worktree;
3. pack it with repository-owned `scripts/release-gate/dx.py`;
4. verify it with the same implementation;
5. write the verified carrier to `$DX`; and
6. delete the temporary export directory.

The runner shall not:

- pack `.dx/evidence/` directly;
- bypass ignore semantics;
- modify `scripts/release-gate/dx.py` to special-case release-gate evidence;
- track generated verification evidence; or
- require a second packaging utility.

### 26.9 Console contract

Human-readable progress and the concise summary go to stderr. Stdout contains exactly one machine-readable result line:

```text
DX_RELEASE_GATE_RESULT={"schema":"dx.release-gate.result-envelope/1.0","profile":"accept-ready","decision":"NOT_ACCEPT_READY","process_exit_code":1,"carrier":"<absolute-carrier-path>","carrier_sha256":"<sha256>","carrier_size":123456,"carrier_verification":"PASS"}
```

This line is a locator and integrity envelope. It is not the handoff content.

Stdout shall not contain:

- the complete feedback dossier; or
- `DX_RELEASE_GATE_FEEDBACK=`.

### 26.10 Exit-code precedence

The process exit code represents the gate outcome unless final-carrier production fails:

- `0`: the selected profile passed and the carrier was created and verified;
- `1`: the gate mechanically established failure and the carrier was created and verified;
- `2`: required evidence was not proven and the carrier was created and verified;
- `3`: operational error, including inability to create or verify the required carrier;
- `5`: external evidence is required and the carrier was created and verified; and
- `6`: a human decision is required and the carrier was created and verified.

Carrier creation or verification failure takes precedence over every gate outcome and returns `3`.

An operational-error handoff shall contain the exception, traceback, available repository identity, completed observations, and exact resumption action.

### 26.11 Scoped correction

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

Consumer failures remain a separate correction after the handoff mechanism reports them reliably.

### 26.12 Mandatory acceptance criteria

The correction is accepted only when automated tests prove that:

1. every completed `run.py` invocation attempts one final `.dx.txt` carrier;
2. passing runs produce the carrier;
3. failing runs produce the carrier;
4. incomplete runs produce the carrier;
5. operational-error runs produce the carrier whenever repository-owned `dx.py` remains executable;
6. the packing source is outside the Git worktree;
7. the carrier destination is `$DX`;
8. the carrier uses DX v2.0;
9. the carrier passes `dx.py inspect --verify`;
10. the carrier contains `handoff.json`;
11. `handoff.json` identifies every actionable finding;
12. every actionable finding contains expected, observed, impact, next action, and evidence;
13. decisive evidence is physically transported;
14. successful criteria are summarized sufficiently to establish continuity;
15. child stage reports are normalized rather than recursively embedded;
16. repository and candidate identity prevent cross-run confusion;
17. stdout contains exactly one `DX_RELEASE_GATE_RESULT=` line;
18. stdout does not contain the complete handoff dossier;
19. stdout does not contain `DX_RELEASE_GATE_FEEDBACK=`;
20. the result envelope contains carrier path, byte size, SHA-256, and verification status;
21. a failed gate with a valid carrier retains the gate-specific nonzero exit code;
22. carrier creation or verification failure returns operational exit code `3`;
23. no external collector or `dxs` dependency is used;
24. no Git-ignore bypass is required; and
25. unit tests inspect the real generated carrier inventory and transported content.

### 26.13 Required execution architecture

```text
run.py
-> execute gate
-> normalize success, failure, incomplete, or error evidence
-> create one self-contained handoff tree
-> pack with repository-owned dx.py
-> verify with repository-owned dx.py
-> emit one compact result envelope
-> exit according to the gate outcome unless carrier production failed
```

The final result is one verified DX v2.0 `.dx.txt` carrier containing the complete, relevant, directly actionable LLM handoff.

## References

- ACCEPT READY criteria (`.dx/dx-domain-v0.1.0-alpha-accept-ready-criteria.md`)
- Definition of Done (`.dx/dx-domain-v0.1.0-alpha-definition-of-done.md`)
- Blocking issues (`.dx/dx-domain-v0.1.0-alpha-blocking-issues.md`)
- [Alpha release process](release-process.md)
- [CI/CD pipeline](ci/ci-cd-pipeline.md)
- [CI/CD quick reference](ci/ci-cd-quick-reference.md)
- [Documentation authority](documentation-authority.md)