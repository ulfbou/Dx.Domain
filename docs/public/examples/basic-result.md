# Basic Result Usage

```csharp
var result = Order.Create(customerId);

if (result.IsFailure)
    return result;

return result.Value;
```

## Violations:

- Ignoring `result` triggers DXA020
- Throwing instead of returning triggers DXA022
