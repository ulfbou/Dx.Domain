# Configuration Reference

## Scope and facade

```ini
[*.cs]
dx.scope.map = S0:Dx.Domain;S1:MyApp.Domain;S2:MyApp.Application;S3:MyApp.Infrastructure
dx.facade.root = MyApp.Domain.DomainFactory
```

## Result handlers

Where supported by the analyzer version, approved handlers and terminalizers can be classified with analyzer configuration:

```ini
dx.result.handlers = MyApp.Results.Observe
dx.result.terminalizers = MyApp.Http.ToResponse
```

Configuration values classify symbols and layers. They do not disable runtime checks or guarantee semantic correctness. Unknown or misspelled values may cause fallback classification, so validate behavior with analyzer tests in the consuming repository.
