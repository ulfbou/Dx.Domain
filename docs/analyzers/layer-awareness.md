# Analyzer Layer Awareness & Scope

**Goal:** Kernel must remain self‑hosting; consumer discipline must never constrain authority layers.

## Compiler‑Visible Properties
Expose these MSBuild properties to Roslyn analyzers:
```xml
<PropertyGroup>
  <DxLayer>Consumer</DxLayer>
</PropertyGroup>
<ItemGroup>
  <CompilerVisibleProperty Include="DxLayer" />
  <CompilerVisibleProperty Include="DxResolvedRole" />
  <CompilerVisibleProperty Include="IsTestProject" />
</ItemGroup>
```

**Layer Semantics**
- Kernel/Primitives/Annotations → **exempt** from consumer DXA rules.
- Contracts/Shared/Services (Consumers) → **enforced**.
- Tests → **lightened** or exempt as configured.

**Fallbacks**
- Analyzer may infer using assembly name/path (`Dx.Domain.Kernel`, etc.) when `DxLayer` is absent, to remain functional for external adopters.
