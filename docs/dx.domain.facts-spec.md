
# Dx.Domain.Facts — Specification

**Status**: Draft → Candidate for `0.1.0-alpha.5`  
**Audience**: Domain/Kernel engineers, library contributors, analyzer/generator authors  
**Scope**: Structural “facts” (immutable domain statements) and composition utilities for state transitions

***

## 1. Purpose & Rationale

**Dx.Domain.Facts** provides the structural core for representing and composing **facts**—immutable, lineage‑aware statements about domain state—together with the **causation** metadata that explains *why* a fact exists. It also offers a **transition result** type that couples the outcome of a domain operation with the set of facts it emitted, enabling safe, functional composition. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

This package **does not** define transports, storage formats, dispatching, or business semantics. It is **purely structural** and intended to be embedded in domain logic and its tests. Transport and persistence packages can build upon this foundation.

***

## 2. Non‑Goals

*   **No transports or formats** (JSON/Avro/Protobuf/etc.). Serialization belongs in integration packages.
*   **No side effects** (no logging, IO, time providers, or ambient context).
*   **No business rules**. Domain policies and event semantics live outside.
*   **No framework hosting concerns** (e.g., DI, ASP.NET, message buses).

***

## 3. Design Tenets

1.  **Immutability & Value Semantics**: Facts and causation are immutable value types; equality is well‑defined and cheap to compute. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
2.  **UTC Discipline**: All timestamps are UTC. The Kernel’s `Invariant` helpers enforce structural constraints, not policy. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)
3.  **Purity & Composability**: Transition results compose via functional operators (map/bind), enabling readable, testable domain pipelines. (This spec introduces compositional APIs on `TransitionResult<T>`; see §8). [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
4.  **Observability‑Ready**: Causation carries correlation, tracing, and actor identity to make emitted history explainable across boundaries. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
5.  **Transport‑Agnostic Contracts**: Public APIs reveal the *shape* of facts and their metadata; envelopes and type taxonomy are defined without binding to any codec or schema system.

***

## 4. Canonical Concepts & Glossary

*   **Fact**: An immutable statement with a type name, strongly‑typed payload, unique `FactId`, causation, and UTC timestamp. Not a “domain event” in the business sense; it’s a structural record. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   **Causation**: Correlation (`CorrelationId`), distributed trace (`TraceId`), actor (`UserId`), and recording time—capturing *why/when/by whom* a fact exists. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   **Transition Result**: The outcome of a domain state transition (`Result<TState>`) plus the set of emitted facts; used to persist new state and append facts in one pass. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   **Kernel Result**: Functional result type (`Result<T>` / `Result<T, TError>`) with compositional operators used throughout the ecosystem. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)

***

## 5. Public API Surface (Normative)

> **Note**: Namespaces and summary comments below describe the **normative contract**. Implementations may include additional debugger displays or attributes that do not change semantics.

### 5.1 `Causation` (struct)

**Namespace**: `Dx.Domain.Facts`

