# Dx.Domain.Facts

**Version:** 0.9.0-preview  
**Status:** Structural History & Causation Tracking

---

## Purpose

`Dx.Domain.Facts` provides structural, meaning-agnostic primitives for tracking causation lineage and state transitions in domain systems.

### Key Principles

- **Structural, not semantic**: Facts capture *that* something happened and *how* it relates to other events, not *what* it means
- **Lineage-aware**: Facts carry causation metadata (correlation, parent/child relationships)
- **Not domain events**: Facts are infrastructure for tracking history; domain events are business-level semantics

---

## Core Types

### `Fact<TPayload>`

A structural container for a payload with causation metadata.

```csharp
public readonly struct Fact<TPayload>
{
    public TPayload Payload { get; }
    public Causation Cause { get; }
    public FactId Id { get; }
    public DateTimeOffset Timestamp { get; }
}
```

### `Causation`

Tracks correlation and lineage between facts.

```csharp
public readonly struct Causation
{
    public CorrelationId Correlation { get; }
    public FactId? Parent { get; }
    public FactId? Root { get; }
}
```

### `TransitionResult<TState>`

Represents the result of a state transition with invariant checking.

```csharp
public readonly struct TransitionResult<TState>
{
    public bool IsSuccess { get; }
    public TState? NewState { get; }
    public Fact<TState>? Fact { get; }
    public InvariantError? Error { get; }
}
```

---

## Dependencies

- **Dx.Domain.Kernel**: For Result types, error handling, and invariants
- **Dx.Domain.Primitives**: For identity types (FactId, CorrelationId, etc.)

---

## Usage Example

```csharp
using Dx.Domain.Facts;
using Dx.Domain.Primitives;

var causation = new Causation(
    correlation: CorrelationId.Create(),
    parent: null,
    root: null
);

var fact = new Fact<OrderCreated>(
    payload: new OrderCreated(orderId, customerId),
    cause: causation
);

// Facts are structural; business logic interprets them
var result = orderAggregate.ApplyFact(fact);
```

---

## Design Decisions (DPI-Justified)

**Why separate from Kernel?**

Originally, Facts were part of the kernel as "structural history primitives." Moving them to a separate package:

1. **Decouples InvariantError from Primitives**: Kernel's `InvariantError` needs to be usable without typed identity primitives, avoiding circular dependencies
2. **Minimizes Kernel surface**: Kernel focuses on Result algebra, invariants, and errors; Facts are a higher-level concern
3. **Preserves structural nature**: Facts remain meaning-agnostic and structural, just at a different architectural layer

This is a **deliberate scope change** from the original specification, justified by:
- Avoiding Kernel → Primitives dependency cycles
- Enabling InvariantError to use soft correlation (strings/Guids)
- Maintaining kernel purity (no ambient context, no typed identities)

---

## License

MIT License. See LICENSE file in repository root.
