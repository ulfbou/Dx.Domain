# Alpha Conformance Phase 2: Workflow and Publication Reconciliation

**Related issue:** #66
**Evidence:** `.dx/phase-2-conformance-evidence.txt`
**Evidence SHA-256:** `250614ac12e7de29ef48f32f7f5b8953e9bfa71ef470afd2d6f462e99209aa5b`

## Evidence findings

- `master` is the repository default branch and the branch required by `Release CD`.
- `release/0.1.0-alpha` is the current alpha integration base and a target of both PR validation workflows.
- the DocFX workflow incorrectly targeted `main`, duplicated artifact upload and deployment steps, and described triggers and published content that did not match its implementation;
- both DocFX configurations used `main` for contribution links;
- the internal pipeline documentation described nonexistent workflows, branches, scans, previews, drift protection, and deployment behavior;
- `docfx.public.json` already limited its content root to `docs/public`.

## Implemented corrections

- contribution links now target `master`;
- the full-build metadata now reports `0.1.0-alpha`;
- DocFX publication now runs manually or from relevant changes on `master`;
- the publication workflow builds the canonical public configuration with warnings as errors;
- duplicate upload and deployment steps are removed;
- public publication is explicitly limited to `docs/public`;
- maintainer release and CI documentation now describes the executable workflows and branch roles;
- unimplemented release-job boundaries are not represented as completed deployments.

## Verification required

Do not mark P0.6, P0.7, or P0.8 as `met` until all of the following pass on the applied repository state:

```bash
git diff --check
python3 -c 'import json; json.load(open("docfx.public.json")); json.load(open("docfx.full.json"))'
dotnet tool restore
dotnet docfx docfx.public.json --warningsAsErrors
bash scripts/docs-lint.sh
git grep -n -I -E '\bmain\b|pre-merge-ci|integration-ci|drift-protection' -- .github docs docfx.public.json docfx.full.json
```

Any remaining match must be classified as an external URL, historical record, or unresolved contradiction before final review.
## Verification result

Phase 2 verification completed successfully on `docs/alpha-workflow-conformance`.

- DocFX configuration JSON parsed successfully.
- Public DocFX content is limited to `docs/public`.
- Full maintainer DocFX content is limited to `docs/public` and `docs/internal`.
- DocFX contribution links target `master`.
- The full documentation configuration reports `0.1.0-alpha`.
- The publication workflow contains one artifact upload.
- The publication workflow contains one GitHub Pages deployment.
- Release build succeeded.
- Analyzer tests succeeded: 56 passed, 0 failed, 0 skipped.
- Documentation lint succeeded for 41 public pages.
- Structural validation succeeded for 5 C# blocks.
- The material Quickstart compiled successfully for `net8.0`.
- Public DocFX completed with 0 warnings and 0 errors.
- Remaining `main` matches were classified as schema vocabulary, external URLs, initial audit evidence, or explicit statements that `main` is not active.
- The DXA065 descriptor and generated metadata were corrected to use the active `master` branch.

On this evidence, P0.6, P0.7, and P0.8 are verified as met for the Phase 2 scope.
