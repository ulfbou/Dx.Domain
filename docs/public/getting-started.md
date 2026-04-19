# Getting Started

## 1. Install packages

```bash
dotnet add package Dx.Domain.Primitives
dotnet add package Dx.Domain.Kernel
dotnet add package Dx.Domain.Annotations
dotnet add package Dx.Domain.Facts
dotnet add package Dx.Domain.Analyzers
```

Package roles: [Primitives](packages/primitives.md), [Kernel](packages/kernel.md), [Annotations](packages/annotations.md), [Facts](packages/facts.md).

## 2. Understand the three constraints

Before writing code, read [Core Specification §4-5](specification/core-platform.md#4-construction-authority):

- **Construction is restricted** — see [DXA010](packages/analyzers.md#dxa010)
- **All operations return Result** — see [Result Semantics](specification/core-platform.md#5-result-semantics)
- **Exceptions are not control flow** — see [DXA022](packages/analyzers.md#dxa022)

## 3. Write your first domain type

Use value objects from [Primitives](packages/primitives.md) and annotate with [Annotations](packages/annotations.md).

Example: [basic-result.md](examples/basic-result.md)

## 4. Compile

Analyzers enforce correctness. You will see these first:

- [DXA010](packages/analyzers.md#dxa010) – if you use `new` instead of Dx facade
- [DXA011](packages/analyzers.md#dxa011) – if you leave a constructor public
- [DXA020](packages/analyzers.md#dxa020) – if you ignore a Result (Error)
- [DXA022](packages/analyzers.md#dxa022) – if you throw in a Result method
- [DXA030](packages/analyzers.md#dxa030) – if you pass Result to an unregistered handler

Full list: [Analyzer Reference](packages/analyzers.md)

**Next:** [Architecture Overview](architecture-overview.md) → [Specification](specification/core-platform.md)
