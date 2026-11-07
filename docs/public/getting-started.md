# Getting Started

**Audience:** .NET developers evaluating `0.1.0-alpha`
**Outcome:** install the minimum packages, enable analyzers, and build a Result-returning method.

## Prerequisites

- A project targeting .NET 8, .NET 9, or .NET 10
- An SDK compatible with the selected target framework
- NuGet access to the feed containing the alpha packages

## Install the minimum set

```bash
dotnet add package Dx.Domain.Kernel --version 0.1.0-alpha
dotnet add package Dx.Domain.Primitives --version 0.1.0-alpha
dotnet add package Dx.Domain.Analyzers --version 0.1.0-alpha
```

Install analyzers explicitly during alpha. Add `Dx.Domain.Facts` when you need structural facts and `Dx.Domain.Annotations` when you need semantic metadata directly.

## Configure analyzer facts

Create or update `.editorconfig`:

```ini
root = true

[*.cs]
dx.scope.map = S0:Dx.Domain;S1:MyApp.Domain;S2:MyApp.Application;S3:MyApp.Infrastructure
dx.facade.root = MyApp.Domain.DomainFactory
```

Configuration supplies classification facts. It does not expand runtime guarantees.

## First Result

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

Build the project:

```bash
dotnet build
```

Next, follow the [Quickstart](quickstart.md) and review [Result handling](guides/handle-results.md).
