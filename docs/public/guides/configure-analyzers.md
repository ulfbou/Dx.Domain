# Configure Analyzers

Install `Dx.Domain.Analyzers` explicitly, then provide classification facts in `.editorconfig`:

```ini
root = true

[*.cs]
dx.scope.map = S0:Dx.Domain;S1:MyApp.Domain;S2:MyApp.Application;S3:MyApp.Infrastructure
dx.facade.root = MyApp.Domain.DomainFactory
```

Build with `dotnet build` and treat repository policy separately from descriptor defaults. Standard compiler suppression mechanisms remain technically available, although a repository may prohibit them by policy.
