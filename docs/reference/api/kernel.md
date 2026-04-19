# Dx.Domain.Kernel

**Package**: `Dx.Domain.Kernel`  
**Role**: Runtime judge of invariants, results, errors, facts

## Purpose
The runtime **judge** of invariants, results, errors, and facts. No I/O/infrastructure. If it **compiles**, **passes analyzers**, and the **Kernel accepts it**, the state is valid.

## Core Types
- **Result<T>** - Discriminated union for success/failure. Never throw in Result-returning methods.
- **DomainError** - Structured error with code, message, and context. `DomainError.Create(code, message)`
- **Invariant** - Kernel-enforced preconditions
- **Unit** - Void equivalent for Result<Unit>

## Guarantees
- Frozen surface area - governed by DXA040
- No temporal helpers - DXA050
- No I/O, no infrastructure dependencies
- Deterministic behavior

## Exception Discipline
DXA022: No `throw` in Result methods. DXA020: Result must be handled. DXA030: Only approved handlers.

## Dependency Rules
- **Must** depend on Annotations
- **Must not** depend on Analyzers

See: [`public/packages/kernel.md`](../../public/packages/kernel.md)
