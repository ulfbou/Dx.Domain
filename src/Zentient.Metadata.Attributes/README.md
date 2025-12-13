# Zentient.Metadata.Attributes

[![NuGet Version](https://img.shields.io/nuget/v/Zentient.Metadata.Attributes.svg)](https://www.nuget.org/packages/Zentient.Metadata.Attributes)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-blue.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Zentient.Metadata.Attributes** provides a declarative, attribute-based approach to defining and discovering metadata in .NET applications. It enables you to annotate your types and members with rich metadata, which can be discovered and composed at runtime.

## 🧩 Key Features

- **Attribute-Based Metadata**: Use `[BehaviorDefinition]`, `[CategoryDefinition]`, and `[MetadataTag]` to declare metadata
- **Unified Discovery**: Scan assemblies, types, and members for metadata attributes
- **Legacy & Custom Attribute Support**: Integrates with Zentient.Abstractions.Common.Metadata for cross-package compatibility
- **Analyzer-Ready**: Designed for use with Zentient.Metadata.Analyzers for best practices and DX

## 🚀 Getting Started

Install Zentient.Metadata.Attributes into your .NET project:

```sh
dotnet add package Zentient.Metadata.Attributes
```

**Supported Frameworks:** .NET 8.0, .NET 9.0

### Example Usage

```csharp
using Zentient.Metadata.Attributes;

[BehaviorDefinition]
public class AuditableBehavior : IBehaviorDefinition { }

[CategoryDefinition]
public class ServiceCategory : ICategoryDefinition { }

[MetadataTag(typeof(VersionTag), "1.2")]
public class MyService { }
```

## ✨ Core Concepts
- **Declarative Model**: Move metadata logic out of business code and into component design
- **Unified Scanning**: Discover both Zentient.Metadata and Zentient.Abstractions attributes
- **Analyzer Integration**: Get warnings for duplicate/conflicting tags and missing documentation

## 📚 Documentation
- [CHANGELOG.md](CHANGELOG.md)
- [docs/Zentient_Metadata_Attribute-Specification.md](../docs/Zentient_Metadata_Attribute-Specification.md)
- [docs/usage-examples.md](docs/usage-examples.md)

## 🤝 Contributing
We welcome contributions! Visit our [GitHub Repository](https://github.com/ulfbou/Zentient.Metadata) for source code, issues, and guidelines.

## 📄 License
Zentient.Metadata.Attributes is licensed under the MIT License. See the [LICENSE](../LICENSE) file for details.

---

**Zentient Framework** – Attribute-driven, discoverable metadata for .NET.
