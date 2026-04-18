# Dx.Domain.Primitives

Dx.Domain.Primitives is a small, opinionated library of low-level domain primitives for .NET. It provides strongly-typed identifiers and tracing-related value objects that encapsulate primitive types (such as `Guid` and `ulong`) behind explicit, self-documenting types.

These primitives are designed to be:

- Immutable and value-based (structs with equality and hashing over their underlying values)
- Serialization-friendly (canonical string formats, `ToString`, and `TryFormat` support)
- Parseable from strings (`IParsable<T>` / `TryParse`)
- Suitable as keys in dictionaries and other collections

The library currently includes:

- `Dx.Domain.Primitives.ActorId`
- `Dx.Domain.CorrelationId`
- `Dx.Domain.FactId`
- `Dx.Domain.Primitives.SpanId`
- `Dx.Domain.Primitives.TraceId`

> Note that `CorrelationId` and `FactId` live directly under the `Dx.Domain` namespace, while `ActorId`, `SpanId`, and `TraceId` live under `Dx.Domain.Primitives`.

## Package goals

The `Dx.Domain.Primitives` package aims to:

- Replace raw `Guid`, `ulong`, and string identifiers with explicit domain types
- Provide predictable, well-defined string formats for logging, persistence, and interop
- Support modern .NET features like `IParsable<T>` and `ISpanFormattable` for efficient parsing/formatting

These primitives are intentionally minimal: they model identity and tracing concerns only, and do **not** contain business logic or generation policies beyond basic random creation helpers.

## Primitives overview

### ActorId (`Dx.Domain.Primitives.ActorId`)

A strongly-typed identifier for an actor.

- Backed by a non-empty `Guid`
- Namespace: `Dx.Domain.Primitives`
- Canonical format: `"N"` (32 hex characters, no separators)

Key members:

- `public Guid Value { get; }`
- `public static ActorId New()`
- `public static ActorId FromGuid(Guid value)`
- `public static ActorId Parse(string s, IFormatProvider? provider)`
- `public static bool TryParse(string? s, IFormatProvider? provider, out ActorId result)`
- `public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)`

Example:

```csharp
using Dx.Domain.Primitives;

ActorId id = ActorId.New();
string s = id.ToString();          // 32 hex chars, no dashes
ActorId parsed = ActorId.Parse(s, null);
```

### CorrelationId (`Dx.Domain.CorrelationId`)

Correlates related operations across system boundaries.

- Backed by a `Guid`
- Namespace: `Dx.Domain`
- Empty is allowed (`CorrelationId.Empty`) to represent "no correlation"
- Canonical format: `"N"` (32 hex characters, no separators)

Key members:

- `public static readonly CorrelationId Empty`
- `public Guid Value { get; }`
- `public static CorrelationId New()`
- `public static CorrelationId FromGuid(Guid value)`
- `public static CorrelationId Parse(string s, IFormatProvider? provider)`
- `public static bool TryParse(string? s, IFormatProvider? provider, out CorrelationId result)`
- `public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)`

Example:

```csharp
using Dx.Domain;

CorrelationId correlation = CorrelationId.New();
// Can be passed across services / messages for correlation
string headerValue = correlation.ToString();
```

### FactId (`Dx.Domain.FactId`)

Identifies an immutable domain fact.

- Backed by a non-empty `Guid`
- Namespace: `Dx.Domain`
- Empty is **not** allowed
- Canonical format: `"N"` (32 hex characters)

Key members:

- `public Guid Value { get; }`
- `public static FactId New()`
- `public static FactId FromGuid(Guid value)`
- `public static FactId Parse(string s, IFormatProvider? provider)`
- `public static bool TryParse(string? s, IFormatProvider? provider, out FactId result)`
- `public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)`

Example:

```csharp
using Dx.Domain;

FactId factId = FactId.New();
// Use as a stable identity for facts/events
var key = factId.ToString();
```

### SpanId (`Dx.Domain.Primitives.SpanId`)

Represents a 64-bit opaque span identifier, suitable for distributed tracing.

- Backed by a `ulong`
- Namespace: `Dx.Domain.Primitives`
- `SpanId.Empty` uses `0UL`
- Canonical format:
  - `ToString()` renders as 16 lowercase hex characters (`"x16"`)
  - `IParsable<SpanId>` uses unsigned decimal for parsing

Key members:

- `public static readonly SpanId Empty`
- `public ulong Value { get; }`
- `public static SpanId New()`
- `public static SpanId FromUInt64(ulong value)`
- `public static SpanId Parse(string s, IFormatProvider? provider)`
- `public static bool TryParse(string? s, IFormatProvider? provider, out SpanId result)`
- `public bool TryFormat(Span<char> destination, out int charsWritten)`
- `public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)`

Example:

```csharp
using Dx.Domain.Primitives;

SpanId span = SpanId.New();
string traceSegment = span.ToString(); // e.g. "0123abcd..."
```

### TraceId (`Dx.Domain.Primitives.TraceId`)

Represents a 128-bit trace identifier.

- Backed by two `ulong` fields (`_hi`, `_lo`)
- Namespace: `Dx.Domain.Primitives`
- `TraceId.Empty` has both parts set to `0UL`
- Canonical format for `ToString()` / `TryFormat`:
  - 32 lowercase hex characters (16 for high, 16 for low), no separators
- `Parse` / `TryParse` use a human-oriented `"hi:lo"` decimal format

Key members:

- `public static readonly TraceId Empty`
- `public static TraceId New()`
- `public static TraceId FromParts(ulong hi, ulong lo)`
- `public static TraceId Parse(string s, IFormatProvider? provider)`
- `public static bool TryParse(string? s, IFormatProvider? provider, out TraceId result)`
- `public bool TryFormat(Span<char> destination, out int charsWritten)`
- `public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)`

Example:

```csharp
using Dx.Domain.Primitives;

TraceId traceId = TraceId.New();
string wireValue = traceId.ToString(); // 32 hex chars, "hi" then "lo"

// Parsing from decimal "hi:lo" form
default(TraceId).TryParse("12345:67890", null, out var parsed);
```

## Usage notes

- All primitives are structs and implement `IEquatable<T>` so they work well as dictionary keys or set members.
- All parsing methods throw standard framework exceptions (`ArgumentNullException`, `FormatException`, `ArgumentException`) when inputs are invalid.
- `TryParse` / `TryFormat` APIs are provided where relevant for allocation-free usage in low-level code paths.
- Formatting generally defaults to invariant culture and stable, canonical forms suitable for logging and persistence.

## Target frameworks

`Dx.Domain.Primitives` currently targets:

- `net8.0`
- `net9.0`
- `net10.0`

These primitives are intended to be used from higher-level Dx.Domain components (kernel, generators, analyzers), but can also be used independently in your own applications or libraries.
