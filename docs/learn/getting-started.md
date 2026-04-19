# Getting Started

1. Purpose: Install Dx.Domain packages and get a compile-clean project with analyzers active.
2. When to use:
   - First time adding Dx.Domain to a solution
   - Setting up CI to enforce Result handling
3. When NOT to use:
   - Inside the Dx.Domain kernel repository itself
   - For projects that rely on exceptions for normal flow
4. Guarantees:
   - Analyzers ship with the Kernel package and apply to S1–S3
   - S0 Kernel types are exempt from DXA010/DXA011 per ADR-0018
   - Default severities are Warning for DXA010 and DXA020
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
Analyzers are included transitively via Dx.Domain.Kernel.

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

## First compile-clean example

Compiles cleanly:
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

- Up: [Dx.Domain in 90 seconds](index.md)
- Next: [Quickstart](quickstart.md)
- Reference: [DXA020](../analyzers/dxa020.md)
- Governance: [ADR-0018](../adr/ADR-0018-kernel-public-surface.md)

