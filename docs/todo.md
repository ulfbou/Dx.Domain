# Dx.Domain — Phase 0 TODO
**Authority Substrate Freeze — COMPLETE 2026-05-15**

## Phase 0 Exit Criteria

### 1. AnalyzerServices Substrate
- [x] Implement `AnalyzerServices` as sealed record
- [x] Refactor ALL analyzers — remove every `new ScopeResolver(...)`
    - Verified: `git grep` returns 0 hits in Analyzers/
- [x] Tests: AnalyzerServicesTests passing