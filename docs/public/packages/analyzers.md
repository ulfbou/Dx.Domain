# Dx.Domain.Analyzers

**Purpose:** Compiler diagnostics for architectural and Result discipline.
**Release:** `0.1.0-alpha`
**Target:** .NET Standard 2.0 analyzer host

## Install

```bash
dotnet add package Dx.Domain.Analyzers --version 0.1.0-alpha
```

Install the analyzer package explicitly during alpha.

## Shipped diagnostics

DXA010, DXA011, DXA020, DXA022, DXA030, DXA040, DXA050, DXA060, DXA065, DXA070, and DXA080. See the [diagnostic reference](../reference/diagnostics/index.md).

## Constraints

Static analysis is local to participating compilations and can be bypassed through unsupported runtime paths or standard suppression mechanisms. Public behavior and configuration are provisional during alpha.

## Related material

- [Configure analyzers](../guides/configure-analyzers.md)
- [Architecture](../architecture.md)
- [Limitations](../limitations.md)
