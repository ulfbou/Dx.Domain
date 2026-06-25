# Dx.Domain.Analyzers

Compile-time governance for Dx.Domain.

## Purpose

Provide Roslyn analyzers that enforce Dx.Domain architectural invariants at build time without modifying program behavior.

## Guarantees

- Diagnostics only; analyzers never rewrite code
- Stable diagnostic IDs within a major version
- No runtime dependencies or performance impact
- Respects generated code boundaries

## Constraints

- Requires .NET SDK with Roslyn 4.8 or later
- Runs during compilation only
- Configuration via .editorconfig or MSBuild properties
- No automatic code fixes in alpha

## Alpha Limitations

- Rule coverage is partial and focused on kernel integrity and Result discipline
- Diagnostic messages and severities may evolve during alpha
- Scope detection relies on DxLayer MSBuild property and assembly attributes
- No Visual Studio code fixes or refactorings yet

## Role in System

- Shipped embedded in Dx.Domain.Kernel, Dx.Domain.Primitives, Dx.Domain.Facts, and Dx.Domain.Annotations
- Enforces construction authority, Result handling, and kernel surface discipline
- Complements runtime types by making misuse visible at compile time

## Diagnostics (v0.1.0-alpha)

### Construction and Authority
- **DXA010** Construction Authority Violation — Create domain instances via the Dx facade. Direct construction or public factory use is forbidden outside kernel packages.
- **DXA011** Public Factory Exposure — Public construction surface on domain type detected. Make constructor or factory internal and expose creation via Dx facade.
- **DXA080** Facade Invariant Enforcement Missing — Facade factory does not enforce invariants. Ensure invariants are checked and failures return DomainError or Result.

### Result Discipline
- **DXA020** Result Ignored — Result value is produced and ignored. Either handle, return, or explicitly discard with intent.
- **DXA022** Discouraged Domain Control Exception — Use Result.Failure instead of throwing exception in Result-returning method.
- **DXA030** Unapproved Handler Usage — Result passed to an unapproved handler. Register the handler in .editorconfig or use a known adapter.

### Kernel Integrity
- **DXA040** Kernel Public Surface Freeze — New public kernel API detected. Provide DPI justification and confirm it cannot live at the edges.
- **DXA050** Temporal Helper Usage in Kernel — Temporal or policy-sensitive helper used in kernel. Move to edge package or justify via DPI.
- **DXA060** Forbidden Vocabulary in Kernel — Forbidden vocabulary used in kernel. Move to adapter or rename to mechanical term.

### Documentation and Generation
- **DXA065** Unresolved XML Documentation Reference — XML doc cref could not be resolved. Fix the reference or suppress intentionally.
- **DXA070** Generated Code Tagging — Generated code missing required generator tag. Add [GeneratedCode] attribute or configured marker.

## Configuration

Scope is determined by the `DxLayer` MSBuild property set by each package (Kernel=S0, Primitives=Shared, etc.).

In repository builds, Dx diagnostic severities are governed centrally. `.editorconfig` entries that override `dotnet_diagnostic.DX*` severities are blocked by build policy.

Supported configuration is limited to non-severity analyzer inputs such as scope mapping, approved handlers, and generated-code markers.

## Versioning

0.1.0-alpha — Diagnostic IDs are stable within alpha. No compatibility guarantees for messages or default severities.

## See Also

- [Dx.Domain.Kernel](../Dx.Domain.Kernel/readme.md) — Runtime types enforced by analyzers
- [Dx.Domain.Primitives](../Dx.Domain.Primitives/readme.md) — Identity primitives
- [Dx.Domain.Facts](../Dx.Domain.Facts/readme.md) — Fact modeling
- [Dx.Domain.Annotations](../Dx.Domain.Annotations/readme.md) — Metadata vocabulary
