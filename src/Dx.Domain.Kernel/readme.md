# Dx.Domain.Kernel

Functional core types for Dx.Domain.

## Purpose

Provide result types, error representations, and invariant enforcement for domain operations without using exceptions for control flow.

## Guarantees

- Public types expose immutable state only
- Result types model success and failure explicitly
- No ambient context, no static mutable state
- All operations are pure, deterministic, and thread-safe

## Constraints

- No external dependencies beyond .NET runtime
- No logging, IO, retry logic, or policy decisions
- Invariant violations throw InvariantViolationException; recoverable validation returns Result
- Public surface is intentionally minimal

## Alpha Limitations

- API surface subject to change during alpha
- No compatibility guarantees prior to 0.1.0 stable
- Analyzer enforcement for kernel rules is partial (see Analyzers documentation)
- Breaking changes are unlikely but possible

## Role in System

- Foundational layer consumed by Primitives, Facts, and domain layers
- Provides error model and Result-based control flow
- No governance or enforcement role; enforcement is provided by Dx.Domain.Analyzers

## Public API Surface

### Result Types

- `Result<TValue, TError>` — Discriminated union for operations
- `Result<TValue>` — Result with DomainError as error type
- `Result` — Static factories: Success, Failure

### Validation

- `Dx.Require.That(condition, error)` — Recoverable validation returning Result<Unit>
- `Dx.Require.That(condition, value, error)` — Validation returning Result<TValue>
- Lazy overloads accepting Func<DomainError> for allocation-free success paths

### Errors

- `DomainError` — Stable code, message, and metadata
- `InvariantError` — Diagnostic context for invariant violations (member, file, line, timestamp)
- `InvariantViolationException` — Exception wrapper for structural invariant failures

### Supporting Types

- `DomainTime` — UTC timestamp with zero offset enforcement
- `Unit` — Void result type for operations with no value

### Combinators

Available as extension methods on Result:
- Map, Bind, Match, Tap, Ensure
- MapError, Recover
- Try, TryAsync for exception-to-Result translation

## Versioning

0.1.0-alpha — API surface subject to change. No compatibility guarantees.

## See Also

- [Dx.Domain.Primitives](../Dx.Domain.Primitives/readme.md) — Core value types
- [Dx.Domain.Facts](../Dx.Domain.Facts/readme.md) — Immutable facts with causation
- [Dx.Domain.Annotations](../Dx.Domain.Annotations/readme.md) — Metadata vocabulary
- [Architecture Decision Records](../../docs/adr/index.md) — Design rationale
