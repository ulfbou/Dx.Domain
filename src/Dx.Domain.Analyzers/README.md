# Dx.Domain.Analyzers

Production-grade Roslyn analyzers enforcing domain modeling principles for the Dx.Domain framework.

## Overview

This package provides compile-time analysis and enforcement of architectural patterns and best practices for domain-driven design using the Dx.Domain framework. Analyzer scope and authority are layer-aware (Kernel/Primitives/Annotations/Consumer).

**Authority modes**:
- **Definitional** (Kernel)
- **Structural** (Primitives/Annotations)
- **Constraining** (Consumer)
- **Observational** (Kernel-aware, non-restrictive)

## Status

**Phase 0: Authority Substrate** - ✅ COMPLETE  
**Initial Analyzer Rules** - ✅ IMPLEMENTED (DXA010, DXA020, DXA022)

All infrastructure components are implemented and the solution builds successfully with zero warnings/errors.

## Implemented Analyzers

### DXA010: Construction Authority Violation
**Severity**: Warning  
**Scope**: Consumer only (S3)
**Authority**: Constraining

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
**Scope**: Consumer only (S3)
**Authority**: Constraining

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
**Scope**: Consumer only (S3)
**Authority**: Constraining

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

### DXA060: Forbidden Vocabulary
**Severity**: Error  
**Scope**: Consumer only (S3)  
**Authority**: Constraining

Detects forbidden architectural vocabulary in consumer code. The allow-list can be configured via:

```
dx_forbidden_vocab_allow = Namespace.TypeName;Other.Namespace.*
```

### DXT004: DXT Invariants Required
**Severity**: Error  
**Scope**: Consumer solutions (including tests)
**Authority**: Constraining

Ensures consumer solutions include the required `.dx/invariants.json` file at the solution root.

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
Resolves assemblies and symbols to scopes (S0-S3) based on build properties and assembly metadata:
- **S0**: Authority (Kernel/Primitives/Annotations) - Trusted, minimal rules
- **S3**: Consumer - Construction authority enforced

Scopes S1/S2 are reserved for future domain/application specialization.

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

# Scope and role signals (MSBuild or templates)
build_property.DxLayer = Consumer
build_property.DxResolvedRole = Domain

# Kernel API freeze (authority-only, opt-in)
build_property.DxKernelApiFreeze = true

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
| **S0** | Authority (trusted) | Exempt | Exempt | Exempt |
| **S3** | Consumer | Enforced | Enforced | Enforced |

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
