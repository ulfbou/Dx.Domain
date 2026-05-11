---
name: Release Checklist
about: Track a version release and final verification steps
title: '[RELEASE] '
labels: area:governance, type:chore, prio:P0, status:needs-triage
assignees: ulfbou
---

## Release
**Version:**
**Milestone:**

## Context
<!-- Link to previous checklist or related issues -->
Continues #

## Automated Validation
- [ ] All ACs have automated tests
- [ ] `dotnet build` succeeds on all TFMs
- [ ] All tests passing
- [ ] Diagnostic severities correct

## Manual Verification
- [ ] Update docs/todo.md Phase 0 checklist
- [ ] Add CHANGELOG entry
- [ ] Verify no new public types in Dx.Domain.Kernel
- [ ] Run: `git grep -n "new ScopeResolver\|new DxFacadeResolver" src/Dx.Domain.Analyzers`
- [ ] Tag release
- [ ] Publish API docs to gh-pages

## Documentation
- [ ] README updated with release status
- [ ] Package readmes aligned with precision model
- [ ] DocFX published

## Related
- PRs:
- Issues:
