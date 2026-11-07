# Changelog

All notable changes to Dx.Domain are documented here.

## [0.1.0-alpha] — 2026-05-15 — authority substrate frozen

### Added
- AnalyzerServicesFactory as single composition root
- Phase 0 authority substrate (Kernel, Primitives, Facts, Annotations)

### Changed
- All 11 analyzers refactored to use centralized services
- Public API surface frozen for Kernel

## [0.x] — Pre-release

### Added
- Core platform specification (`core-platform.md`)
- Normative package documentation:
    - Annotations
    - Kernel
    - Primitives
    - Facts
- Analyzer-enforced invariants:
    - construction authority
    - Result handling discipline
    - exception usage constraints

### Changed
- Documentation rewritten to reflect enforced invariants
- Public surface aligned with analyzer behavior

### Notes
- This is a pre-release
- API surface may change
- Core invariants are considered stable
