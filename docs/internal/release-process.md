# Alpha Release Process

## Preconditions

- The release commit is on the branch accepted by the release workflow and version configuration.
- The working tree is clean.
- `version.json`, package metadata, changelog, and release notes agree.
- GitHub Security Advisories is available for private reports.

## Validate

```bash
dotnet tool restore
dotnet restore
dotnet build -c Release
dotnet test -c Release --no-build
scripts/docs-lint.sh
scripts/docs-snippets-compile.sh docs/public net8.0
dotnet docfx docfx.public.json --warningsAsErrors
```

Repeat snippet or sample validation for .NET 9 and .NET 10 when framework-specific APIs are used.

## Inspect artifacts

1. Pack all intended packages into a clean artifacts directory.
2. Verify exact package IDs, version, target frameworks, dependencies, README, license, repository metadata, symbols, and analyzer placement.
3. Install the packages into a clean external sample project.
4. Confirm the quickstart builds and every shipped diagnostic appears with the documented default severity.
5. Verify the public DocFX artifact contains no internal pages.

## Publish

1. Publish to a staging feed.
2. Repeat the clean-project smoke test against staging.
3. Review release notes and known limitations.
4. Publish to the production feed.
5. Create the matching Git tag and GitHub release using the already validated artifacts.
6. Verify package pages, documentation URLs, and checksums.

## Failure and rollback

Stop promotion on any mismatch. NuGet packages are immutable; publish a corrected prerelease version rather than replacing an artifact. Unlist a defective package only when continued discovery creates material risk, and document the decision in the release notes and changelog.
