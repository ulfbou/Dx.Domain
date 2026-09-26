# Configure analyzers

Each of the four published packages carries the analyzer assembly. Install the package that owns the runtime or metadata capability you use. Do not add a standalone analyzer package.

## Classify scopes and authority

```ini
root = true

[*.cs]
dx.scope.map = S0:Dx.Domain;S1:MyApp.Domain;S2:MyApp.Application;S3:MyApp.Infrastructure
dx.facade.root = MyApp.Domain.DomainFactory
dx.result.handlers = MyApp.Results.Observe
dx.result.terminalizers = MyApp.Http.ToResponse
```

## Verify effective behavior

1. Restore and build the consuming project.
2. Add one small source fixture that should trigger the relevant diagnostic.
3. Add the corrected form and verify that the diagnostic disappears.
4. Inspect the diagnostic ID and default severity in the [reference](../../reference/diagnostics/index.md).

## Common failure modes

- A package reference is absent, so the analyzer asset is not loaded.
- An assembly prefix does not match the produced assembly name.
- `dx.facade.root` does not resolve to the intended symbol.
- A handler is configured by the wrong fully qualified name.
- `.editorconfig` scope or precedence prevents the setting from applying.

Configuration supplies classification facts. It does not disable runtime limits or prove that a handler is semantically correct.
