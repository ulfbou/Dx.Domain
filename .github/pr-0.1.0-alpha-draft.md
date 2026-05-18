---
title: "Release: v0.1.0-alpha — Authority Substrate Frozen"
branch: release/0.1.0-alpha
labels: [type:release, status:needs-review, prio:P0, area:governance]
---

## Description

Prepare v0.1.0-alpha for publication. This release marks the first public alpha of Dx.Domain, freezing the authority substrate (Kernel, Primitives, Facts, Annotations) and completing the 11-analyzer refactor to use centralized composition.

**Release Notes:** See [docs/public/changelog.md](../docs/public/changelog.md) consolidated entry for [0.1.0-alpha].

## Type of Change

- [x] Configuration change
- [x] Documentation update
- [x] Chore / Release Management

## Related Issues

Closes #[TBD: create Issue #NNN for "Release v0.1.0-alpha"]

## Changes Made

- **docs/public/changelog.md**
  - Consolidated `[0.x] — Pre-release` into single `[0.1.0-alpha] — 2026-05-15` release entry
  - Added release context: "authority substrate frozen"
  - Detailed Added section: AnalyzerServicesFactory, Phase 0 packages, analyzer-enforced invariants with rule IDs (DXA010, DXA020, DXA022)
  - Clarified Changed section: 11 analyzers refactored, Kernel surface frozen, documentation aligned

- **global.json**
  - `sdk.rollForward`: `latestFeature` → `latestPatch` for release reproducibility
  - `sdk.allowPrerelease`: `true` → `false` (stable SDK only)

- **src/Dx.Domain.Analyzers/Dx.Domain.Analyzers.csproj**
  - Added readme.md to package manifest (`<None Include="readme.md" Pack="true" PackagePath="/" />`)

## Testing

Release validation completed on main (prior to branch):

### Test Configuration

- **OS**: Windows
- **.NET Version**: .NET 10.0.100
- **Branch**: release/0.1.0-alpha

### Test Results

- [x] All existing unit tests pass: 53/53 passed, 0 failed, 0 skipped
- [x] Release build: `dotnet build -c Release` succeeded with 0 errors
- [x] Integration tests pass: full build/test/pack pipeline validated
- [x] Manual pack validation: 5 NuGet packages produced

```bash
# Build validation
dotnet clean -c Release
dotnet restore
dotnet build -c Release --no-restore
dotnet test -c Release --no-restore --no-build
dotnet pack -c Release --no-restore --no-build
```

## Documentation

- [x] Updated code comments/XML documentation (AnalyzerServicesFactory fixed CS1570; all public APIs documented)
- [x] Updated README.md (release status reflected)
- [x] Updated conceptual docs in `docs/` (changelog consolidated, release context added)
- [x] Reviewed DocFX preview (docs/public/ aligned with shipped APIs)

## CI/CD Checklist

- [x] Pre-merge CI passed (full release validation: build, test, pack on Release configuration)
- [x] Code follows style guidelines (Kernel surface frozen via DXA040, no new violations)
- [x] No new warnings introduced (Release build: 0 errors, 0 warnings in src/)
- [x] Security scan requirements checked
- [x] No vulnerable dependencies detected
- [x] Commit messages follow conventional commits: `chore(release): prepare v0.1.0-alpha`

## Breaking Changes

No breaking changes introduced in this release. Authority substrate remains stable.

## Additional Context

**Release Readiness Status:**
- ✅ Nullability: src/ clean (CS8601 warnings only in test files, non-blocking)
- ✅ DXA040 Compliance: Kernel public APIs marked [ApprovedKernelApi]
- ✅ GitVersion: Versions explicitly set in Directory.Build.props (0.1.0-alpha)
- ✅ DXA065 / Docs: All public APIs documented with XML, AnalyzerServicesFactory added
- ✅ Constraint Audit: Passed (construction authority, result discipline, UTC-only time)
- ✅ Conventional Commits: Commit messages conform to .commitlintrc.json schema
- ✅ Release CD: Workflow at .github/workflows/release-cd.yml validated

**Merge Strategy:** This PR should be merged after:
1. Release Issue #[NNN] created (using .github/ISSUE_TEMPLATE/release_checklist.md)
2. Code review approval
3. Final manual verification of package publishing flow

**Post-merge steps:**
- Tag: `git tag -a v0.1.0-alpha -m "v0.1.0-alpha — Authority Substrate Frozen"`
- Publish: Run `.github/workflows/release-cd.yml` with environment=production
- Close release issue #[NNN]

---

**Branch:** release/0.1.0-alpha  
**Commit:** e60cc8c — chore(release): prepare v0.1.0-alpha
