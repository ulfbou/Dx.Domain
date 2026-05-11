---
name: Test Failure
about: Report a failing analyzer test or AC regression
title: '[TEST] '
labels: type:bug, area:analyzers, status:needs-triage
assignees: ''
---

## Test Information
**Test Name:**
**AC Number:** (e.g., AC1, AC5)
**File:** `tests/Dx.Domain.Analyzers.Tests/...`

## Failure Details
**Expected:**
```
Expected diagnostic: DXA010 at line X
```

**Actual:**
```
Actual diagnostics:
```

## Reproduction
```bash
dotnet test --filter "FullyQualifiedName~TestName" -v n
```

## Environment
- **Branch:**
- **Commit:**
- **.NET SDK:**

## Analyzer Output
```
Paste test output here
```

## AC Impact
- [ ] Blocks alpha gate
- [ ] Regression from previous pass
- [ ] Flaky test (intermittent)

## Additional Context
<!-- Link to validation report, spec, or ADR -->
