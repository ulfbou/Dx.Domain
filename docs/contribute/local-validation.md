# Local validation

Run validation from the repository root in this order. Stop at the first failure, correct its cause, and restart the sequence.

```bash
dotnet tool restore
dotnet restore
dotnet build -c Release
dotnet test -c Release --no-build
scripts/docs-lint.sh
scripts/docs-snippets-compile.sh docs net8.0
dotnet docfx docfx.public.json
```

Documentation changes must not introduce empty published pages, unresolved conflict markers, broken local links, placeholder contacts, or unclassified enforcement claims.
