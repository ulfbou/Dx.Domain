# Facades and Construction Authority

1. Purpose: Explain why domain creation flows through a facade and how analyzers enforce it.
2. When to use:
   - Designing public APIs for aggregates, entities, and value objects
   - Deciding where constructors and factories live
3. When NOT to use:
   - Inside S0 Kernel implementation where construction is authorized
   - For pure data transfer objects outside the domain
4. Guarantees:
   - Single auditable creation path per bounded context
   - Invariants run at the boundary, not scattered in callers
5. Constraints:
   - Public constructors on domain types are discouraged in S1 to S3
   - Factories must be discoverable via dx.facade.root

## What a facade is

A facade is a small static class or set of methods that serves as the public entry point for creating domain instances. It centralizes validation and invariant checks. The kernel calls this the construction authority.

## Construction authority

Analyzers classify types as domain types. Outside S0, direct construction is a violation. The approved path is a method on the facade root or a type reachable from it.

## Public surface rules

- Keep constructors internal in domain assemblies
- Expose creation via facade methods that return Result<T>
- Name factories after the intent, not the mechanics

✅ Compiles cleanly
```csharp
// Example (non-prescriptive)
public static class Dx
{
    public static Result<OrderId> CreateOrderId(string raw)
    {
        return Guid.TryParseExact(raw, "N", out var g) && g != Guid.Empty
            ? Result<OrderId>.Success(OrderId.FromGuid(g))
            : Result<OrderId>.Failure(DomainError.Create("Order.BadId", "Invalid id"));
    }
}
```

⚠️ Triggers DXA010
```csharp
// Example (non-prescriptive)
var id = new OrderId(Guid.NewGuid()); // consumer code
```

## Invariant enforcement

Place invariant checks inside the facade method before constructing the instance. This keeps invariants close to creation and visible to reviewers and analyzers.

## Related diagnostics

- DXA010 Construction Authority Violation
- DXA011 Public factory surface exposure

## Related links

- Up: concepts/index.md
- Guide: guides/build-a-facade.md
- Reference: reference/diagnostics/DXA010.md
