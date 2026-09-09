# Dx.Domain

Dx.Domain is a small, compiler-assisted substrate for explicit invariants, results, errors, identities, and structural facts in .NET.

> **Alpha:** `0.1.0-alpha` is pre-release software. Principles are stable, but APIs and analyzer behavior may change before a stable release. Review the [stability policy](docs/public/stability.md) and [limitations](docs/public/limitations.md) before adoption.

## What it provides

- `Dx.Domain.Kernel`: `Result<T>`, `DomainError`, `Invariant`, `Dx.Result`, and `Dx.Require`
- `Dx.Domain.Primitives`: strongly typed identifiers such as `UserId`, `CorrelationId`, and `TraceId`
- `Dx.Domain.Facts`: immutable structural facts and causation data
- `Dx.Domain.Annotations`: metadata used to express architectural intent
- `Dx.Domain.Analyzers`: compile-time diagnostics for participating C# projects

Enforcement is compiler-, analyzer-, or runtime-based depending on the rule. It is not formal verification and does not cover reflection, serialization materialization, dynamic invocation, or business-semantic correctness.

## Install

```bash
dotnet add package Dx.Domain.Kernel --version 0.1.0-alpha
dotnet add package Dx.Domain.Primitives --version 0.1.0-alpha
dotnet add package Dx.Domain.Analyzers --version 0.1.0-alpha
```

Add `Dx.Domain.Facts` and `Dx.Domain.Annotations` only when their capabilities are needed. Installing the analyzer package explicitly is the documented alpha configuration.

## Minimal example

```csharp
using Dx.Domain;
using Dx.Domain.Errors;
using Dx.Domain.Primitives;

static Result<UserId> ParseUserId(string text)
{
    if (UserId.TryParse(text, provider: null, out var id))
        return Dx.Result.Success(id);

    return Dx.Result.Failure<UserId>(
        DomainError.Create("user.id.invalid", "Expected a non-empty GUID in N format."));
}
```

## Start here

1. [Getting Started](docs/public/getting-started.md)
2. [Quickstart](docs/public/quickstart.md)
3. [Package guide](docs/public/packages/index.md)
4. [Analyzer reference](docs/public/reference/diagnostics/index.md)
5. [Configuration](docs/public/reference/configuration.md)
6. [Alpha release notes](docs/public/release-notes/0.1.0-alpha.md)

## Support and contribution

- Report vulnerabilities privately using [GitHub Security Advisories](SECURITY.md).
- Read [CONTRIBUTING.md](CONTRIBUTING.md) before proposing changes.
- See [CHANGELOG.md](CHANGELOG.md) for user-visible history.

Dx.Domain is licensed under the [MIT License](LICENSE).
