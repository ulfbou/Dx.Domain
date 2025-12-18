# Dx.Domain — Phase 0 TODO
**Authority Substrate Freeze**

> Phase 0 is **PARTIALLY COMPLETE**. The substrate exists in code, but it is not yet sealed, tested, and consumed exclusively via `AnalyzerServices`. Do not declare Phase 0 frozen until every checkbox below is green in CI.

---

## Current Status (2026-05-15)

| Component | Status | Linked PR / Issue |
|-----------|--------|-------------------|
| Release pipeline | In review | PR #35 → closes #33 |
| Issue templates / governance | Done | PR #30, #31 |
| Docs canonicalization | Done | PR #13 (merged), #28 |
| Kernel facade surface | Done | PR #15 (merged) |
| Analyzer enforcement (DXA010-080) | Done | PR #17 (merged) |
| AC validation suite | Done | PR #19 (merged) |
| **AnalyzerServices substrate** | **In progress** | Issue #23, PR #24 |
| ScopeResolver / DxFacadeResolver / Classifier | Partial | PR #24 |
| ResultFlowEngineWrapper | Not started | — |
| EditorConfig round-trip tests | Not started | — |

**Blocker for v0.1.0-alpha:** #32 — cannot close until items 1-7 below pass.

---

## Phase 0 Exit Criteria

### 1. AnalyzerServices Substrate
**Requirement:** Single sealed composition root for all authority services.

- [ ] Implement `AnalyzerServices` as sealed record in `src/Dx.Domain.Analyzers/Infrastructure/AnalyzerServices.cs`
    - Constructor: `IScopeResolver, IDxFacadeResolver, ISemanticClassifier, IExceptionIntentClassifier, ResultFlowEngineWrapper, IGeneratedCodeDetector`
    - All properties `get`-only
- [ ] Refactor ALL analyzers to accept `AnalyzerServices` — remove every `new ScopeResolver(...)` and `new DxFacadeResolver(...)`
    - Current violations: see `git grep -n "new ScopeResolver" src/Dx.Domain.Analyzers` (12 hits)
- [ ] Tests:
    - [ ] `AnalyzerServicesTests` — sealed, immutable, non-null properties
    - [ ] Reuse test — same instance across multiple rule invocations

**Work:** Issue #23 → PR #24

### 2. ScopeResolver
**Requirement:** S0–S3 model with fail-open to S3.

- [ ] Implementation reads `dx.scope.map` and `dx.scope.rootNamespaces`
- [ ] Returns S3 when config missing, invalid, or assembly not mapped
- [ ] Tests in `ScopeResolverTests`:
    - [ ] Explicit map: `Dx.Domain` → S0
    - [ ] Missing map → S3
    - [ ] Root namespace prefix → S3
    - [ ] Invalid enum → no throw, returns S3

**Work:** Part of PR #24

### 3. DxFacadeResolver
**Requirement:** Canonical authority for facade factories.

- [ ] Implementation scans `Dx` public static factories only
- [ ] API: `IsDxFacadeFactory()`, `FindFacadeFactoryForType()`
- [ ] Tests in `DxFacadeResolverTests`:
    - [ ] Matches `docs/analyzers/dx.factories.md` exactly
    - [ ] Excludes internal/non-facade methods

**Work:** PR #15 (facade), PR #17 (enforcement) — needs test lock

### 4. SemanticClassifier
**Requirement:** Pure classification of domain types.

- [ ] Implementation per `design-decisions.md`
- [ ] Tests in `SemanticClassifierTests`:
    - [ ] IDs, Result, DomainError classified correctly
    - [ ] Consumer types not misclassified
    - [ ] Only accessed via `AnalyzerServices`

**Work:** Part of PR #24

### 5. IGeneratedCodeDetector
**Requirement:** Exempt generated code.

- [ ] Detects `[GeneratedCode]`, `CompilerGenerated`, `dx_generated_markers`
- [ ] Tests in `GeneratedCodeDetectorTests` for all three signals

**Work:** Part of PR #24

### 6. ResultFlowEngineWrapper
**Requirement:** Deterministic, cached, fail-open.

- [ ] Wrapper with cache key `(method, compilation, options)`
- [ ] Eviction on syntax change
- [ ] Returns empty graph on exception (never throws)
- [ ] Tests:
    - [ ] Determinism across runs
    - [ ] Cache invalidation
    - [ ] Failure injection → no diagnostics

**Work:** **NOT STARTED** — blocks Phase 0

### 7. Analyzer-Level Integration Tests
**Requirement:** PR-001 gates.

- [ ] Add `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` to test project
- [ ] `EditorConfigRoundTripTests` — verify `dx.scope.*` keys with minimal config, expect 0 diagnostics on kernel
- [ ] Reflection gate — enumerate facade surface, fail if undocumented

**Work:** PR #19 provides harness — needs the two tests above

### 8. Governance
**Requirement:** No kernel API leaks.

- [ ] All authority types `internal` to analyzers assembly
- [ ] Manual check: public API of `Dx.Domain` unchanged since 70f292a
- [ ] CHANGELOG entry: "v0.1.0-alpha — authority substrate frozen"

**Work:** PR #31, Issue #32

---

## Immediate Next Actions (in order)

1. **Merge PR #35** (release-cd) → unblocks tagging
2. **Land PR #24** with items 1-5 complete and tested
3. **Implement item 6** (ResultFlowEngineWrapper) — new branch `feat/analyzers/flow-wrapper`
4. **Add item 7 tests** — new branch `test/analyzers/editorconfig-roundtrip`
5. **Close #32** only after CI shows all 8 sections green

Do not create any new analyzer rules (DXAxxx) until Phase 0 checklist is fully checked — per contracts, rules must consume `AnalyzerServices` only.
