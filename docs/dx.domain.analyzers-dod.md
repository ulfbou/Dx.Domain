## A. Must‑pass “physics” (highest ROI)

1.  **Scope‑exactness locked (Authority ≠ Consumer)**
    *   **DoD**: All rules explicitly branch on `DxLayer`/`DxResolvedRole`/`IsTestProject`. No DXA\* fires in S0 (Kernel/Primitives/Annotations). Analyzer never probes `.dx/invariants.json` in authority scope.
    *   **Proof**: Analyzer unit tests (“authority immunity”) + repo‑shape tests pass; zero DXA diagnostics in foundations. 

2.  **Non‑silence axiom enforced (consumer builds)**
    *   **DoD**: Attempts to turn off analyzers, downgrade `DX*` in `.editorconfig`, or pragma‑suppress rules cause a **deterministic build failure** (DXA090 or governance target).
    *   **Proof**: CI job “non‑silence gate” runs a misuse sample and fails as expected. 

3.  **DXT004 (template contract) is deterministic**
    *   **DoD**: In consumer solutions, absence of `/.dx/invariants.json` triggers DXT004 (Error). In authority solutions, analyzers do **not** look for DXT.
    *   **Proof**: Unit tests for presence/absence; repo‑shape test solutions; authority build proves no DXT probing. 

4.  **Must‑have rule set implemented & green**
    *   **DoD**: DXA010, DXA020, DXA022, DXA060, DXT004, DXK001, DXK002, DXK003, DXK006, DXK007, DXK008: all have final analyzers + tests + docs.
    *   **Proof**: Rule charters complete; rule tests pass; DocFX generated successfully. 

5.  **Perf budget met (< 5 ms/method)**
    *   **DoD**: Analyzer perf benches on a method‑dense sample show average analysis time under 5 ms/method with a CI trend threshold; regressions fail the build.
    *   **Proof**: Benchmark report artifacts stored; CI trend gate configured. 

6.  **Determinism guaranteed**
    *   **DoD**: Given identical inputs, analyzers emit byte‑identical diagnostics (IDs, locations, messages). Generated code is exempt via `[GeneratedCode]`/markers.
    *   **Proof**: Golden diagnostics tests + generated‑code exemption tests green. 

7.  **Transitive distribution wired (no stray analyzer nupkg)**
    *   **DoD**: Kernel/Primitives/Annotations **ship** analyzers as `IncludeAssets="analyzers"; PrivateAssets="all"`. `Dx.Domain.Analyzers` is **non‑packable** by default; governance prevents accidental publish.
    *   **Proof**: Pack inspection shows analyzer assemblies under `analyzers/dotnet/cs/` for foundation packages; analyzer project cannot be packed standalone. 

8.  **Code‑fix coverage for the “paper cuts”**
    *   **DoD**: Safe fixes exist for:
        *   **DXK001**: insert `[assembly: DxAssemblyRole(...)]`
        *   **DXA020**: assign/return/propagate `Result`
        *   **DXA010**: suggest approved facade usage (where detectable)
        *   **DXT004**: add a minimal `/.dx/invariants.json` skeleton
    *   **Proof**: Code‑fix tests verify edits and preview. 

9.  **Docs: rule charters & layer‑awareness are complete**
    *   **DoD**: `docs/analyzers/charters/*` cover **Intent • Applies • Never applies • Classification • Examples • Remediation**. `docs/analyzers/layer-awareness.md` and build governance docs are up to date; DocFX builds with warnings‑as‑errors.
    *   **Proof**: DocFX job green; internal links validated. 

10. **Repository self‑hosting proof**
    *   **DoD**: Building the **Dx.Domain** repo with analyzers enabled requires **no** `/.dx/invariants.json`, produces **zero** consumer‑discipline diagnostics, and passes all analyzer tests & smoke samples.
    *   **Proof**: CI “framework integrity” stage green. 

***

## B. High‑value completeness (next ROI tier)

11. **Repo‑shape tests for dependency geometry**
    *   **DoD**: Synthetic solutions verify DXK002: forbidden edges (e.g., Domain→Infrastructure, Contracts→Kernel) are flagged; allowed edges pass.
    *   **Proof**: Repo‑shape test suite green. 

