# Dx.Domain in 90 seconds

1. Purpose: Dx.Domain provides a small mechanical kernel for explicit results, domain errors, and invariants with compile-time enforcement.
2. When to use:
   - You want failures represented as values via Result<T>
   - You want analyzers to prevent ignored results and unauthorized construction in S1–S3
   - You target .NET 8, 9, or 10
3. When NOT to use:
   - You need a full DDD framework with repositories or sagas in the kernel
   - You prefer exceptions for normal control flow
4. Guarantees:
   - Result<T> must be handled explicitly
   - Construction of domain types in S1–S3 flows through the Dx facade
   - S0 Kernel types (Result, DomainError) are publicly constructible per ADR-0018
   - Scope-aware enforcement varies by S0 to S3
5. Constraints:
   - Kernel surface is frozen
   - Analyzers must be enabled in the build

## What it enforces

- Explicit Result usage. Ignored Result<T> fails the build via DXA020.
- Analyzer-driven correctness. Construction authority is enforced via DXA010 and DXA011 in S1–S3 only.
- Scope awareness. S0 is exempt from construction rules.

## Package map

- Dx.Domain.Kernel: Result<T>, DomainError, Invariant, Unit
- Dx.Domain.Primitives: CorrelationId, TraceId, ActorId, FactId, SpanId, UserId
- Dx.Domain.Facts: Fact<TPayload>, Causation, TransitionResult<TState>
- Dx.Domain.Annotations: Scope, DxAssemblyRole, AggregateRootAttribute, ValueObjectAttribute
- Dx.Domain.Analyzers: Roslyn analyzers DXA010–DXA080

## Start here

1. [Getting Started](getting-started.md): install packages and enable analyzers
2. [Quickstart](quickstart.md): one cohesive scenario with Result, DomainError, a primitive, and a facade
3. [What the analyzers enforce](../analyzers/dxa010.md): read DXA010 and DXA020 first

## Related links

- Up: [Public Overview](../public/overview.md)
- Concept: [Results](concepts/results.md)
- Governance: [ADR-0018: Kernel Public Surface](../adr/adr-0018-kernel-public-surface.md)
- Architecture: [Architecture Overview](../public/architecture-overview.md)

### Additional resources

- [CHANGELOG](changelog.md)
- [ENFORCEMENT_MAP](enforcement_map.md)
- [STABILITY](stability.md)
- [OVERVIEW](overview.md)
- [Architecture Overview](architecture-overview.md)

- [Release Notes](release-notes/index.md)
- [Changelog](changelog/index.md)


## Concepts

- [Errors](concepts/errors.md)
- [Facades](concepts/facades.md)
- [Facts vs Events](concepts/facts-vs-events.md)
- [Generated Code](concepts/generated-code.md)
- [Invariants](concepts/invariants.md)
- [Results](concepts/results.md)
- [Scopes and Roles](concepts/scopes-and-roles.md)

## Guides

- [Build a Facade](guides/build-a-facade.md)
- [Define a Primitive](guides/define-a-primitive.md)
- [Emit Facts](guides/emit-facts.md)
- [Exceptions Policy](guides/exceptions-policy.md)
- [Handle Errors](guides/handle-errors.md)
- [Result Handling Patterns](guides/result-handling-patterns.md)

