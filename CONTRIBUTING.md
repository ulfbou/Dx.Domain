# Contributing to Dx.Domain

Dx.Domain keeps its kernel small and moves convenience, integration, and policy to the edges.

## Before opening a change

1. Read the public [architecture](docs/public/architecture.md) and [limitations](docs/public/limitations.md).
2. For kernel changes, review the repository's internal Manifesto, Non-Goals, DPI, and applicable ADRs.
3. Open or link an issue that states the problem, scope, and acceptance criteria.
4. Keep changes focused and use Conventional Commits.

## Validate locally

```bash
dotnet tool restore
dotnet restore
dotnet build -c Release
dotnet test -c Release --no-build
scripts/docs-lint.sh
scripts/docs-snippets-compile.sh docs/public net8.0
```

Documentation changes must contain no empty published pages, unresolved conflict markers, broken local links, placeholder contacts, or unclassified enforcement claims.

## Pull requests

Describe the user-visible effect, validation performed, stability impact, and documentation impact. Kernel surface changes require explicit DPI reasoning and API review.

Security vulnerabilities must be reported through [GitHub Security Advisories](SECURITY.md), not through pull requests or public issues.
