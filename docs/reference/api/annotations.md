# Dx.Domain.Annotations

**Package**: `Dx.Domain.Annotations`  
**Role**: Semantic vocabulary and contracts

## Purpose
Pure vocabulary and metadata for marking domain types. **No runtime logic.**

Annotations provide the semantic contracts that analyzers enforce and the Kernel trusts. They define scope, authority, and structural roles without implementing behavior.

## Key Attributes
- **Scope** - S0 Kernel, S1 Domain Facades, S2 Application, S3 Infrastructure/Consumer
- **DxAssemblyRole** - Declares package role in the substrate
- **AggregateRootAttribute** - Marks aggregate roots
- **ValueObjectAttribute** - Marks immutable value objects

## Dependency Rules
- Kernel **must** depend on Annotations
- Analyzers depend on Annotations; never on Kernel at runtime
- All packages may reference Annotations

## Guarantees
- Zero runtime overhead
- Compile-time metadata only
- Analyzer-enforced usage patterns

See: [`public/packages/annotations.md`](../../public/packages/annotations.md)
