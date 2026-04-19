# Handle errors with Result

1. Purpose: Show the mechanical patterns that satisfy DXA020 Result must be handled.
2. When to use:
   - You have a method returning Result<T> or Result
   - You need to propagate, transform, or terminalize a result
3. When NOT to use:
   - For throwing exceptions as normal control flow
   - For infrastructure failures that should remain exceptions
4. Guarantees:
   - All samples state whether they compile cleanly or trigger a diagnostic
   - Patterns align with analyzer-approved handlers
5. Constraints:
   - Do not discard Result without explicit intent
   - Use only handlers listed in dx.result.handlers or terminalizers in dx.result.terminalizers

## Result.Match

Explicitly handle both outcomes.

✅ Compiles cleanly
```csharp
// Example (non-prescriptive)
Result<OrderId> result = CreateOrder();

result.Match(
    onSuccess: id => Console.WriteLine(id),
    onFailure: error => Log(error)
);
```

⚠️ Triggers DXA020
```csharp
// Example (non-prescriptive)
var result = CreateOrder(); // result is not used
```

Fix: add Match, Map, Bind, or an approved handler.

## Result.Map

Transform success value, preserve error.

✅ Compiles cleanly
```csharp
Result<UserId> user = GetUser();
Result<string> asString = user.Map(u => u.ToString());
```

## Result.Bind

Chain operations that return Result.

✅ Compiles cleanly
```csharp
Result<OrderId> step1 = CreateOrder();
Result<Confirmation> step2 = step1.Bind(id => Confirm(id));
```

## Terminalize at boundaries

At application edges, convert Result to a terminal action.

✅ Compiles cleanly with default terminalizer
```csharp
Result<OrderId> result = CreateOrder();
return result.Match(
    onSuccess: id => Results.Ok(id),
    onFailure: err => Results.BadRequest(err.Code)
);
```

Configure additional terminalizers in .editorconfig:
```
dx.result.terminalizers = MyApp.Results.ToActionResult
```

## Explicit discard with intent

If you intentionally ignore a result, use an approved discard handler.

✅ Compiles cleanly when configured
```csharp
Result<Unit> r = LogFact(fact);
r.Tee(_ => { }); // Tee is an approved handler when listed
```

⚠️ Triggers DXA020 when not configured
```csharp
LogFact(fact); // discarded
```

## DomainError.Create

Create stable, comparable errors.

✅ Compiles cleanly
```csharp
var error = DomainError.Create("Order.MissingCustomer", "CustomerId is required");
return Result<OrderId>.Failure(error);
```

## Related links

- Up: guides/handle-errors.md parent in toc.yml
- Concept: concepts/results.md
- Reference: reference/diagnostics/DXA020.md
