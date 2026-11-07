# Changelog

All notable changes to Dx.Domain are documented here.

## [0.1.0-alpha] — 2026-05-15 — authority substrate frozen

### Added
- AnalyzerServicesFactory as single composition root
- Phase 0 authority substrate (Kernel, Primitives, Facts, Annotations)
- Core platform specification (`core-platform.md`)
- Normative package documentation:
  - Annotations
  - Kernel
  - Primitives
  - Facts
- Analyzer-enforced invariants:
  - construction authority (DXA010)
  - Result handling discipline (DXA020)
  - exception usage constraints (DXA022)

### Changed
- All 11 analyzers refactored to use centralized services
- Public API surface frozen for Kernel
- Documentation rewritten to reflect enforced invariants
- Public surface aligned with analyzer behavior

### Notes
- First public alpha release
- Core invariants are considered stable and release-locked
- API surface may evolve before 0.1.0 stable
- Breaking changes unlikely but possible prior to 0.1.0 stable

