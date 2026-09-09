# Architecture

Dx.Domain is a small, compiler-assisted substrate for explicit invariants, results, errors, identities, and structural facts.

## Packages

- **Annotations:** semantic metadata targeting .NET Standard 2.0.
- **Primitives:** immutable identity values targeting .NET 8, 9, and 10.
- **Kernel:** Results, errors, invariants, and requirements targeting .NET 8, 9, and 10.
- **Facts:** structural fact and causation values targeting .NET 8, 9, and 10.
- **Analyzers:** compile-time diagnostics targeting .NET Standard 2.0.

Facts depends on Primitives, Kernel, and Annotations. Kernel and Primitives depend on Annotations. Runtime projects reference the analyzer project as a compiler analyzer in repository builds; consumers install the analyzer package explicitly during alpha.

## Scope model

- **S0:** substrate assemblies owned by Dx.Domain.
- **S1:** consumer domain and construction boundaries.
- **S2:** application orchestration.
- **S3:** infrastructure and adapters.

Analyzer behavior is scope-aware. The exact reach of each rule is documented in the [diagnostic reference](reference/diagnostics/index.md).

## Boundaries

The Kernel contains mechanical correctness primitives, not repositories, persistence, dispatch, transport, or application workflow. Facts are structural records, not an event bus. Analyzers inspect source and symbols; they do not police runtime behavior.