12. **Exception intent classification reliability (DXA022)**
    *   **DoD**: Classifier correctly buckets ArgumentValidation/InvariantViolation/ControlFlow/DomainControl; public Result methods only forbid “DomainControl” throws.
    *   **Proof**: Targeted classifier tests; mixed samples; boundary rethrows allowed. 

13. **Result‑flow CFG coverage (DXA020, DXA030)**
    *   **DoD**: Result creation/propagation/termination tracked; common patterns recognized; approved handlers configurable.
    *   **Proof**: Flow tests across direct returns, pattern‑matching returns, async flows, `using`/`await using`. 

14. **Generated‑code tagging culture (DXA070)**
    *   **DoD**: Generated code marked by attribute or configured markers; false positives minimized across source‑generated files.
    *   **Proof**: Tests cover both attribute and marker paths. 

15. **Forbidden vocabulary guard (DXA060) refined**
    *   **DoD**: Rule supports allow‑listing exceptions and namespace scoping to avoid hampering adapters; no false positives in authority/tests.
    *   **Proof**: Rule charter examples + tests with allow‑list configuration. 

***

## C. Nice‑to‑have before freeze (polish tier)

16. **DXA011 (Public factory) code‑fix hint**
    *   **DoD**: Offer quick action to make constructor internal or move creation to facade snippet.
    *   **Proof**: Code‑fix tests; no unsafe edits. 

17. **DXA040 (Kernel API freeze) opt‑in guard**
    *   **DoD**: When `build_property.DxKernelApiFreeze=true`, observational warnings surface API expansion in Kernel; ignored elsewhere.
    *   **Proof**: Opt‑in test suite green. 

18. **DXA050 (Temporal helper usage) clarity**
    *   **DoD**: Educational diagnostics point developers to adapter placement; rule off in authority/tests; warns in consumer.
    *   **Proof**: Samples & charter docs published. 

***

## D. Packaging & release mechanics (ship‑blocking hygiene)

19. **Analyzer assets in Foundation NuGets**
    *   **DoD**: Kernel/Primitives/Annotations packages contain `analyzers/dotnet/cs/*.dll` and SourceLink/SNUPKG/SBOM as per policy.
    *   **Proof**: Pack inspection in CI artifacts. 

20. **CHANGELOG & versioning discipline**
    *   **DoD**: Append‑only diagnostic ID catalog confirmed; any rule behavior changes captured under RC; SemVer declared; release notes include scope model & non‑silence.
    *   **Proof**: CHANGELOG entry and GitHub Release draft complete. 

21. **DocFX site & “Analyzer Suite Index”**
    *   **DoD**: Published Analyzer Index with links to charters, scope model, governance, enforcement guarantees, and admission test.
    *   **Proof**: Public (or internal) docs URL built from tag. 

***

## E. CI/CD gates (go/no‑go switches)

22. **PR pipeline (fast path)**
    *   **DoD**: Build analyzers, analyzer tests, non‑silence gate (suppression attempt), perf smoke, DocFX validate: all green.
    *   **Proof**: Required checks enforced on PR. 

23. **Main pipeline (extended)**
    *   **DoD**: Extended perf benchmarks; repo‑shape tests; doc publish to preview; SBOM/license scans; analyzer transitive shipping verified on nightly pack.
    *   **Proof**: Nightly “main” workflow passes; artifacts retained. 

24. **Tag pipeline (release)**
    *   **DoD**: Deterministic rebuild; sign; publish foundation packages (with analyzers); publish docs; create release with benchmark & coverage badges and diagnostic catalog snapshot.
    *   **Proof**: Release job complete; release page content validated. 

***

## F. Final acceptance (single‑screen executive summary)

*   ✅ **Authority immunity** proven; analyzers do not read DXT in S0. 
*   ✅ **Non‑silence** enforced; suppression/downgrade attempts fail CI. 
*   ✅ **Must‑have rules** implemented, tested, and documented; **code‑fixes** in place for high‑leverage rules. 
*   ✅ **Deterministic outputs** + **< 5 ms/method** perf budget met with CI trend guard. 
*   ✅ **Transitive shipping** only; analyzer project itself not packable; SourceLink/SBOM set. 
*   ✅ **DocFX** Analyzer Index + Charters + Layer Awareness + Governance published from tag. 
