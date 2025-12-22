Phase 0 is only partially in place. Some of the “authority substrate” exists, but from the perspective of the contracts and the ROADMAP, you cannot yet declare Phase 0 complete.

Below is a checklist keyed to your Phase 0 definition and the contracts docs, focusing on what must be true for Phase 0 to be “frozen”.

I’ll treat Phase 0 as done only if all bullets under each heading are satisfied by code + tests in this repo.

---

**1. AnalyzerServices Substrate**

Phase 0 requires a sealed authority layer that every rule consumes, not ad‑hoc wiring.

You must have:

- A `AnalyzerServices` (or equivalent) sealed record/type that:
  - Is the single composition root for:
    - `IScopeResolver`
    - `IDxFacadeResolver`
    - `ISemanticClassifier`
    - `IExceptionIntentClassifier`
    - `ResultFlowEngineWrapper`
    - `IGeneratedCodeDetector`
  - Is immutable: all dependencies passed via constructor and exposed as `get`-only properties.
  - Is used consistently by any existing analyzer or handler (no direct `new ScopeResolver(...)` etc. inside rules).

**If this is not already true**, Phase 0 work:

- [ ] Implement `AnalyzerServices` exactly once, in `src\Dx.Domain.Analyzers\Infrastructure\AnalyzerServices.cs`, with:
  - Constructor taking all authority services.
  - Public, get-only properties for each service.
- [ ] Refactor any existing analyzers/handlers to accept `AnalyzerServices` instead of constructing infra types directly.
- [ ] Add unit tests that:
  - [ ] Assert the record is sealed and properties are non-null.
  - [ ] Verify one `AnalyzerServices` instance can be reused across multiple rule invocations without mutation.

---

**2. ScopeResolver**

Contracts/roadmap expect:

- Scope model: S0–S3, with config-driven behavior.
- Fail‑open semantics to S3 when config is missing/underspecified.

You must have:

- An implementation of `IScopeResolver` (`ScopeResolver`) that:
  - Reads `dx.scope.map` and `dx.scope.rootNamespaces` from `AnalyzerConfigOptionsProvider`.
  - Correctly maps assemblies to S0–S3.
  - Defers to S3 if:
    - No map is present; or
    - Assembly not present in map; or
    - Root namespaces don’t match.

- Tests that exercise:
  - [ ] Explicit map mapping `Dx.Domain` to S0.
  - [ ] Missing map → returns S3 for arbitrary assemblies.
  - [ ] Root namespace prefix matches produce S3 as default for “consumer” assemblies.
  - [ ] Misconfiguration (e.g., invalid enum values) does not throw; resolver still returns S3.

**If any of these tests don’t exist or fail**, Phase 0 work:

- [ ] Finish `ScopeResolver` implementation according to `canonical-scope-model.md`.
- [ ] Add a dedicated test suite, e.g. `ScopeResolverTests`, covering:
  - All the bullet scenarios above.
  - Edge cases like multiple root namespaces, mixed case, and whitespace noise.

---

**3. DxFacadeResolver**

The roadmap + DXA010 contract require:

- A canonical authority for which methods are valid facades.
- A stable “facade surface” that matches your `dx.factories.md` (or equivalent).

You must have:

- An `IDxFacadeResolver` implementation that:
  - Scans `Dx.Domain` (and optionally other assemblies) for public static factory methods that are allowed construction authorities.
  - Exposes:
    - `IReadOnlyCollection<IMethodSymbol>` or `IReadOnlySet<IMethodSymbol>` of all recognized facade factories.
    - A way to query by type (`FindFacadeFactoryForType`) and predicate (`IsDxFacadeFactory`).
- Tests that:
  - [ ] For a synthetic compilation with a `Dx` facade:
    - Only public static methods under `Dx` (and its nested “facade” classes) are discovered.
    - Internal or non-facade methods are not discovered.
  - [ ] The discovered symbol set exactly matches what’s documented in `dx.factories.md` (you can assert against a curated list for now).

**If any of the above is missing**, Phase 0 work:

- [ ] Complete `DxFacadeResolver` implementation in `Infrastructure\Facades`.
- [ ] Add `DxFacadeResolverTests` that:
  - Use Roslyn compilations built from local code snippets.
  - Assert you only surface the intended factories, and everything else is excluded.

---

**4. SemanticClassifier**

Docs refer to classification of domain types (value objects, IDs, facts, result-like types, etc.).

You must have:

- An `ISemanticClassifier` implementation that:
  - Encodes the semantics from `design-decisions.md` and your domain modeling rules (e.g., what is a value object, what counts as a domain ID).
  - Is pure and deterministic for a given `Compilation`/`ISymbol`.

- A test suite ensuring:
  - [ ] Known domain types (IDs, Result, DomainError, etc.) are classified correctly.
  - [ ] Non-domain/consumer types are not misclassified.
  - [ ] The classifier does not reach out to Roslyn directly in analyzers; it’s always consumed via `AnalyzerServices`.

**If not present**, Phase 0 work:

- [ ] Implement `ISemanticClassifier` in `Infrastructure`.
- [ ] Add `SemanticClassifierTests` with a small Roslyn test-assembly per classification scenario.

---

**5. IGeneratedCodeDetector**

Contracts specify:

- Generated code must be exempted from most rules (or specially treated).
- Tagging through attributes and/or namespace markers.

You must have:

