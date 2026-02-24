Here is the complete, rewritten **PR Review Checklist**.

I have integrated the **Authority vs. Consumer** bifurcation while keeping every single technical check regarding MSBuild hygiene, identity resolution, and analyzer distribution intact. This version ensures that the "Legislature" (Framework) is held to its own internal standards without being choked by consumer-level "Traffic Laws."

---

# PR Review Checklist — MSBuild & csproj Discipline (v001)

Copy this section into your PR description and check each item.

---

## 1. Scope & Jurisdiction (Authority vs. Consumer)

* [ ] **Authority Stamping:** New or modified authority projects have the correct local `Directory.Build.props`:
* `src/Dx.Domain.Kernel/` → `DxLayer=Kernel`
* `src/Dx.Domain.Primitives/` → `DxLayer=Primitives`
* `src/Dx.Domain.Annotations/` → `DxLayer=Annotations`


* [ ] **No DXT in Authority:** Verified that NO `.dx/invariants.json` has been introduced into the `dx.domain` repository. (DXT is a consumer-only artifact).
* [ ] **Short-Circuiting:** If modifying analyzers, verified that consumer-discipline rules (`DXA*`) short-circuit when `DxLayer != Consumer` or `IsTestProject=true`.

## 2. Pre-flight (Structure)

* [ ] No per-project imports of governance/analyzer targets.
* [ ] Only the root uses the choke-point: `Directory.Build.targets`.
* [ ] Hierarchical props import uses `ImportGroup` + explicit `Import` (no `@(Items)` in `Import` conditions).
*Sanity check against MSB4099-style issues.* **Reference:** `/builds/Dx.Props.hierarchical.props`.

## 3. Identity & Layer

* [ ] Root `Directory.Build.props` sets default `<DxLayer>Consumer</DxLayer>`.
* [ ] Root `Directory.Build.props` exposes to analyzers:
`<CompilerVisibleProperty Include="DxLayer|DxResolvedRole|IsTestProject" />`.
* [ ] **Tests:** Either set `<IsTestProject>true</IsTestProject>` or set `<DxIsTest>true</DxIsTest>` **and** root normalizes it to `IsTestProject`.
**Reference:** `/builds/common/Dx.Defaults.props`.

## 4. Targets Ordering (Root Choke Point)

* [ ] `Directory.Build.targets` imports identity **first**:
`<Import Project="$(DxBuildRoot)identity/Dx.ResolveIdentity.targets" ... />`
* [ ] Then governance:
`<Import Project="$(DxBuildRoot)policy/Dx.Governance.targets" ... />`
`<Import Project="$(DxBuildRoot)policy/Dx.DomainAnalyzerGovernance.targets" ... />`

## 5. DX-First Analyzer Distribution

* [ ] **Kernel & Annotations:**
- `PackageReference` to `Dx.Domain.Analyzers` with `IncludeAssets="analyzers"`, `PrivateAssets="all"`.
- **Repo Fallback:** `ProjectReference` to `Dx.Domain.Analyzers` with `OutputItemType="Analyzer"`, `ReferenceOutputAssembly="false"`.
* [ ] Any local copy target for analyzer deps is **safety net only**; analyzer nupkg should ship required deps under `analyzers/`.

## 6. Governance Expectations (No Expansion)

* [ ] Non-test projects cannot disable analyzers; `DX*` severities cannot be overridden in `.editorconfig`.
* [ ] Non-test projects escalate `DXA*`, `DXK*`, `DXT*`, `DX100*`, `DX700*` to errors (central policy).
* [ ] Packable projects declare role; constants purity checks use guarded IO.
**References:** `/builds/policy/Dx.Governance.targets`, `/builds/policy/Dx.DomainAnalyzerGovernance.targets`.

## 7. Test/Projects Hygiene

* [ ] Test projects are correctly marked `IsTestProject=true`.
* [ ] Remove OOB framework packages (e.g., `System.Net.Http`) from `netX.Y` tests unless strictly required (prevents binding conflicts).

## 8. CI Ratchet

* [ ] CI uses `/builds/ci/Dx.CI.props` (`ContinuousIntegrationBuild=true`, `TreatWarningsAsErrors=true`).
* [ ] **Framework Integrity:** Verified that `dx.domain` CI builds correctly without any `.dx/invariants.json` present.

## 9. DXT (Templates Only)

* [ ] **Template Emission:** If modifying `dx.templates`, verified that templates emit `.dx/invariants.json` describing role/ref constraints.
* [ ] **Analyzer Fallback:** Verified that analyzers correctly fall back to layer/role physics if DXT is absent (as is required for authority repo builds).

## 10. Final Acceptance (Build + Analyzers)

* [ ] `dotnet build` at repo root succeeds with identity resolved **before** governance diagnostics.
* [ ] **Authority Purity:** Authority layers + tests result in **zero** consumer-discipline `DXA*` diagnostics.
* [ ] **Consumer Verification:** Sample consumer misuse triggers `DXA010`/`DXA022` as expected (verified via analyzer unit tests).

---
