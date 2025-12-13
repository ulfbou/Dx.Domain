# Zentient.Metadata.Abstractions

[![NuGet Version](https://img.shields.io/nuget/v/Zentient.Metadata.Abstractions.svg)](https://www.nuget.org/packages/Zentient.Metadata.Abstractions)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-blue.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Zentient.Metadata.Abstractions** provides the forward-compatible contracts and interfaces for the Zentient metadata system. It enables library and framework authors to build on a stable, extensible foundation for metadata composition, scanning, and registry support.

## 🧩 Key Features

- **Stable Contracts**: All interfaces use the `Zentient.Abstractions.Metadata.*` namespace for zero-churn migration
- **Core Abstractions**: `IMetadata`, `IMetadataBuilder`, `IMetadataScanner`, `IMetadataDefinition`, `IMetadataTag`, `IPresetKey`
- **Extensible Design**: Designed for future migration to the root Zentient.Abstractions package
- **Cross-Package Compatibility**: Supports integration with Zentient.Metadata, Zentient.Metadata.Attributes, and diagnostics packages

## 🚀 Getting Started

Install Zentient.Metadata.Abstractions into your .NET project:

```sh
dotnet add package Zentient.Metadata.Abstractions
```

**Supported Frameworks:** .NET 8.0, .NET 9.0

### Example Usage

```csharp
using Zentient.Abstractions.Metadata;

public class MyMetadataConsumer
{
    public void UseMetadata(IMetadata metadata)
    {
        // Query tags, categories, and behaviors
        if (metadata.ContainsKey("Version"))
        {
            var version = metadata.GetValueOrDefault<string>("Version");
        }
    }
}
```

## ✨ Core Interfaces
- **IMetadata**: Immutable, key-value metadata
- **IMetadataBuilder**: Fluent builder for metadata composition
- **IMetadataScanner**: Pluggable scanner abstraction
- **IMetadataDefinition**: Type-centric metadata definition
- **IMetadataTag**: Key-value tag abstraction
- **IPresetKey**: Strongly-typed preset key system

## 📚 Documentation
- [CHANGELOG.md](CHANGELOG.md)
- [docs/Zentient_Metadata-Specification.md](../docs/Zentient_Metadata-Specification.md)

## 🤝 Contributing
We welcome contributions! Visit our [GitHub Repository](https://github.com/ulfbou/Zentient.Metadata) for source code, issues, and guidelines.

## 📄 License
Zentient.Metadata.Abstractions is licensed under the MIT License. See the [LICENSE](../LICENSE) file for details.

---

**Zentient Framework** – The foundation for modern, extensible metadata in .NET.
