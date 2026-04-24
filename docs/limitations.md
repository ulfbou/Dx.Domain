# Limitations

## What enforcement does not provide

Enforcement does not mean:
- runtime prevention
- semantic correctness of business logic
- completeness across assemblies
- resistance to intentional suppression

## Scope boundary

All analyzers operate exclusively on:
- statically analyzable code paths
- within the current compilation unit
- with analyzers enabled

They do not analyze:
- reflection
- serialization
- dynamic invocation
- code in assemblies built without analyzers

*Source: docs/specifications/dx.domain-enforcement-specification.md*
