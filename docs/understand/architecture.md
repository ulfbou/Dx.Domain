# Architecture

Dx.Domain is a small, compiler-assisted substrate for explicit invariants, results, errors, identities, and structural facts.

## Published packages

Dx.Domain publishes exactly four packages for consumer use:

- **Dx.Domain.Annotations:** semantic metadata targeting .NET Standard 2.0.
- **Dx.Domain.Primitives:** immutable identity values targeting .NET 8, 9, and 10.
- **Dx.Domain.Kernel:** Results, errors, invariants, and requirements targeting .NET 8, 9, and 10.
- **Dx.Domain.Facts:** structural fact and causation values targeting .NET 8, 9, and 10.

Dependencies flow as: Facts → (Primitives, Kernel, Annotations); Kernel, Primitives → Annotations.

Each published package carries the analyzer assembly (`analyzers/dotnet/cs/Dx.Domain.Analyzers.dll`). There is no separately published analyzer package. Consumers must not install a standalone analyzer package.

## Repository structure

The repository implements the analyzer in the `src/Dx.Domain.Analyzers` project. During repository builds, runtime projects reference this project as a compiler analyzer. During consumer package installation, the analyzer assembly is embedded in each published package and loaded automatically by NuGet.

## Scope model

- **S0:** substrate assemblies owned by Dx.Domain.
- **S1:** consumer domain and construction boundaries.
- **S2:** application orchestration.
- **S3:** infrastructure and adapters.

Analyzer behavior is scope-aware. The exact reach of each rule is documented in the [diagnostic reference](../reference/diagnostics/index.md).

## Boundaries

The Kernel contains mechanical correctness primitives, not repositories, persistence, dispatch, transport, or application workflow. Facts are structural records, not an event bus. Analyzers inspect source and symbols; they do not police runtime behavior.
