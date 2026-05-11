# Changelog

## [0.1.0-alpha] - 2026-05-08

### Added
- CorrelationId — correlation identifier, empty permitted, canonical format "N"
- TraceId — 128-bit W3C trace identifier, canonical format 32 hex chars
- SpanId — 64-bit W3C span identifier, canonical format 16 hex chars
- UserId — non-empty actor identifier, canonical format "N"
- FactId — non-empty fact identifier, canonical format "N"

### Notes
- This is an alpha release. APIs are functional but may evolve prior to 0.1.0 stable
- All types are immutable readonly structs with structural equality
- Parsing and formatting are allocation-conscious via ISpanFormattable
- No dependencies on Dx.Domain.Kernel; primitives remain at the bottom of the dependency graph
