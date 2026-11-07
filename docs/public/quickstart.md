# Quickstart

**Audience:** first-time consumers
**Status:** alpha sample using shipped Kernel and Primitives APIs

This example parses a typed identity and returns an explicit domain failure.

```csharp
using Dx.Domain;
using Dx.Domain.Errors;
using Dx.Domain.Primitives;

static Result<UserId> ParseUserId(string text) =>
    UserId.TryParse(text, null, out var id)
        ? Dx.Result.Success(id)
        : Dx.Result.Failure<UserId>(
            DomainError.Create("user.id.invalid", "Expected a non-empty GUID in N format."));
```

Handle both outcomes at the boundary:

```csharp
var result = ParseUserId("d3c9f1a2b3c4d5e6f7a8b9c0d1e2f3a4");
var message = result.Match(
    id => $"user:{id}",
    error => $"error:{error.Code}");
```

The typed identifier is **compiler-enforced**. Its non-empty construction rule is **runtime-enforced**. Explicit observation of returned Results is **analyzer-enforced** within supported static patterns. Correct business behavior is **not guaranteed**.

See [Configuration](reference/configuration.md) and [DXA020](reference/diagnostics/DXA020.md).
