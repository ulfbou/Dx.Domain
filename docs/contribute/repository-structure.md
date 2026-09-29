# Repository structure

## Directory layout

The Dx.Domain repository is organized into:

- **src/** — Published packages and repository implementation projects
  - `Dx.Domain.Annotations` — semantic attributes and markers (netstandard2.0)
  - `Dx.Domain.Kernel` — results, errors, invariants (net8.0, net9.0, net10.0)
  - `Dx.Domain.Primitives` — typed identities and primitives (net8.0, net9.0, net10.0)
  - `Dx.Domain.Facts` — immutable structural facts (net8.0, net9.0, net10.0)
  - `Dx.Domain.Analyzers` — compiler analyzers (repository implementation; packaged within published packages)
  - `Dx.Domain.Generators` — source generators for compile-time support

- **tests/** — Test projects
  - `Dx.Domain.Analyzers.Tests` — analyzer validation
  - `Dx.Domain.Tests` — runtime and API tests

- **docs/** — Canonical documentation tree
  - `index.md` — documentation root
  - `toc.yml` — navigation authority
  - `docfx.json` — DocFX build configuration
  - `use/` — consumer guidance
  - `understand/` — architecture and enforcement
  - `reference/` — package, API, and diagnostic lookup
  - `contribute/` — contributor guidance
  - `internal/` — transitional; excluded from publication

- **.dx/** — Release-gate and documentation-analysis artifacts
  - `.dx/docs-reimagination/` — Phase 1–3 evidence and validation scripts
  - `.dx/phase-*.dx.txt` — DX v2.0 carriers for completed phases

- **.github/workflows/** — CI/CD automation
  - `build-validate.yml` — restore, build, test, docs validation
  - `ci-analyzers.yml` — analyzer tests and governance
  - `docfx.yml` — documentation publication to `gh-pages`
  - `release-cd.yml` — package publication workflow

- **builds/** — MSBuild properties and targets
  - `common/` — shared constants and defaults
  - `policy/` — governance and analyzer configuration
  - `ci/` — CI-specific properties
  - `identity/` — identity resolution and versioning

- **scripts/** — Validation and release-gate tooling
  - `docs-lint.sh` — documentation link and structure validation
  - `docs-snippets-compile.sh` — structural code snippet checks
  - `docs-examples-compile.sh` — material documentation-example compilation
  - `release-gate/dx.py` — DX v2.0 carrier codec and CLI
  - `verify-diagnostic-conformance.py` — analyzer metadata verification

## Canonical authorities

- **docs/index.md** — Documentation entry point
- **docs/toc.yml** — Sole navigation authority
- **docs/docfx.json** — Publication boundary and build configuration
- **docs/contribute/documentation-authority.md** — Documentation scoping contract
- **Dx.Domain.sln** — Project and solution structure
- **CONTRIBUTING.md** — Contributor entry point

## Branches and release flow

- **master** — Integration base; publication source for packages and documentation
- **release/0.1.0-alpha** — Alpha release preparation base; not publication source
- **gh-pages** — Generated documentation site (read-only; updated by publication workflow)

## Documentation structure

See [documentation authority](documentation-authority.md) for the publication boundary and content scoping.

See [local validation](local-validation.md) for the canonical validation sequence that runs before contributions are accepted.
