# Configuration

> **Retained legacy reference:** This page is subordinate to the [canonical configuration reference](../public/reference/configuration.md) and [task-oriented analyzer guide](../public/guides/configure-analyzers.md).

## Analyzer Configuration

Consumers install `Dx.Domain.Analyzers` explicitly during alpha. Standard compiler suppression mechanisms remain technically available; repository policy may govern their use. Configure participating compilations via `.editorconfig`:

```ini
is_global = true

[*.cs]
dx.scope.map = S0:Dx.Domain;S1:MyApp.Domain;S3:MyApp.App
dx.facade.root = MyApp.Domain.Dx
dx.result.handlers = Dx.Domain.ResultExtensions.Tee
dx.result.terminalizers = Microsoft.AspNetCore.Http.Results
```

## Scope Resolution
Rules are applied based on scope:
- **S0 (kernel)** — trusted
- **S1–S3** — enforced

## DocFX Configuration
See `docfx.json` for metadata generation:
```json
{
  "metadata": [
{
  "src": [{ "files": ["src/**/*.csproj"] }],
  "dest": "docs/api"
}
  ]
}
```

Generated API reference appears in `docs/api/Dx.Domain.*.yml`.

## Analyzer Codes
- **DXA010** Construction Authority
- **DXA011** Public Factory Exposure
- **DXA020** Result Ignored (Error)
- **DXA022** No throw in Result methods
- **DXA030** Unapproved Handler
- **DXA040** Kernel Surface Freeze (Error)
- **DXA050** No temporal helpers
- **DXA060** Forbidden vocabulary (Error)
- **DXA065** Unresolved XML documentation reference
- **DXA070** Generated code tagging
- **DXA080** Facade Invariant Enforcement
DXA090 is planned and unshipped. For authoritative titles, messages, severities, scope, and remediation, use the [canonical diagnostic reference](../public/reference/diagnostics/index.md).
