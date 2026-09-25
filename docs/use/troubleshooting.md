# Troubleshooting

## Diagnostics do not appear

1. Confirm that one of the four published packages is referenced and contains `analyzers/dotnet/cs/Dx.Domain.Analyzers.dll`.
2. Run `dotnet restore` and rebuild.
3. Inspect build output for the analyzer assembly.
4. Confirm the file is included in the participating project.

## A project is classified incorrectly

Review `dx.scope.map` and assembly metadata. Keep mappings specific and ensure the assembly name matches the configured prefix.

## DXA010 appears on authorized construction

Confirm the code is inside the configured construction boundary or uses the configured facade root. If classification is wrong, correct configuration rather than suppressing the rule.

## DXA020 reports an ignored Result

Return, transform, or terminally handle the Result. Assigning it without later observation may still be unsafe even when a local pattern escapes detection.

## Documentation build fails

Run `scripts/docs-lint.sh`, correct the first reported local link or empty page, then run `dotnet docfx docs/docfx.json --warningsAsErrors`.
