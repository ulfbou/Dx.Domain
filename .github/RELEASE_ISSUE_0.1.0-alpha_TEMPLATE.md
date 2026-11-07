---
**Title:** [RELEASE] v0.1.0-alpha — Authority Substrate Frozen

**Description:** Track final verification and publication of Dx.Domain v0.1.0-alpha (first public alpha).

---

## Release Metadata

| Property | Value |
|----------|-------|
| **Version** | 0.1.0-alpha |
| **Release Date** | 2026-05-15 |
| **Codename** | Authority Substrate Frozen |
| **Scope** | Phase 0: Kernel, Primitives, Facts, Annotations, 11 Analyzers |
| **TFMs** | .NET 8, .NET 9, .NET 10, .NET Standard 2.0 |

## Pre-Release Checklist

### Automated Validation
- [x] All ACs have automated tests
- [x] `dotnet build -c Release` succeeds on all TFMs
- [x] All tests passing (53/53, 0 failed)
- [x] Diagnostic severities correct (DXA010–DXA080)
- [x] Release build: 0 errors, 0 warnings (src/)
- [x] NuGet packs: 5 packages produced successfully

### Documentation
- [x] CHANGELOG.md consolidated and released
- [x] Public API docs complete (XML docstrings)
- [x] AnalyzerServicesFactory docs fixed (CS1570 resolved)
- [x] README.md reflects release status
- [x] Package-level READMEs aligned
- [x] DocFX preview reviewed

### Code Quality
- [x] No new public API surface in Kernel (DXA040 compliance)
- [x] Nullability: clean (src/ has 0 CS warnings)
- [x] Result handling discipline enforced (DXA020)
- [x] Conventional commits: all messages conform
- [x] global.json: SDK roll-forward set to latestPatch

### Release Readiness
- [x] PR created: release/0.1.0-alpha → main
- [x] PR labels applied: type:release, prio:P0, area:governance
- [x] CI/CD workflow present: .github/workflows/release-cd.yml
- [x] Secrets configured (signing certificate, NuGet API key)
- [x] Package metadata complete (Authors, Description, License, RepositoryUrl, Tags)

## Manual Verification

- [ ] Review PR diff and approve
- [ ] Verify package signing configuration
- [ ] Test NuGet publish to staging (if applicable)
- [ ] Confirm git tag: `v0.1.0-alpha`
- [ ] Verify GitHub release creation
- [ ] Check NuGet.org package visibility (after publish)

## Publishing Steps (Post-Merge)

1. **Tag the release:**
   ```bash
   git tag -a v0.1.0-alpha -m "v0.1.0-alpha — Authority Substrate Frozen"
   git push origin v0.1.0-alpha
   ```

2. **Trigger release workflow:**
   - Go to `.github/workflows/release-cd.yml`
   - Run workflow with:
	 - **environment:** production
	 - **version:** 0.1.0-alpha (auto-detected from tag)

3. **Monitor publishing:**
   - Observe artifact signing
   - Verify NuGet push to nuget.org
   - Confirm GitHub release auto-created

4. **Post-publish verification:**
   - NuGet.org: https://www.nuget.org/packages/Dx.Domain.Kernel/0.1.0-alpha
   - NuGet.org: https://www.nuget.org/packages/Dx.Domain.Analyzers/0.1.0-alpha
   - GitHub releases: https://github.com/ulfbou/Dx.Domain/releases/tag/v0.1.0-alpha

## Blockers & Risks

**None identified.** Release is validated and ready for publication.

---

## Related

- **PR:** release/0.1.0-alpha (#[TBD])
- **Changelog:** [docs/public/changelog.md](../../docs/public/changelog.md)
- **Commit:** e60cc8c — chore(release): prepare v0.1.0-alpha

**Assignee:** @ulfbou  
**Labels:** area:governance, type:chore, prio:P0, status:needs-triage