- `IGeneratedCodeDetector` + implementation that:
  - Detects `[GeneratedCode]`, compiler‑generated attributes, and `dx_generated_markers` namespaces.
  - Is used by rule scaffolding to short‑circuit detection for generated code.

- Tests that:
  - [ ] For symbols with `[GeneratedCode]` or `CompilerGeneratedAttribute`, `IsGenerated` returns `true`.
  - [ ] For code under namespaces configured in `dx_generated_markers`, `IsGenerated` returns `true`.
  - [ ] Regular user code is not marked as generated.

**If any of that is missing**, Phase 0 work:

- [ ] Finish the detector (it exists but ensure config + attributes and namespacing all cooperate).
- [ ] Add `GeneratedCodeDetectorTests` for all three signal types (attribute, compiler‑generated, namespace marker).

---

**6. ResultFlowEngineWrapper and FlowGraph Determinism**

Roadmap + `analyzer-contracts.md` demand:

- Deterministic `ResultFlowEngine`.
- Caching/eviction semantics.
- Authority wrapper that analyzers call, not direct `ControlFlowGraph` use.

You must have:

- `ResultFlowEngine` + a `ResultFlowEngineWrapper` that:
  - Accepts `IMethodSymbol` + `AnalyzerConfigOptions` + `Compilation`.
  - Caches analysis results keyed appropriately (e.g., `(method, compilation version, options)`).
  - Evicts / invalidates on syntax or semantic changes.
  - Always fails open: on any failure, returns an empty `FlowGraph` (no diagnostics), never throws.

- Tests that:
  - [ ] For an identical `Compilation` and method, `Analyze` returns bit‑identical `FlowGraph` across multiple runs.
  - [ ] For simple changes to method body, cache is invalidated and new graph is produced.
  - [ ] For partial methods, conditional compilation, async/await, etc., graph is stable from run to run.
  - [ ] Any internal exception thrown during analysis is converted to a benign, empty graph rather than leaking.

**If any of this is not yet true**, Phase 0 work:

- [ ] Implement `ResultFlowEngineWrapper` in `Infrastructure\Flow`, with explicit caching and invalidation semantics.
- [ ] Add tests:
  - `ResultFlowEngineDeterminismTests` (snapshot or equivalence tests).
  - `ResultFlowEngineCacheTests` (ensure mutation invalidations).
  - A smoke test that intentionally injects a failure path and confirms the rule receives “no diagnostics”.

---

**7. Analyzer-Level Integration Tests**

Phase 0’s PR-001 also listed:

- `EditorConfigRoundTripTests`
- Reflection gate for facade surface
- “No rule logic merged” — only substrate.

You must have:

- `EditorConfigRoundTripTests` that:
  - Use the Roslyn analyzer test harness (`CSharpAnalyzerTest<EmptyAnalyzer, XUnitVerifier>` or equivalent).
  - Run an analyzer using a `.editorconfig` that defines minimum required `dx` keys (e.g., `dx.scope.map`, `dx.scope.rootNamespaces`).
  - Assert **no diagnostics** under minimal S0/S3 config for the `Dx.Domain` kernel baseline.

- Reflection gate tests that:
  - [ ] For the current `Dx.Domain` assembly, enumerate the expected facade factories and assert they match the resolver’s view.
  - [ ] Fail the build/test run if a new facade appears without being added to `dx.factories.md` (or your contract source).

**If currently missing/incomplete**, Phase 0 work:

- [ ] Add `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` package to `Dx.Domain.Analyzers.Tests`.
- [ ] Implement `EditorConfigRoundTripTests` using the intended `CSharpAnalyzerTest` shape and verify the `dx.scope` keys behave per the contracts.
- [ ] Add a simple reflection-based test verifying the facade surface.

---

**8. Governance Constraints**

Phase 0 exit criteria also include governance:

- “No public kernel API changes”
- “DPI not required (infrastructure only)”
- “Authority substrate frozen”

Concrete enforcement:

- [ ] Ensure all new authority-layer types are `internal` or `public` only in the analyzer assembly, not leaking into `Dx.Domain`.
- [ ] Add a test or build step that:
  - Fails if the public surface of `Dx.Domain` changes without a corresponding DPI document update (can be a later phase, but at Phase 0 you should at least have a manual check noted).

---

**Summary: Can Phase 0 Be Declared Complete Today?**

Only if you can honestly check off all of:

- AnalyzerServices present, immutable, and consumed by all infra & rules.
- `ScopeResolver`, `DxFacadeResolver`, `SemanticClassifier`, `IGeneratedCodeDetector`, `ResultFlowEngineWrapper` all implemented according to the contracts and used via `AnalyzerServices`.
- Determinism and fail‑open behavior are covered by explicit tests.
- EditorConfig round-trip and facade reflection gates are in place and passing.
- No rule logic PRs are piggy-backing infra changes without DPI justification.

If any of those are missing or only “manually verified”, Phase 0 is **not yet frozen**.

Your immediate next steps, in order:

1. Finish/refine infra implementations to conform exactly to the contracts (scope model, exception model, design decisions).
2. Build out unit tests for each infra component to lock behavior.
3. Wire up the Roslyn analyzer test harness and implement the EditorConfig round-trip test as originally intended.
4. Add a small reflection-based facade-surface test.
5. Once all those tests pass and no rules directly talk to Roslyn services, explicitly mark Phase 0 as frozen (e.g., in `ROADMAP.md` and a short `CHANGELOG` entry).
