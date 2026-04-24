# Dx.Domain

Deterministic domain modeling for .NET. If it compiles, passes analyzers, and the Kernel accepts it, the state is valid.

**Enforcement scope:** Static analysis only. Applies to S1–S3. Not enforced at runtime. Bypassable via reflection, serialization, dynamic, and suppression.

## Start here (2 minutes)

1. [Overview](overview.md) — what it is and why
2. [Getting Started](getting-started.md) — install packages and write your first type
3. [Core Specification](specification/core-platform.md) — the normative rules

## Architecture

- [Architecture Overview](architecture-overview.md) — the four-package substrate
- [Packages](packages/index.md)
- [Annotations](packages/annotations.md)
- [Primitives](packages/primitives.md)
- [Kernel](packages/kernel.md)
- [Facts](packages/facts.md)
- [Analyzers](packages/analyzers.md)

## Analyzer Enforcement

All rules are enforced at compile time. See the complete reference:

- [DXA010 Construction Authority](packages/analyzers.md#dxa010)
- [DXA011 Public Factory](packages/analyzers.md#dxa011)
- [DXA020 Result Ignored](packages/analyzers.md#dxa020)
- [DXA022 No Throw in Result](packages/analyzers.md#dxa022)
- [DXA030 Unapproved Handler](packages/analyzers.md#dxa030)
- [DXA040 Kernel Freeze](packages/analyzers.md#dxa040)
- [DXA050 Temporal Helper](packages/analyzers.md#dxa050)
- [DXA060 Forbidden Vocabulary](packages/analyzers.md#dxa060)
- [DXA070 Generated Code Tag](packages/analyzers.md#dxa070)
- [DXA080 Facade Invariants](packages/analyzers.md#dxa080)

## Contract

All behavior is governed by the [Core Platform Specification](specification/core-platform.md) and enforced by analyzers. See [Analyzer Rules](packages/analyzers.md#key-diagnostics).

### Additional

- [README](readme.md)
- [CHANGELOG](changelog.md)
- [Examples](examples/index.md)
- [Release Notes](release-notes/index.md)
- [Changelog](changelog/index.md)
