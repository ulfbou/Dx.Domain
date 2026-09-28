# Troubleshooting

## Diagnostics do not appear

1. Confirm that one of the four published packages is referenced and contains `analyzers/dotnet/cs/Dx.Domain.Analyzers.dll`.
2. Run `dotnet restore` and rebuild.
3. Inspect build output for the analyzer assembly.
4. Confirm the file is included in the participating project.

## A project is classified incorrectly

Review `dx.scope.map` and assembly metadata. Keep mappings specific and ensure the assembly name matches the configured prefix.

## DXA010: Construction Authority Violation

Confirm the code is inside the configured construction boundary or uses the configured facade root. If classification is wrong, correct configuration rather than suppressing the rule.

## DXA011: Public Factory Exposure

Make constructor/factory internal and expose creation via the Dx facade instead. Domain types must not expose public construction paths.

## DXA020: Result Ignored

Return, transform, or terminally handle the Result. Assigning it without later observation may still be unsafe even when a local pattern escapes detection.

## DXA022: Domain Control Exception

Replace with a controlled exception type or DomainError result as required by domain policy.

## DXA030: Unapproved Handler

Use only registered, auditable handler patterns. Verify the handler is integrated through the approved event or subscription boundary.

## DXA040: Kernel Public Surface Freeze

Do not add or modify public surface on Kernel types outside of sanctioned release cycles. Update the configuration if the scope has changed.

## DXA050: Temporal Helper Usage

Replace temporal helpers with the required Chronometer and temporal primitive types.

## DXA060: Forbidden Vocabulary

Replace the forbidden term with an approved alternative as defined in the configuration.

## DXA065: Unresolved Xml Doc Reference

Ensure all XML doc `<see cref="…"/>` and `<see href="…"/>` references point to valid, existing members or URIs.

## DXA070: Generated Code Tagging

Add the required marker or attribute to generated code as configured. Generated code must be tagged to prevent false positives.

## DXA080: Facade Invariant Enforcement

Ensure the facade factory validates inputs and returns DomainError or Result to signal invariant violations.

## Documentation build fails

Run `scripts/docs-lint.sh`, correct the first reported local link or empty page, then run `dotnet docfx docs/docfx.json --warningsAsErrors`.
