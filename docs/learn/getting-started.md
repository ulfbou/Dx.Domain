# Getting Started

> **Retained legacy documentation:** Use the [canonical Getting Started guide](../public/getting-started.md) for supported installation steps and the [canonical Quickstart](../public/quickstart.md) for the compile-tested consumer path.

1. Purpose: Install Dx.Domain packages and get a compile-clean project with analyzers active.
2. When to use:
   - First time adding Dx.Domain to a solution
   - Setting up CI to enforce Result handling
3. When NOT to use:
   - Inside the Dx.Domain kernel repository itself
   - For projects that rely on exceptions for normal flow
4. Guarantees:
   - Consumers install `Dx.Domain.Analyzers` explicitly during alpha
   - Rule applicability is diagnostic-specific and scope-aware
   - Descriptor defaults are authoritative; DXA020 is an error
5. Constraints:
   - Requires .NET 8, 9, or 10 SDK
   - Requires .editorconfig for scope and facade root

## Install

```bash
dotnet add package Dx.Domain.Kernel --prerelease
dotnet add package Dx.Domain.Primitives --prerelease
dotnet add package Dx.Domain.Facts --prerelease
dotnet add package Dx.Domain.Annotations --prerelease
```
Install `Dx.Domain.Analyzers` explicitly during alpha; repository project references do not prove transitive NuGet behavior.

## Enable analyzers

Create `.editorconfig` at the repo root:

```
is_global = true

[*.cs]
dotnet_analyzer_diagnostic.category-Domain.Architecture.severity = warning
dotnet_analyzer_diagnostic.category-Domain.Usage.severity = warning

dx.scope.map = S0:Dx.Domain;S1:MyApp.Domain;S2:MyApp.Application;S3:MyApp.Api
dx.facade.root = MyApp.Domain.Dx
```

## Legacy conceptual example

The following retained example is conceptual. It is not part of the compile-tested public example set:
```csharp
using Dx.Domain;
using Dx.Domain.Kernel;
using Dx.Domain.Primitives;

public static class Demo
{
public static Result<UserId> GetOrCreate(string? raw)
{
if (!Guid.TryParseExact(raw, "N", out var g) || g == Guid.Empty)
{
return Result<UserId>.Failure(
DomainError.Create("Demo.BadId", "Provide a non-empty GUID in N format")
);
}

return Result<UserId>.Success(UserId.FromGuid(g));
}
}
```

Use it:
```csharp
var result = Demo.GetOrCreate("d3c9f1a2b3c4d5e6f7a8b9c0d1e2f3a4");
var message = result.Match(
onSuccess: id => $"ok:{id}",
onFailure: e => $"err:{e.Code}"
);
```

## What will fail CI first

- **DXA010 Construction Authority**: you called new on a domain type in S1–S3. Fix by routing through the facade.
- **DXA020 Result must be handled**: you produced a Result and did not handle it. Fix by using Match, Map, Bind, or an approved handler.
- **DXA022 No throw in Result methods**: you threw inside a method that returns Result. Fix by returning Result.Failure.

## Related links

- Canonical guide: [Getting Started](../public/getting-started.md)
- Compile-tested path: [Quickstart](../public/quickstart.md)
- Reference: [DXA020](../public/reference/diagnostics/DXA020.md)
- Architecture: [Public architecture](../public/architecture.md)
