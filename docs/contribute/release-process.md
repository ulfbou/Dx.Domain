# Release Process

## Branch roles

- `release/0.1.0-alpha` is the integration base for alpha release-readiness pull requests.
- `master` is the repository default branch and the only branch accepted by the current release publication workflow.
- `gh-pages` contains the generated public documentation site.
- `main` is not an active repository branch and must not be used in repository-local workflow instructions.

Merging into `release/0.1.0-alpha` does not publish packages or deploy documentation. Release publication remains a separate, manually dispatched operation from `master`.

## Pull-request validation

Pull requests targeting `master` or `release/0.1.0-alpha` run:

- `Build & Validate`, including restore, Release build, tests, documentation lint, structural snippet checks, and material documentation-example compilation;
- `CI (Analyzers & Docs Governance)`, including analyzer tests, API-baseline validation, documentation lint, structural snippet checks, and the API-diff gate when present.

Structural snippet checks are not compilation. Material examples must be compiled separately or explicitly classified as conceptual.

## Local validation

```bash
dotnet tool restore
dotnet restore
dotnet build -c Release
dotnet test -c Release --no-build
bash scripts/docs-lint.sh
bash scripts/docs-snippets-compile.sh docs/public net8.0
bash scripts/docs-examples-compile.sh
dotnet docfx docfx.public.json --warningsAsErrors
```

## Documentation publication

A push to `master` that changes `docs/public/**`, `docfx.public.json`, the pinned tool manifest, or the DocFX workflow triggers public documentation publication. The workflow:

1. restores the pinned tools;
2. builds `docfx.public.json` with warnings treated as errors;
3. uploads one `docfx-public-site` artifact;
4. deploys that same `_site` output once to `gh-pages`.

`docfx.public.json` publishes only `docs/public`. `docfx.full.json` is a local maintainer build that includes `docs/internal`; it is not the GitHub Pages publication input.

## Package publication

The `Release CD` workflow is manually dispatched and rejects any branch other than `master`. Its current sequence is:

1. validate the branch and calculate the version with the pinned version tool;
2. restore, test, and pack;
3. optionally sign packages when signing credentials exist;
4. upload package artifacts;
5. run the selected staging, canary, and production jobs;
6. create the GitHub release after successful production deployment.

The staging and canary jobs currently provide workflow boundaries, but their deployment commands are not production package publication. Do not describe them as completed external deployments without execution evidence.

## Artifact inspection

Before production publication:

1. verify package IDs, version, target frameworks, dependencies, README, license, repository metadata, symbols, and analyzer placement;
2. install packages in a clean external project;
3. compile the public Quickstart against the candidate packages;
4. confirm every shipped diagnostic appears with its descriptor-default severity;
5. verify that the public DocFX output contains no internal pages.

## Failure and rollback

Stop promotion on any mismatch. NuGet packages are immutable, so publish a corrected prerelease version rather than replacing an artifact. Unlist a defective package only when continued discovery creates material risk, and record the decision in release notes and the changelog.

## Release-gate handoff
Every release-gate invocation attempts one final verified DX v2.0 carrier in `$DX`. Human-readable output goes to stderr. Stdout contains exactly one `DX_RELEASE_GATE_RESULT=` locator and integrity envelope.

Continue by opening `release-gate/handoff.json` inside the carrier. It identifies the repository and run, decision, source state, normalized findings, blocked work, preservation requirements, decisive evidence, and permitted next actions. The next actor must not require the originating repository to understand and continue the run.

Preserve every artifact named by the handoff. Do not rerun, rebuild, replace, or discard a candidate when the continuation contract prohibits it. Treat an absent or unverified carrier as an operational failure even when the gate decision passed.