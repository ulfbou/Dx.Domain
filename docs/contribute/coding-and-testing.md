# Coding and testing

## Coding practices

All code changes must:

- **Follow the style guide**: See [style guide](style-guide.md) for documentation, naming, and code commentary conventions
- **Use Conventional Commits**: Follow [Conventional Commits](conventional_commits.md) for all commit messages
- **Pass analyzer enforcement**: Code must compile without analyzer warnings under Release configuration
- **Preserve backward compatibility**: Runtime changes targeting published packages must maintain API and behavior compatibility
- **Document public APIs**: Public types and members must have XML documentation comments

## Analyzer authoring

When adding or modifying analyzers:

- Read [analyzer authoring](analyzer-authoring.md) for diagnostics, descriptors, and implementation patterns
- Follow the established diagnostic ID scheme (DXA010, DXA011, etc.)
- Verify that all descriptors and rules match the [enforcement specification](../understand/enforcement-specification.md)
- Test both positive and negative cases with the analyzer test suite
- Run `scripts/verify-diagnostic-conformance.py` to confirm metadata consistency

## Testing

All changes require test coverage:

- **Analyzer tests** in `tests/Dx.Domain.Analyzers.Tests/`
- **Runtime tests** in `tests/Dx.Domain.Tests/`
- **API tests** to validate public surface stability

Build and run tests locally before submitting:

```bash
dotnet test -c Release --no-build
```

Test failures must be fixed or justified in the pull request.

## API surface changes

Public API additions or modifications require:

- Explicit documentation of the change purpose and scope
- Test coverage demonstrating the new behavior
- Review under the [API surface process](api-surface-process.md)
- Baseline synchronization if required by the CI/CD gates

## Documentation validation

All pull requests undergo documentation validation:

- **Local links**: All Markdown links must resolve to existing files or anchors
- **Code snippets**: All fenced code blocks marked with a language are validated for structure
- **Material examples**: All documentation examples must compile and execute successfully
- **DocFX publication**: The canonical `docs/toc.yml` and `docs/docfx.json` must publish without warnings

Run the [local validation](local-validation.md) sequence before committing.

## Pull-request requirements

Every pull request must:

1. Target only `master` or `release/0.1.0-alpha`
2. Keep changes focused and logically grouped
3. Follow Conventional Commits for clear commit history
4. Include a description of user-visible changes, stability effects, and documentation changes
5. Pass the canonical [local validation](local-validation.md) sequence
6. Report any breaking changes explicitly

See [release process](release-process.md) for guidance on pull-request validation workflows and branch roles.
