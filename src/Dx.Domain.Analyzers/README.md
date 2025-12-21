# Dx.Domain.Analyzers

Production-grade Roslyn analyzers enforcing domain modeling principles for the Dx.Domain framework.

## Overview

This package provides compile-time analysis and enforcement of architectural patterns and best practices for domain-driven design using the Dx.Domain framework.

## Status

**Phase 0: Authority Substrate** - ✅ COMPLETE  
**Initial Analyzer Rules** - ✅ IMPLEMENTED (DXA010, DXA020, DXA022)

All infrastructure components are implemented and the solution builds successfully with zero warnings/errors.

## Implemented Analyzers

### DXA010: Construction Authority Violation
**Severity**: Warning  
**Scope**: S1 (Domain), S2 (Application), S3 (Consumer)

Ensures domain types are constructed only through the Dx facade, centralizing invariant enforcement and making object creation auditable.

**Detects**:
- Direct `new` construction of domain types
- Static factory method calls outside approved Dx facades
- Public creation methods bypassing the facade layer

**Exempts**:
- S0 (Kernel) code
- Type constructors (.cctor)
- Generated code

**Example Violation**:
```csharp
// ❌ Violation - direct construction
var id = new ActorId(Guid.NewGuid());

// ✅ Correct - use Dx facade
var id = Dx.CausationFactory.CreateActorId(Guid.NewGuid());
```

### DXA020: Result Ignored
**Severity**: Error  
**Scope**: S1 (Domain), S2 (Application)

Prevents silent failures by ensuring all `Result<T>` values are explicitly handled.

**Detects**:
- Result values produced but never used
- Expression statements that return Result
- Discarded Result instances

**Exempts**:
- S0 (Kernel) code
- Generated code

**Example Violation**:
```csharp
// ❌ Violation - result ignored
SomeMethodReturningResult();

// ✅ Correct - result handled
var result = SomeMethodReturningResult();
if (result.IsFailure) return result;

// ✅ Correct - result propagated
return SomeMethodReturningResult();
```

### DXA022: Discouraged Domain Control Exception
**Severity**: Warning  
**Scope**: S1 (Domain), S2 (Application)

Enforces explicit Result-based error handling instead of throwing exceptions for domain control flow.

**Detects**:
- Throwing exceptions in Result-returning methods
- Domain control exceptions (InvalidOperationException, custom domain exceptions)
- Exceptions used for business rule violations

**Allows**:
- ArgumentException for parameter validation
- InvariantViolationException for invariant failures
- Rethrows (throw;)

**Exempts**:
- S0 (Kernel) code
- Generated code

**Example Violation**:
```csharp
// ❌ Violation - throwing in Result-returning method
public Result<Order> ProcessOrder(OrderId id)
{
    if (!_orders.ContainsKey(id))
        throw new InvalidOperationException("Order not found");
    // ...
}

// ✅ Correct - return Result.Failure
public Result<Order> ProcessOrder(OrderId id)
{
    if (!_orders.ContainsKey(id))
        return Result.Failure<Order>(DomainError.Create("ORDER_NOT_FOUND", "Order not found"));
    // ...
}
```

## Infrastructure Components

### AnalyzerServices
Composition root providing all analyzer dependencies:
- ScopeResolver
- DxFacadeResolver  
- SemanticClassifier
- ExceptionIntentClassifier
- ResultFlowEngineWrapper
- GeneratedCodeDetector

### ScopeResolver
Resolves assemblies and symbols to scopes (S0-S3) based on EditorConfig:
- **S0**: Kernel - Trusted, minimal rules
- **S1**: Domain - Strict invariant enforcement
- **S2**: Application - Business logic rules
- **S3**: Consumer - Construction authority enforced

### DxFacadeResolver
Discovers and validates Dx facade factory methods, ensuring only approved construction patterns are used.

### SemanticClassifier
Classifies domain types (Results, Errors, Exceptions) for accurate analysis.

### ExceptionIntentClassifier
Determines exception intent:
- **ArgumentValidation**: Parameter guard clauses (allowed)
- **InvariantViolation**: Invariant checks (allowed)
- **DomainControl**: Business rule violations (discouraged in Result-returning methods)
- **Infrastructure**: System/IO errors (allowed)

### ResultFlowEngine
Control flow graph analysis for tracking Result lifecycle (Created, Checked, Propagated, Terminated, Ignored).

## Configuration

Add to your `.editorconfig`:

```ini
[*.cs]

# Scope mapping (required for scope-aware rules)
dx.scope.map = Dx.Domain=S0;MyApp.Domain=S1;MyApp.Application=S2;MyApp=S3
dx.scope.rootNamespaces = MyApp

# Generated code markers (optional)
dx_generated_markers = Generated;__generated

# Analyzer severities (optional overrides)
dotnet_diagnostic.DXA010.severity = warning
dotnet_diagnostic.DXA020.severity = error
dotnet_diagnostic.DXA022.severity = warning
```

## Scope Behavior

| Scope | Description | DXA010 | DXA020 | DXA022 |
|-------|-------------|--------|--------|--------|
| **S0** | Kernel (trusted) | Exempt | Exempt | Exempt |
| **S1** | Domain | Enforced | Enforced | Enforced |
| **S2** | Application | Enforced | Enforced | Enforced |
| **S3** | Consumer | Enforced | - | - |

## Build Requirements

- **.NET SDK**: 10.0 or later (project uses net10.0)
- **Analyzer Target**: netstandard2.0 (required for Roslyn analyzers)
- **Roslyn**: Microsoft.CodeAnalysis.CSharp 4.12.0

## Building

```bash
dotnet build Dx.Domain.sln
```

## Design Principles

1. **Fail-Open**: Analyzers never break builds on infrastructure failures
2. **Scope-Aware**: Rules adapt based on code location (kernel vs consumer)
3. **Performance**: <5ms per method analysis budget
4. **Determinism**: Identical inputs produce identical diagnostics
5. **Generated Code Exemption**: Respects [GeneratedCode] and namespace markers

## What's Not Included (Yet)

The following are documented but not yet implemented:
- Code fix providers
- Unit and integration tests
- EditorConfig validation tests
- Facade surface reflection tests
- Migration guides

See `docs/internal/implementation-status.md` for detailed tracking.

## References

- [ROADMAP.md](../../docs/ROADMAP.md) - Development phases
- [rules.md](../../docs/internal/rules.md) - Complete rule specifications
- [canonical-scope-model.md](../../docs/internal/canonical-scope-model.md) - Scope definitions
- [analyzer-contracts.md](../../docs/internal/analyzer-contracts.md) - Acceptance criteria
- [implementation-status.md](../../docs/internal/implementation-status.md) - Current status

## License

MIT License - See [LICENSE](../../LICENSE) for details
