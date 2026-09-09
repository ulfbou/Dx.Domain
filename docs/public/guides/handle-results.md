# Handle Results

Create Results through the public `Dx.Result` facade:

```csharp
var error = DomainError.Create("order.not_found", "The order was not found.");
Result<Order> result = Dx.Result.Failure<Order>(error);
```

Handle both outcomes:

```csharp
string response = result.Match(
    order => order.Id.ToString(),
    failure => failure.Code);
```

Returning, mapping, binding, or matching a Result can make intent explicit. DXA020 detects directly ignored values but does not prove correct downstream behavior.
