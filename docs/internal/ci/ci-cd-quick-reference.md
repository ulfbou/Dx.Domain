# CI/CD Quick Reference

## Branches

- Alpha integration PR base: `release/0.1.0-alpha`
- Default and publication branch: `master`
- Generated documentation branch: `gh-pages`

## Validate a documentation change

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

## Check a pull request

```bash
gh pr checks --watch
```

For a failed run:

```bash
gh run view RUN_ID --repo ulfbou/Dx.Domain --log-failed
```

## Build documentation locally

Canonical public site:

```bash
dotnet tool restore
dotnet docfx docfx.public.json --warningsAsErrors
```

Maintainer build including internal documentation:

```bash
dotnet tool restore
dotnet docfx docfx.full.json --warningsAsErrors
```

Only the public configuration is deployed by `.github/workflows/docfx.yml`.

## Release publication

1. Ensure the accepted release commit is on `master`.
2. Run the complete validation sequence.
3. Inspect candidate package artifacts and clean-consumer behavior.
4. Manually dispatch `Release CD` with the intended environment.
5. Treat workflow output as deployment evidence only when the corresponding external operation actually ran.
6. Verify package, release, documentation, and checksum outputs after production publication.

## Failure handling

- Inspect the exact failed step before changing repository content.
- Do not infer the defect from the workflow name alone.
- Preserve failed-run logs as evidence when the correction affects release readiness.
- Re-run validation after the correction and record the successful check URL in the pull request.

## Run and inspect the release gate

```bash
python3 scripts/release-gate/run.py --profile local
python3 scripts/release-gate/dx.py inspect --verify "$DX_FILE"
```

Each run attempts exactly one `$DX/release-gate-<profile>-<decision>-<run-id>.dx.txt` carrier. Read the single `DX_RELEASE_GATE_RESULT=` stdout line for the decision, process exit code, carrier path, byte size, SHA-256, and verification result. Open `release-gate/handoff.json` first.

Exit codes with a verified carrier retain the gate outcome: `0` pass, `1` gate failure, `2` required evidence not proven, `5` external evidence required, and `6` human decision required. Carrier creation or verification failure returns `3`.