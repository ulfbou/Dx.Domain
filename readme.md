# Zentient.Metadata

<<<<<<< HEAD
A modern, extensible metadata platform for .NET. This monorepo contains the core packages used to declare, compose, discover, and analyze metadata in libraries and applications.

[![CI](https://github.com/ulfbou/Zentient.Metadata/actions/workflows/ci-cd.yml/badge.svg)](.github/workflows/ci-cd.yml)
=======
[![.github/workflows/ci-cd.yml](https://github.com/ulfbou/Zentient.Metadata/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/ulfbou/Zentient.Metadata/actions/workflows/ci-cd.yml)
>>>>>>> origin/main
[![NuGet](https://img.shields.io/nuget/v/Zentient.Metadata.svg)](https://www.nuget.org/packages/Zentient.Metadata)

What’s inside
- Zentient.Metadata — Core metadata engine (immutable metadata, fluent builder, scanners)
- Zentient.Metadata.Attributes — Attribute-based metadata discovery and conversion
- Zentient.Metadata.Abstractions — Interfaces and contracts used across packages
- Zentient.Metadata.Analyzers — Roslyn analyzers for best practices and correctness
- Zentient.Metadata.Diagnostics — Diagnostic helpers and profiles

Goals
- Provide a small, stable set of abstractions for metadata composition and discovery
- Offer a flexible runtime model with first-class support for attribute-driven metadata
- Deliver tooling (analyzers and diagnostics) to improve developer experience

Quick start
1. Add the package you need:

```bash
dotnet add package Zentient.Metadata
# or
dotnet add package Zentient.Metadata.Attributes
```

2. Build metadata using the fluent API:

```csharp
var metadata = Metadata.Create()
    .SetTag("Version", "1.0.0")
    .SetTag("Author", "Zentient Team")
    .Build();

var version = metadata.GetValueOrDefault<string>("Version");
```

Documentation
- API reference: https://ulfbou.github.io/Zentient.Metadata/
- Specification and design docs: docs/
- CHANGELOG: CHANGELOG.md

CI / Release
- The repository uses GitHub Actions (.github/workflows/ci-cd.yml) to run restore, build, test, pack, and publish.
- Releases are triggered by tags using the form `vMAJOR.MINOR.PATCH` and publish packages to NuGet.org.

Contributing
- See CONTRIBUTING.md for contribution and release guidelines.
- Open issues and PRs on GitHub; all changes should include tests and updates to CHANGELOG.md when applicable.

License
- MIT — see LICENSE file.

