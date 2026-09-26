# Local validation

Run from the repository root, stop at the first failure, correct it, and restart the sequence.

```bash
dotnet tool restore
dotnet restore
dotnet build -c Release
dotnet test -c Release --no-build
bash scripts/docs-lint.sh
bash scripts/docs-snippets-compile.sh docs net8.0
bash scripts/docs-examples-compile.sh
dotnet docfx docs/docfx.json --warningsAsErrors
```

Documentation acceptance requires one canonical authored tree, valid local links, compiled material examples, and a warning-free DocFX build from `docs/toc.yml`.
