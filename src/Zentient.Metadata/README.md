# Zentient.Metadata

[![NuGet Version](https://img.shields.io/nuget/v/Zentient.Metadata.svg)](https://www.nuget.org/packages/Zentient.Metadata)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-blue.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Zentient.Metadata** is the core metadata engine for the Zentient Framework, providing a powerful, attribute-based system for declaring, composing, and discovering metadata in .NET applications. It enables a declarative, extensible, and immutable metadata model for modern frameworks and platforms.

## 🧩 Key Features

- **Immutable Metadata Objects**: Safe, deterministic, and thread-safe `IMetadata` instances
- **Fluent Builder Pattern**: Compose metadata with `IMetadataBuilder` for maximum flexibility
- **Attribute & Code-Defined Metadata**: Seamlessly combine attribute-based and runtime metadata
- **Pluggable Scanners**: Discover metadata from assemblies, types, and members
- **Registry & Preset Key Support**: Organize and resolve metadata with strong typing
- **Extensible & Composable**: Merge, extend, and query metadata with rich extension methods

## 🚀 Getting Started

Install Zentient.Metadata into your .NET project:

```sh
dotnet add package Zentient.Metadata
```

**Supported Frameworks:** .NET 8.0, .NET 9.0

### Quick Start Example

```csharp
using Zentient.Metadata;
using Zentient.Abstractions.Metadata;

// Build metadata using the fluent builder
var metadata = Metadata.Create()
    .SetTag("Version", "1.0.0")
    .SetTag("Author", "Zentient Team")
    .Build();

// Query metadata
string version = metadata.GetValueOrDefault<string>("Version");

// Attribute-based discovery (see Zentient.Metadata.Attributes)
// var discovered = MetadataAttributeReader.GetMetadata(typeof(MyService));
```

## ✨ Core Concepts

- **IMetadata**: Immutable, key-value metadata with type-safe access
- **IMetadataBuilder**: Fluent API for composing metadata
- **IMetadataScanner**: Pluggable scanners for runtime and attribute-based discovery
- **Preset Keys**: Strongly-typed keys for organizing metadata
- **Extension Methods**: Rich API for merging, querying, and composing metadata

## 📚 Documentation
- [CHANGELOG.md](../CHANGELOG.md)
- [Zentient_Metadata-Specification.md](../docs/Zentient_Metadata-Specification.md)

## 🤝 Contributing
We welcome contributions! Visit our [GitHub Repository](https://github.com/ulfbou/Zentient.Metadata) for source code, issues, and guidelines.

## 📄 License
Zentient.Metadata is licensed under the MIT License. See the [LICENSE](../LICENSE) file for details.

---

**Zentient Framework** – Modern, declarative, and extensible metadata for .NET applications.
