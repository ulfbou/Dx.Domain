# Quickstart

> **Retained legacy documentation:** This scenario is preserved as explanatory material. Use the [canonical Quickstart](../public/quickstart.md) for the compile-tested consumer path.

1. Purpose: Show one complete flow using Result, DomainError, a primitive, a Fact, and a facade.
2. When to use:
   - You installed the packages and want a compile-clean baseline
   - You want to see how analyzers enforce the flow
3. When NOT to use:
   - As a production domain model
   - For infrastructure or persistence concerns
4. Guarantees:
   - The scenario illustrates Result handling, construction boundaries, and structural facts
   - Analyzer applicability remains diagnostic-specific and scope-aware
   - This retained example is conceptual, not compile-tested
5. Constraints:
   - Requires Dx.Domain.Kernel, Primitives, Facts
   - Requires .editorconfig with scope and facade root

## Scenario

Create an order identifier, validate input, emit a fact, and return a Result.

Conceptual example:
```csharp
using Dx.Domain;
using Dx.Domain.Primitives;
using Dx.Domain.Facts;

public sealed record OrderPlaced(string OrderId);

public static class OrderFacade
{
public static Result<OrderId> Create(string? rawId, Causation causation)
{
if (!Guid.TryParseExact(rawId, "N", out var g) || g == Guid.Empty)
{
var err = DomainError.Create("Order.BadId", "OrderId must be a non-empty GUID in N format");
return Result<OrderId>.Failure(err);
}

var id = OrderId.FromGuid(g);

var fact = Fact.Create(
type: "OrderPlaced",
payload: new OrderPlaced(id.ToString()),
causation: causation
);

_ = fact;

return Result<OrderId>.Success(id);
}
}
```

## Using the facade

```csharp
var causation = Causation.Create(
correlationId: CorrelationId.New(),
traceId: TraceId.New(),
actorId: UserId.New()
);

Result<OrderId> result = OrderFacade.Create(
rawId: "d3c9f1a2b3c4d5e6f7a8b9c0d1e2f3a4",
causation: causation
);

string response = result.Match(
onSuccess: id => $"OK:{id}",
onFailure: e => $"ERR:{e.Code}"
);
```

## What the analyzers check

- DXA010: you did not use new OrderId directly in S1 consumer code
- DXA020: you handled the Result with Match
- DXA022: you did not throw inside the Result-returning method

## Minimal .editorconfig

```
is_global = true

[*.cs]
dx.scope.map = S0:Dx.Domain;S1:MyApp.Domain;S3:MyApp.App
dx.facade.root = MyApp.Domain.Dx
```

## Related links

- Canonical Quickstart: [Quickstart](../public/quickstart.md)
- Concept: [Results](concepts/results.md)
- Reference: [DXA010](../public/reference/diagnostics/DXA010.md)
- Architecture: [Public architecture](../public/architecture.md)
