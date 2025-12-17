# Dx.Domain

Compiler-assisted domain safety for .NET.

Dx.Domain is a small, opinionated kernel for expressing invariants, results, and structured domain errors.
It exists to make incorrect domain modeling impossible to ignore – not to provide general-purpose helpers.

> Time, causation, invariants, results, values — this is the spine. Everything else lives outside or on top.

---

## Core Concepts

- `DomainError` – canonical, boundary-safe error value for domain failures.
- `Result<TValue>` / `Result<TValue, TError>` – explicit success/failure results with typed errors.
- `Invariant` – guard-style APIs for enforcing executable invariants.
- Invariant diagnostics – `InvariantError` and `InvariantViolationException` explain why the program is wrong
  without leaking diagnostics into domain values.

The kernel is intentionally small and frozen; evolution happens in analyzers, generators, and adapters that
push correctness pressure toward the edges.

### Value Types and Factories

Value types in the kernel may expose static factory methods for safe, efficient construction — including randomness or time capture — provided the resulting value remains immutable and semantically atomic.

This protects random ID generation, time capture, and optimized memory usage without opening the door to builders or mutability.

### Identifiers and Semantic Strings

While the kernel demands strong typing, it recognizes that certain identifiers must be serializable and human-readable to cross system boundaries.

**Canonical identifiers (such as error codes and fact types) are permitted as strings provided they are:**

1. **Centralized** – Defined in a single location (for example, a `Faults` or `FactTypes` static class).
2. **Intentional** – Represent permanent semantic contracts, not transient or local values.
3. **Validated** – Subject to internal invariants (such as `NotNullOrWhiteSpace`) and, where applicable, structural rules.

These strings are treated as *identities*, not configuration knobs. By contrast, ad-hoc literals used directly in control flow (for example, `if (role == "admin")`) remain violations of the "No Magic Strings" rule and should be modeled via dedicated types or enums.

---

## Philosophy

Dx.Domain is defined as much by what it refuses as by what it provides:

- Not a convenience library or kitchen-sink utility layer.
- Not a DDD pattern museum (no generic repositories, services, or aggregate frameworks).
- Not a persistence or transport framework; those live as adapters on top.
- Not a runtime-first safety net; correctness is primarily compile-time and invariant-driven.
- The kernel forbids semantic expansion, not mechanical support.

The design favors friction that teaches. Misuse should be mechanically hard or impossible. Correct usage should
be obvious from the types.

For the full rationale, see:

- [ `docs/MANIFESTO.md`](./docs/MANIFESTO.md) – the core refusals and demands.
- [`docs/NON_GOALS.md` ](./docs/NON_GOALS.md) – what Dx.Domain will never become.
- [`docs/DPI.md`](./docs/DPI.md) – the Design Pressure Index that governs whether a change belongs in the core.

---

## Packages

The repository is organized as a small core plus edge packages:

- `Dx.Domain` – core invariants, results, errors, time/causation primitives.
- `Dx.Domain.Analyzers` – Roslyn analyzers that enforce idioms and migrate legacy code toward the model.
- `Dx.Domain.Generators` – source generators that remove boilerplate without weakening invariants.
- `Dx.Domain.Persistence.*` – persistence and transport adapters that respect, but do not redefine, the core.

---

## Using Dx.Domain (sketch)

```csharp
using Dx.Domain;

public static Result<OrderId> CreateOrder(string? customerId)
{
    Invariant.NotNullOrWhiteSpace(
        customerId,
        DomainError.Create("order.customer.missing", "Customer id is required."));

    var id = OrderId.New();
    return Result.Ok(id);
}
```

Results and domain errors are designed to cross boundaries (APIs, persistence, tests). Invariant diagnostics are not.

---

## Documentation

- Manifesto & rationale: `docs/MANIFESTO.md`
- Non-goals & scope guardrails: `docs/NON_GOALS.md`
- Design Pressure Index (how changes are judged): `docs/DPI.md`
- API reference: generated from the `Dx.Domain` assemblies (TBD).

---

## Contributing

Contributions are welcome, but every change must pass the Design Pressure Index:

- Enforces a domain invariant that cannot be enforced elsewhere.
- Reduces accidental complexity or misuse.
- Increases what the compiler can prove.
- Aligns with the Manifesto and violates none of the Non-Goals.
- Adds at most mechanical support; never semantic expansion.

> Static analysis and code review must distinguish between semantic expansion and mechanical enforcement. Internal helpers that do not add new vocabulary must not be flagged.

If a feature does not belong in the core, it may fit as an analyzer, generator, or adapter – or outside this repo entirely.

---

## License

Licensed under the MIT License. See [`LICENSE`](./LICENSE) for details.
