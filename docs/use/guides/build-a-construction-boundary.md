# Build a construction boundary

This guide creates an S1 factory that owns validation and returns explicit failures.

## 1. Define the value

```csharp
public sealed class CustomerNumber
{
    internal CustomerNumber(string value) => Value = value;
    public string Value { get; }
}
```

## 2. Add the authority

```csharp
using Dx.Domain;
using Dx.Domain.Errors;

public static class DomainFactory
{
    public static Result<CustomerNumber> CreateCustomerNumber(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Dx.Result.Failure<CustomerNumber>(
                DomainError.Create("customer.number.required", "Customer number is required."));
        }

        return Dx.Result.Success(new CustomerNumber(text.Trim()));
    }
}
```

## 3. Configure the boundary

```ini
[*.cs]
dx.scope.map = S0:Dx.Domain;S1:MyApp.Domain;S2:MyApp.Application;S3:MyApp.Infrastructure
dx.facade.root = MyApp.Domain.DomainFactory
```

## 4. Remove competing entry points

Bad:

```csharp
var number = new CustomerNumber(input);
```

Good:

```csharp
Result<CustomerNumber> number = DomainFactory.CreateCustomerNumber(input);
```

Add tests for accepted, rejected, null, empty, and whitespace input. Build with analyzers enabled. DXA010, DXA011, and DXA080 cover supported static patterns, not reflection, serializers, or semantic completeness.
