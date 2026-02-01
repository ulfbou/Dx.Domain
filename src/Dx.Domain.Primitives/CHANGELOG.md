# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Nothing yet

### Changed
- Nothing yet

### Deprecated
- Nothing yet

### Removed
- Nothing yet

### Fixed
- Nothing yet

### Security
- Nothing yet

---

## [0.1.0] - 2026-01-17

### Added
- Initial public release of the Dx.Domain family of packages:
  - `Dx.Domain.Annotations`: Semantic vocabulary and contracts.
  - `Dx.Domain.Kernel`: Result type, invariants, domain errors, and functional helpers.
  - `Dx.Domain.Primitives`: Strongly-typed primitives for identity and tracing (`ActorId`, `CorrelationId`, `FactId`, `SpanId`, `TraceId`).
  - `Dx.Domain.Generators`: Roslyn-based generators for domain models and artifacts.
  - `Dx.Domain.Analyzers`: Roslyn analyzers for enforcing Dx.Domain usage and result/flow patterns.
- Multi-targeting support for .NET 8, .NET 9, and .NET 10 where applicable.
- CI-ready solution layout with separate projects for runtime, generators, analyzers, and tests.

### Technical
- Established conventions for:
  - Package metadata (authors, license, repository URLs).
  - Documentation artifacts (`README.md`, `CHANGELOG.md`, `LICENSE` links from packages).

---

[Unreleased]: https://github.com/ulfbou/dx.domain/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/ulfbou/dx.domain/releases/tag/v0.1.0
