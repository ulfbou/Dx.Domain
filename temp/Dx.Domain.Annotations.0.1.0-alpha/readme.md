# Dx.Domain.Annotations

Semantic vocabulary for Dx.Domain.

## Purpose

Provide pure metadata markers that classify domain concepts. Attributes express intent without introducing runtime behavior.

## Guarantees

- Attributes impose no runtime semantics
- Classification is stable across minor versions within an alpha series
- Analyzers treat attributes as declarative intent for classification and rule evaluation

## Constraints

- No constructors with logic
- No dependencies on Dx.Domain.Kernel
- Must not alter program behavior
- Dx.Domain runtime packages do not depend on annotation discovery

## Alpha Limitations

- Attribute coverage by analyzers may vary
- Classification semantics may evolve during alpha
- No compatibility guarantees for attribute interpretation rules

## Role in System

- Consumed by Dx.Domain.Analyzers as part of classification where applicable
- Coverage varies by attribute
- Ignored by the runtime

## Public API Surface

### Domain Modeling

- `AggregateRootAttribute`
- `EntityAttribute`
- `ValueObjectAttribute`
- `DomainEventAttribute`
- `FactoryAttribute`
- `IdentityAttribute`
- `InvariantAttribute`
- `PolicyAttribute`

### Analyzer Classification

- `DxScopeAttribute`
- `DxLayerAttribute`
- `DxFacadeAttribute`
- `DxGeneratedAttribute`
- `DxTemplateAttribute`
- `DxAssemblyRoleAttribute`
- `DxApprovedHandlerAttribute`
- `ApprovedKernelApiAttribute`
- `DpiJustifiedAttribute`

### Supporting Types

- `Scope` (enum)
- `DxAssemblyRole` (enum)
- `DxCategories` (static)
- `DxRuleIds` (static)
- `DxSeverities` (static)

## Versioning

0.1.0-alpha — API surface subject to change. No compatibility guarantees.

## See Also

- [Dx.Domain.Analyzers](../Dx.Domain.Analyzers/readme.md) — Compile-time enforcement
- [Dx.Domain.Kernel](../Dx.Domain.Kernel/readme.md) — Runtime implementation
- [Architecture Decision Records](../../docs/adr/index.md) — Design rationale
