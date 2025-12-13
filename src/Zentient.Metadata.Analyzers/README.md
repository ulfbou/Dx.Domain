# Zentient.Metadata.Analyzers

[![NuGet Version](https://img.shields.io/nuget/v/Zentient.Metadata.Analyzers.svg)](https://www.nuget.org/packages/Zentient.Metadata.Analyzers)
[![.NET](https://img.shields.io/badge/.NET-Standard%202.0-blue.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Zentient.Metadata.Analyzers** provides Roslyn analyzers for the Zentient.Metadata ecosystem, ensuring best practices, discoverability, and developer experience (DX) for attribute-based metadata in .NET projects.

## 🧩 Key Features

- **Analyzer Rules**: Enforce correct usage of metadata attributes and tags
- **Duplicate/Conflict Detection**: Warnings for duplicate/conflicting tags and attributes
- **Documentation Enforcement**: Analyzer rules for missing or incomplete documentation
- **DX Enhancements**: Designed for seamless integration with IDEs and CI pipelines

## 🚀 Getting Started

Install Zentient.Metadata.Analyzers into your .NET project:

```sh
dotnet add package Zentient.Metadata.Analyzers
```

**Supported Frameworks:** .NET Standard 2.0 (compatible with .NET 6.0, 7.0, 8.0, 9.0)

### Example Usage

Add the analyzer package to your project to receive compile-time feedback and warnings for Zentient.Metadata attribute usage:

- Duplicate/conflicting tags
- Missing required interfaces
- Missing documentation

## ✨ Analyzer Coverage
- **Attribute Usage**: Ensures only valid targets and correct inheritance
- **Tag Consistency**: Detects duplicate or conflicting tags
- **Documentation**: Warns on missing or incomplete XML docs

## 📚 Documentation
- [CHANGELOG.md](CHANGELOG.md)
- [docs/Zentient_Metadata_Attribute-Specification.md](../docs/Zentient_Metadata_Attribute-Specification.md)

## 🤝 Contributing
We welcome contributions! Visit our [GitHub Repository](https://github.com/ulfbou/Zentient.Metadata) for source code, issues, and guidelines.

## 📄 License
Zentient.Metadata.Analyzers is licensed under the MIT License. See the [LICENSE](../LICENSE) file for details.

---

**Zentient Framework** – Analyzer-driven quality and DX for metadata in .NET.