*   **Members**
    *   `CorrelationId CorrelationId` — May be empty only if explicitly permitted by the caller’s policy (kernel invariant checks can be introduced where needed). [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `TraceId TraceId` — Must not be empty for traced flows. An invariant guard exists in the constructor factory. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `UserId ActorId` — `default` if unknown; callers may treat “unknown” vs “anonymous” policy outside Facts. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `DateTimeOffset UtcTimestamp` — When the causation record was created (UTC). [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   **Factory**
    *   `static Causation Create(CorrelationId correlationId, TraceId traceId, UserId? actorId = null)` — Enforces non‑empty `TraceId` and non‑empty `CorrelationId` (when policy requires), stamps UTC now. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   **Equality**
    *   Value equality currently includes timestamp. Consumers SHOULD treat timestamp as context, not identity. (See §9 for an evolution note). [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

### 5.2 `IDomainFact` (interface)

**Namespace**: `Dx.Domain.Facts`

*   **Members**
    *   `FactId Id` — Unique identifier. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `string FactType` — Logical type/category; versioning guidance in §10. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `Causation Causation` — Why the fact exists. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `DateTimeOffset UtcTimestamp` — When the fact occurred. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `object GetPayload()` — Structural payload access for non‑generic consumers. (See §7 for ergonomic extensions.) [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

### 5.3 `Fact<TPayload>` (struct)

**Namespace**: `Dx.Domain.Facts`

*   **Constraints**: `where TPayload : notnull`
*   **Members**
    *   `FactId Id` — Created with `FactId.New()` at factory time. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `string FactType` — Non‑null/non‑whitespace. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `TPayload Payload` — Strongly typed, non‑null payload. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `Causation Causation` — See §5.1. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `DateTimeOffset UtcTimestamp` — Defaults to `UtcNow` if null. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `object GetPayload()` — Explicit interface bridging. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   **Factories**
    *   `static Fact<TPayload> Create(string factType, TPayload payload, Causation causation, DateTimeOffset? utcTimestamp = null)` — Guards fact type and payload; assigns identifiers and time. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   **(Optional, additive)** `static Result<Fact<TPayload>> TryCreate(...)` — Non‑throwing counterpart aligned with Kernel result flows (recommended; see §8). [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)

### 5.4 `TransitionResult<TState>` (struct)

**Namespace**: `Dx.Domain.Facts`

*   **Members**
    *   `Result<TState> Outcome` — Success/failure of the transition. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   `IReadOnlyList<IDomainFact> Facts` — Emitted facts (see §8 for `ImmutableArray` evolution). [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
    *   Convenience flags: `IsSuccess`, `IsFailure`. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   **Factories**
    *   `static TransitionResult<TState> Success(TState state, IReadOnlyList<IDomainFact> facts)`
    *   `static TransitionResult<TState> Success(TState state, IDomainFact fact)`
    *   `static TransitionResult<TState> Failure(DomainError error)` [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   **Deconstruction**
    *   Multiple deconstruct overloads for ergonomic pattern use. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

***

## 6. Behavioral Guarantees (Normative)

1.  **Immutability**: All published types in this package are immutable; no publicly settable state exists after construction. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
2.  **UTC**: All timestamps are in UTC; violations should use Kernel invariants or guarded factories. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)
3.  **Null Safety**: Generic payloads prohibit `null`; APIs guard against `null` or whitespace where applicable. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
4.  **Thread Safety**: Value types and readonly semantics render instances thread‑safe by construction.
5.  **No Ambient Context**: Facts and results capture data explicitly (causation), never via static context. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)

***

## 7. Ergonomics for Consumers (Recommended, Additive)

To make `IDomainFact` friendly without reflection:

*   Introduce `IDomainFact<TPayload> : IDomainFact` where `new TPayload GetPayload()`.
*   Provide `DomainFactExtensions.TryGetPayload<T>(this IDomainFact fact, out T? payload)` for safe pattern matching across mixed collections.

These are **transport‑agnostic** and preserve the current contract. (This spec allows adding these interfaces and extensions in a minor alpha increment.) [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

***

## 8. Composition Model: Transition as a Writer over Result (Recommended, Additive)

`TransitionResult<T>` already couples a `Result<T>` with a list of facts. This spec **elevates it to a first‑class Writer‑style combinator** with LINQ support:

*   **Map**: `(T → U)` transforms state, carries facts unchanged.
*   **Bind**: `(T → TransitionResult<U>)` composes transitions and **concatenates facts**.
*   **LINQ**: `Select`, `SelectMany` sugar over Map/Bind for idiomatic C# usage.

**API (additive, normative contract):**

```csharp
public readonly struct TransitionResult<T> where T : notnull
{
    public TransitionResult<U> Map<U>(Func<T, U> f) where U : notnull;        // pure map
    public TransitionResult<U> Bind<U>(Func<T, TransitionResult<U>> f) where U : notnull; // compose + accumulate facts

    // LINQ
    public TransitionResult<U> Select<U>(Func<T, U> f) where U : notnull;
    public TransitionResult<V> SelectMany<U, V>(Func<T, TransitionResult<U>> bind, Func<T, U, V> project)
        where U : notnull where V : notnull;
}
```

**Fact accumulation** SHOULD be implemented with **`ImmutableArray<IDomainFact>`** internally and exposed as `IReadOnlyList<IDomainFact>` or directly as `ImmutableArray` (preferred) to reflect append‑only semantics and reduce allocations. This is a transport‑agnostic, purely structural optimization. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

**Rationale**: Harmonizes with Kernel `Result` composability and makes aggregate code concise and testable without introducing side effects. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)

***

## 9. Equality, Identity, and Ordering

*   **Facts**: Identity is `FactId`; structural properties are informative.
*   **Causation**: Current equality includes `UtcTimestamp`, which may cause “same context, different instant” to compare unequal.
    *   **Guidance**: Consumers SHOULD treat `{CorrelationId, TraceId, ActorId}` as the causal identity and the timestamp as context.
    *   **Potential evolution**: Consider revising equality to omit `UtcTimestamp` in a future minor release; keep full state in `GetHashCode` only if required for dispersion. (Non‑breaking if introduced with care; discuss in CHANGELOG.) [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

***

## 10. Fact Typing & Versioning (Transport‑Agnostic)

*   **FactType** is a stable, machine‑consumable **string** (e.g., `"order.placed"`). Avoid human phrases. **Do not reuse** codes for different meanings. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   **Versioning**: Evolve payload schemas by **incrementing a version** associated with the fact type (outside this package).
    *   This package MAY introduce a lightweight `readonly record struct FactType(string Code, int Version)` or static helpers (e.g., `FactTypeOf<TPayload>.Code/Version`) **without** committing to any serializer.
    *   Envelopes and registries live in integration packages; see §12.

***

## 11. Errors & Invariants

*   Use Kernel’s `Invariant` to guard structural constraints (e.g., non‑empty trace ids). Violations throw `InvariantViolationException` with rich diagnostics. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)
*   Use Kernel `Result` for recoverable flows; provide `TryCreate` style factories for non‑throwing composition (additive). [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)
*   Error codes should follow the Kernel convention (`dx.kernel.*` for kernel errors, `dx.facts.*` for facts‑package codes). A central `DxK.Codes` exists for kernel errors; facts can add their own codes under a distinct prefix in this package. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)

***

## 12. Envelopes, Persistence & Outbox (Informative)

Although this package remains transport‑agnostic, **downstream** persistence/transport packages commonly define a **FactEnvelope** that flattens:

    (Id, FactType, Version, Causation, UtcTimestamp, Payload)

This package MUST NOT ship serializers, but MAY provide **interfaces** or **pure data shapes** that external packages reference (e.g., an internal `IFactEnvelopeProjector`) as long as they do not introduce codec dependencies. (Outbox, partitioning hints, and idempotency keys are integration concerns and out of scope here.)

***

## 13. Usage Patterns

### 13.1 Emitting Facts in Aggregates

```csharp
var causation = Causation.Create(correlationId, traceId, actorId);
var fact = Fact<OrderSubmitted>.Create("order.submitted", payload, causation);
// persist state + fact(s) together via TransitionResult
```

The example reflects the current factory contracts. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

### 13.2 Composing Transitions (after §8 additions)

```csharp
TransitionResult<OrderState> PlaceOrder(OrderState s, NewOrder cmd)
{
    return TransitionResult.Return(s)                   // helper returning Success(s, [])
        .Bind(Validate)                                 // Result<T> + no facts
        .Bind(ReserveInventory)                         // emits Fact<InventoryReserved>
        .Bind(ChargePayment)                            // emits Fact<PaymentCaptured>
        .Map(st => st with { Status = OrderStatus.Paid });
}
```

### 13.3 Consuming Facts Safely

```csharp
foreach (var f in result.Facts)
{
    if (f.TryGetPayload<PaymentCaptured>(out var p))
    {
        // react to specific payload type without casts or transports
    }
}
```

***

## 14. Performance & Memory

*   Prefer **`ImmutableArray<IDomainFact>`** for facts accumulation (append/concat costs are predictable and allocation‑friendly). Expose as `ImmutableArray` or `IReadOnlyList` for compatibility. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   Keep `Fact<T>` and `Causation` as **readonly structs** to minimize allocations and improve locality. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   Do not capture ambient state; pass causation explicitly to enable pooling and reuse in higher layers.

***

## 15. Testing Guidance

*   Unit‑test transitions by asserting both `Outcome` and `Facts`:
    *   On success: check resulting state and exact sequence of emitted facts.
    *   On failure: assert `Outcome.Error` and that `Facts` is empty. (This is guaranteed by the current `Failure` factory.) [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   Use Kernel `Result` test helpers to validate composition behavior (`Map`, `Bind`, `Ensure`). [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)

***

## 16. Security & Privacy

*   Payloads may contain personal or sensitive data. This package **does not** inspect or redact payloads.
*   Downstream packages should implement redaction in transports and sinks.
*   Causation may include actor identity; treat with least‑privilege access in consumers. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

***

## 17. Backwards Compatibility & Evolution

*   **Additive APIs** (e.g., `TryCreate`, `Map/Bind`, `IDomainFact<T>`) are safe in alpha.
*   **Equality semantics of `Causation`**: If revised to ignore `UtcTimestamp`, document in CHANGELOG with migration guidance. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   **`ImmutableArray` exposure**: Moving from `IReadOnlyList` to `ImmutableArray` is a **source‑compatible** enhancement; binary compatibility depends on public signature changes. Provide dual properties for one alpha if needed. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

***

## 18. Reference Implementation Notes (Informative)

*   Use Kernel `Invariant.That(...)` for guards; throw `InvariantViolationException` with Kernel’s diagnostic capture on failure. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)
*   Reuse Kernel `Result` factories for non‑throwing flows in builder helpers and `TryCreate`. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)
*   Keep Debugger Displays informative and consistent across types to ease debugging; current files provide a good baseline. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)

***

## 19. Minimal API Checklist for This Package

*   [x] `Causation` immutable struct with guarded `Create` factory. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   [x] `IDomainFact` interface + `Fact<TPayload>` struct with guarded `Create`. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   [x] `TransitionResult<TState>` with success/failure factories and deconstructors. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.facts.md)
*   [x] `TransitionResult<T>.Map/Bind` + LINQ methods (no side effects).
*   [x] Represent facts internally as `ImmutableArray<IDomainFact>`.
*   [x] `TryCreate` factories aligned with Kernel `Result`. [\[newtonkomp...epoint.com\]](https://newtonkompetensutveck-my.sharepoint.com/personal/ulf_bourelius_edu_newton_se/Documents/Microsoft%20Copilot%20Chat%20Files/dx.kernel.md)
*   [x] `IDomainFact<T>` + `TryGetPayload<T>` extension for safe consumption.

***

## 20. Appendix — Host & Integration Guidance (Non‑Normative)

*   **Persistence/Outbox**: Persist `Outcome.Value` (state) and `Facts` atomically using your store’s transaction/outbox model.
*   **Tracing**: If a host uses `Activity`/OpenTelemetry, build a tiny adapter outside this package that populates `Causation.Create(...)` from the active activity (trace/span) and request context (correlation/actor).
*   **Type Registry**: Keep fact type codes and versions in a separate catalog (e.g., attributes + codegen) so transport packages can bind to the same registry without coupling back to this package.

***

### End of Specification
