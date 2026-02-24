# Build & Governance (MSBuild + Non‑Silence)

**Scope:** How the repo enforces architectural intent during evaluation/build.

## Root Imports (Directory.Build.*)
- `Directory.Build.props` imports `builds/Dx.Props.hierarchical.props` once (single ingress).
- `Directory.Build.targets` imports defaults, identity resolution, governance, and analyzer governance (single choke point).

## Hierarchical Props (Fixed Import Pattern)
Use **ImportGroup** with one `<Import>` per file. Avoid item lists in import Conditions.
- ✅ `Import Project="common/Dx.Constants.props" Condition="Exists('common/Dx.Constants.props')"`
- ❌ `Import Project="@(DxImport)" Condition="'@(DxImport)' != ''"` (items are not available at that phase)

## Analyzer Governance (Non‑Silence)
- Disallow `RunAnalyzers=false` and `EnableNETAnalyzers=false` for non‑test projects.
- Block `.editorconfig` attempts to override `DX*` diagnostics.
- Promote `DX*` to errors in repo builds: `DXA*`, `DXK*`, `DXT*`, `DX100*`, `DX700*`.

## Intent Validation
- Exactly one Dx project‑kind per packable project (e.g., `DxIsLibrary`, `DxIsAnalyzer`).
- No conditional logic in `Dx.Constants.props` (constants must be unconditional facts).

## Identity Resolution
- Projects get a resolved role via props/targets (e.g., Library/Analyzer/Test) and can publish `DxLayer`/`DxResolvedRole` to analyzers via `CompilerVisibleProperty`.

> See also: `docs/analyzers/layer-awareness.md` for compiler‑visible properties and analyzer scope.
