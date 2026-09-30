# Respond to diagnostics

Treat a diagnostic as evidence of one supported static pattern, then correct the owning design.

## Workflow

1. Record the exact diagnostic ID, message, file, and location.
2. Open its [reference page](../../reference/diagnostics/index.md).
3. Confirm the project scope and effective `.editorconfig` values.
4. Reduce the case to the smallest code that still reports the rule.
5. Apply the documented remediation.
6. Add a regression test containing both reported and accepted forms.
7. Build again and verify the exact diagnostic count.

Bad:

```csharp
ParseUserId(input); // DXA020: returned Result is ignored
```

Good:

```csharp
return ParseUserId(input);
```

Do not silence a warning merely because the current path appears harmless. If repository policy permits suppression, tie it to an auditable reason and test the boundary that remains outside analyzer coverage. An error does not prove that every related misuse was found, and a clean build does not prove business correctness.
