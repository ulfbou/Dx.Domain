# Configuration

## Analyzer Configuration

Analyzers are mandatory and cannot be suppressed. Configure via `.editorconfig`:

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
- **DXA070** Generated code tagging
- **DXA080** Facade Invariant Enforcement
