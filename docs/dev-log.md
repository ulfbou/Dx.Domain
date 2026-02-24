# Dev Log

## Current iteration
- Kernel: updated `DXT004` analyzer to use scope resolution so authority/test scopes skip invariant file enforcement consistently.
- Facts: added `IDomainFact<TPayload>`, `DomainFactExtensions.TryGetPayload<T>`, `Fact<TPayload>.TryCreate`, `TransitionResult<TState>.FactsImmutable`, and introduced `FactType`/`FactTypeOf<TPayload>` helpers.
- Docs: updated `docs/dx.domain.facts-spec.md` checklist to reflect completed items.
- Analyzers: aligned `DXA070` and `DXA080` to consumer-only scope enforcement.
- Analyzers: added `DXA090` suppression detection for DX* pragma and attribute suppressions in consumer scope.
- Docs: updated `docs/dx.domain.analyzers-spec.md` to reflect DXA090 implementation.
- Tests: added `DXA090` analyzer tests for pragma and SuppressMessage suppression detection.
- Tests: switched analyzer test helper to `XunitCompatVerifier` to avoid xUnit verifier version mismatch; fixed its `PushContext` implementation.
- Tests: added `DXA090` analyzer coverage with suppression-check skipping to validate pragma/SuppressMessage detection.
- Tests: aligned `xunit.runner.visualstudio` to v2 for analyzer tests to avoid xUnit verifier runtime mismatch.
- Tests: introduced kernel stubs in analyzer test helper to avoid net10 System.Runtime reference mismatches.
- Analyzers: hardened `ScopeResolver` and `RoleResolver` attribute fallbacks and improved governance targets path lookup in tests.

### Noted implemented but not previously logged
- `Causation` struct and `Fact<TPayload>` factory APIs are already present per `docs/dx.domain.facts-spec.md`.
- `TransitionResult<TState>` factories (`Success`, `Failure`) and deconstruction helpers already exist in code.
- `TransitionResult<TState>` composition APIs (`Map`, `Bind`, LINQ) and `ImmutableArray` fact storage are now implemented in code.

### Recommended next ten items
1. Analyzers: add authority/test scope unit tests for DXA/DXK/DXT rules (authority immunity and test exemptions).
2. Analyzers: re-run DXA020/DXA022/DXA060 tests to confirm kernel stub behavior.
3. Analyzers: add code-fix providers for DXA020, DXA010, DXK001, and DXT004 per DoD.
4. Analyzers: verify all DXA/DXK analyzers enforce scope via `ScopeResolver` and remove any heuristics.
5. Analyzers: add tests for DXT004 presence/absence and authority immunity.
6. Analyzers: add generated-code exemption tests for DXA070 and other syntax-based rules.
7. Analyzers: create repo-shape tests for DXK002 dependency geometry rules.
8. Analyzers: implement performance benchmarks to validate <5ms/method budget.
9. Facts: review `Causation` equality semantics and document any change in CHANGELOG.
10. Facts: add unit tests for `TransitionResult` composition and `TryGetPayload` helpers.
