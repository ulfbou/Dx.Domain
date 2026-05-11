# Changelog

## [0.1.0-alpha] - 2026-05-08

### Added
- DXA010 Construction Authority Violation
- DXA011 Public Factory Exposure
- DXA020 Result Ignored
- DXA022 Discouraged Domain Control Exception
- DXA030 Unapproved Handler Usage
- DXA040 Kernel Public Surface Freeze
- DXA050 Temporal Helper Usage in Kernel
- DXA060 Forbidden Vocabulary in Kernel
- DXA065 Unresolved XML Documentation Reference
- DXA070 Generated Code Tagging
- DXA080 Facade Invariant Enforcement Missing

### Notes
- This is an alpha release. Diagnostic IDs are stable within the alpha series
- Analyzers are shipped embedded in Dx.Domain.Kernel, Dx.Domain.Primitives, Dx.Domain.Facts, and Dx.Domain.Annotations packages
- No separate NuGet package is published for analyzers
- All diagnostics are reported as warnings by default and can be configured via .editorconfig
- No code fixes are provided in this release; diagnostics are guidance only
