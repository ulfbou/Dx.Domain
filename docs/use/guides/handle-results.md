# Handle results

Use `Map` for a successful value transformation, `Bind` for a second fallible operation, and `Match` when leaving the result pipeline.

```csharp
Result<UserId> parsed = ParseUserId(input);

Result<string> label = parsed.Map(id => $"user:{id}");

Result<Account> account = parsed.Bind(id => LoadAccount(id));

string response = account.Match(
    value => value.DisplayName,
    error => $"error:{error.Code}");
```

Bad:

```csharp
ParseUserId(input);
```

Good:

```csharp
return ParseUserId(input);
```

A direct discard can trigger DXA020. Passing a result to an unrecognized handler can trigger DXA030. These rules do not prove that a selected handler implements correct business behavior. Keep terminal handling at an application or transport boundary and test both branches.
