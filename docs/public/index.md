# Dx.Domain

Dx.Domain is a small, compiler-assisted substrate for explicit invariants, results, errors, identities, and structural facts.

> **Release status:** `0.1.0-alpha`. APIs and analyzer behavior are provisional.

## Start in five minutes

1. [Install and configure Dx.Domain](getting-started.md).
2. [Run the quickstart](quickstart.md).
3. Choose packages using the [package guide](packages/index.md).
4. Use the [diagnostic reference](reference/diagnostics/index.md) when the compiler reports a DXA rule.

## How correctness is enforced

- **Compiler-enforced:** static typing, readonly value types, and API visibility.
- **Analyzer-enforced:** local, statically visible architectural patterns in participating compilations.
- **Runtime-enforced:** checks such as non-empty identifiers and explicit invariants.
- **Not guaranteed:** business correctness, reflection, serialization materialization, dynamic invocation, and unchecked assemblies.

Read [Architecture](architecture.md), [Stability](stability.md), and [Limitations](limitations.md) before adopting the alpha.

## Reference

- [Packages](packages/index.md)
- [Concepts](concepts/index.md)
- [Guides](guides/index.md)
- [Configuration](reference/configuration.md)
- [Diagnostics](reference/diagnostics/index.md)
- [API reference](reference/api/index.md)
- [Release notes](release-notes/index.md)
- [Security](security.md)
