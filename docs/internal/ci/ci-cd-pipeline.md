# CI/CD Pipeline

**Status:** Maintainer reference for the workflows currently present in this repository.

## Active workflows

### Build & Validate

File: `.github/workflows/build-validate.yml`

Runs for pull requests targeting `master` or `release/0.1.0-alpha`. It restores dependencies, builds Release, runs tests, lints public documentation, checks C# fence structure, and compiles the material documentation Quickstart.

### CI (Analyzers & Docs Governance)

File: `.github/workflows/ci-analyzers.yml`

Runs for pull requests targeting `master` or `release/0.1.0-alpha`. It builds the repository, runs analyzer tests, checks the analyzer API baseline, lints public documentation, performs structural snippet validation, and invokes the API-diff gate when that script is present.

### Build & Publish DocFX Documentation

File: `.github/workflows/docfx.yml`

Runs manually or after relevant public-documentation changes reach `master`. It builds only `docs/public` through `docfx.public.json`, uploads one site artifact, and deploys that output once to `gh-pages`.

### Release CD

File: `.github/workflows/release-cd.yml`

Runs only through manual dispatch. The workflow enforces `master` as its publication branch, builds package artifacts, and models staging, canary, production, and GitHub-release jobs. A job boundary or echo command is not evidence that an external deployment occurred.

## Branch model

- `master`: default branch, documentation publication source, and release publication source.
- `release/0.1.0-alpha`: integration base for alpha readiness work and a PR-validation target.
- `gh-pages`: generated public documentation output.

There is no active repository-local `main` or `develop` workflow path.

## Validation model

- Build and tests validate compiled repository behavior.
- Analyzer tests validate supported diagnostic behavior.
- Documentation lint validates public emptiness, placeholders, Markdown links, TOC targets, and merge markers.
- Structural snippet validation checks fenced C# shape only.
- `scripts/docs-examples-compile.sh` performs actual compilation of the material Quickstart.
- DocFX validates the canonical public site with warnings treated as errors.

## Authority

Executable workflow files take precedence over this summary. When this document and a workflow disagree, correct this document from the workflow or change the workflow through an explicitly scoped work item.