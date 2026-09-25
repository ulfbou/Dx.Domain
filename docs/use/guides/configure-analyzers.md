# Configure analyzers

The analyzer assembly is embedded in each of the four published packages. Install the package that owns the capability you use; do not install a standalone analyzer package.

```ini
[*.cs]
dx.scope.map = S0:Dx.Domain;S1:MyApp.Domain;S2:MyApp.Application;S3:MyApp.Infrastructure
dx.facade.root = MyApp.Domain.DomainFactory
```

Build with `dotnet build`, then use the [diagnostic reference](../../reference/diagnostics/index.md).
