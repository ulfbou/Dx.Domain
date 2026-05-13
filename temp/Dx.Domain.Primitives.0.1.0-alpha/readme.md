# Dx.Domain.Primitives

Strongly-typed identity and tracing primitives for domain modeling and observability.

## Purpose

Provide low-level value types for correlation, tracing, and identity that are safe to use across domain, application, and infrastructure boundaries without introducing dependencies.

## Guarantees

- Public types expose immutable state only
- Structural equality and stable hashing
- Canonical string formats using invariant culture
- Allocation-conscious parsing and formatting via IParsable and ISpanFormattable
- Thread-safe and zero-dependency beyond .NET runtime

## Constraints

- No implicit conversions to or from Guid, string, or numeric types
- No dependencies on Dx.Domain.Kernel
- No business logic, validation rules, or ambient context
- Empty values permitted only where explicitly documented

## Alpha Limitations

- API surface subject to change during alpha
- No compatibility guarantees prior to 0.1.0 stable
- Exact format and parsing rules may evolve, but canonical formats are stable within alpha
- Additional primitives may be added based on domain needs

## Role in System

- Foundational layer with no upstream dependencies
- Consumed by Dx.Domain.Facts for causation metadata
- Consumed by domain and application layers for identifiers
- Intentionally independent from Kernel to preserve layering

## Public API Surface

### Correlation and Tracing

- `CorrelationId`
  - Correlates related operations across system boundaries
  - Empty value permitted to represent uncorrelated context
  - Canonical format: 32 hex digits, no hyphens ("N")
  - Factories: New(), FromGuid(Guid), Parse, TryParse

- `TraceId`
  - 128-bit W3C Trace Context identifier
  - Empty value permitted (high=0, low=0)
  - Canonical format: 32 lowercase hex characters
  - Factories: New(), FromGuid(Guid), FromUInt128(ulong high, ulong low), Parse, TryParse

- `SpanId`
  - 64-bit W3C span identifier
  - Empty value permitted (0)
  - Canonical format: 16 lowercase hex characters ("x16")
  - Factories: New(), FromUInt64(ulong), Parse, TryParse

### Identity

- `UserId`
  - Identifies an actor
  - Empty values not permitted; FromGuid throws on Guid.Empty
  - Canonical format: 32 hex digits, no hyphens ("N")
  - Factories: New(), FromGuid(Guid), Parse, TryParse

- `FactId`
  - Identifies an immutable domain fact
  - Empty values not permitted; FromGuid throws on Guid.Empty
  - Canonical format: 32 hex digits, no hyphens ("N")
  - Factories: New(), FromGuid(Guid), Parse, TryParse

All types implement:
- `IIdentity` — marker for domain identities
- `IEquatable<T>` — structural equality
- `IParsable<T>` — Parse/TryParse
- `ISpanFormattable` — allocation-free formatting

## Usage

```csharp
var correlation = CorrelationId.New();
var trace = TraceId.New();
var span = SpanId.New();
var user = UserId.FromGuid(userGuid);

// Canonical formatting
string cid = correlation.ToString(); // "N" format by default
string tid = trace.ToString(); // 32 hex chars

// Parsing
if (CorrelationId.TryParse(input, null, out var parsed))
{
    // use parsed
}
```

## Versioning

0.1.0-alpha — API surface subject to change. No compatibility guarantees.

## See Also

- [Dx.Domain.Kernel](../Dx.Domain.Kernel/readme.md) — Functional core types
- [Dx.Domain.Facts](../Dx.Domain.Facts/readme.md) — Immutable facts with causation
- [Dx.Domain.Annotations](../Dx.Domain.Annotations/readme.md) — Metadata vocabulary
- [Architecture Decision Records](../../docs/adr/index.md) — Design rationale
