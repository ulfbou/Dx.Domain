# Changelog

## [0.1.0-alpha] - 2026-05-08

### Added
- Result<TValue, TError>, Result<TValue>, Result static factories
- DomainError, InvariantError, InvariantViolationException
- Dx.Require validation surface (That overloads for recoverable validation)
- Invariant.That assertion surface for structural invariants
- Functional combinators: Map, Bind, Match, Tap, Ensure, MapError, Recover
- Try-pattern helpers for exception-to-Result translation
- DomainTime (UTC-only timestamp)
- Unit void type

### Changed
- Normalized error codes to dx.kernel.* namespace
- Result APIs made allocation-conscious with aggressive inlining

### Notes
- This is an alpha release. APIs are functional but may evolve prior to 0.1.0 stable
- Kernel exposes no ambient context, logging, IO, or policy
- All public types are immutable and thread-safe
