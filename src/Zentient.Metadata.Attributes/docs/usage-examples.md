## **Zentient.Metadata.Attributes – Usage Examples**

### Purpose

The **`Zentient.Metadata.Attributes`** package provides a declarative, attribute-based approach to defining metadata. This simplifies development by allowing you to associate behaviors, categories, and tags directly on your classes, moving metadata logic out of your business code and into your component's design.

This package is intended to be used in conjunction with the `Zentient.Metadata` core library and the `MetadataAttributeReader` utility to automatically discover and process this metadata at runtime.

### Key Concepts

#### The Declarative Model

Instead of manually creating `IMetadata` objects with a builder pattern, you can now use C\# attributes to define your metadata.

For example, to indicate that a service is part of your application's caching pipeline, you simply annotate it with an attribute:

```csharp
[CacheableBehavior]
public class ProductService
{
    // ... service logic
}
```

This declarative model enhances readability, reduces boilerplate code, and ensures a consistent metadata-driven design across your framework.

#### The Attribute Hierarchy

All attributes in this package inherit from the base `MetadataAttribute` class. This allows the `MetadataAttributeReader` to easily discover all metadata-related attributes on a class, even custom ones you create.

  - **`[BehaviorDefinition]`**: Marks a class as a `Behavior`. When applied to a component, it indicates that the component exhibits this behavior.
  - **`[CategoryDefinition]`**: Declares that a component belongs to a specific category.
  - **`[MetadataTag]`**: A flexible attribute for adding custom key-value pairs to a component's metadata.

### Behavior & Category

```csharp
using Zentient.Metadata.Attributes;
using Zentient.Abstractions.Metadata.Definitions;

[BehaviorDefinition]
public class AuditableBehavior : IBehaviorDefinition { }

[CategoryDefinition("service")]
public class ServiceCategory : ICategoryDefinition { }
```

### Tags

```csharp
using Zentient.Metadata.Attributes;

[MetadataTag(typeof(VersionTag), "1.2")]
[MetadataTag(typeof(CacheableTag), true)]
public class MyService { }
```

### Legacy/Abstractions Attributes

```csharp
using Zentient.Abstractions.Common.Metadata;

[DefinitionCategory("service")]
[DefinitionTag("auditable", "cacheable")]
public interface IServiceDefinition { }
```

### Unified Discovery

```csharp
using Zentient.Metadata.Attributes;

var metadata = MetadataAttributeReader.GetMetadata(typeof(MyService));
// metadata will have tags "VersionTag"="1.2", "CacheableTag"=true

var legacyMetadata = MetadataAttributeReader.GetMetadata(typeof(IServiceDefinition));
// metadata will have "category"="service", "tags"=["auditable", "cacheable"]
```

### Custom Attribute Example

```csharp
using Zentient.Metadata.Attributes;

public sealed class MyCustomTagAttribute : MetadataAttribute { }
```

## Analyzer Warnings (DX)
- Duplicate/conflicting tags will trigger analyzer warnings.
- Missing required interfaces for attributed types will trigger analyzer warnings.
- Missing documentation will trigger analyzer warnings.

## Best Practices
- Always use XML documentation and `<example>` tags for public APIs.
- Use attribute-based registration for discoverability.
- Validate your metadata with unit tests.

### Usage Examples

#### 1\. Applying Attributes to a Class

You can apply multiple attributes to a single class to fully describe its metadata.

```csharp
using Zentient.Abstractions.Metadata.Definitions;
using Zentient.Metadata.Attributes;

// Define your metadata types first.
public sealed class DataStoreCategory : ICategoryDefinition { }
public sealed class VersionTag : IMetadataTagDefinition { }

// Apply attributes to a component class.
[DataStoreCategory]
[MetadataTag(typeof(VersionTag), "1.0.0")]
[MetadataTag("Owner", "Zentient Core Team")]
public class OrderRepository
{
    // ...
}
```

#### 2\. Reading Metadata at Runtime

The `MetadataAttributeReader` is the crucial link between your declarative attributes and the runtime metadata system.

```csharp
using System;
using Zentient.Metadata.Attributes;
using Zentient.Metadata.Extensions;

public static class Program
{
    public static void Main()
    {
        // Use reflection to get the Type of your class.
        Type repositoryType = typeof(OrderRepository);

        // Use the MetadataAttributeReader to get the metadata.
        IMetadata repositoryMetadata = MetadataAttributeReader.GetMetadata(repositoryType);

        // You can now query the metadata using the IMetadata extensions.
        bool isDataStore = repositoryMetadata.HasCategory<DataStoreCategory>();
        string version = repositoryMetadata.GetTagValue<VersionTag, string>();
        string owner = repositoryMetadata.GetTagValue<string>("Owner");

        Console.WriteLine($"Is a data store? {isDataStore}");
        Console.WriteLine($"Version: {version}");
        Console.WriteLine($"Owner: {owner}");
    }
}
```

This streamlined process simplifies metadata management, making your code cleaner and more maintainable. It ensures that metadata is always consistent and discoverable without manual setup.

-----

The documentation is now complete. With the code implemented, tested, and documented, the `Zentient.Metadata.Attributes` package is ready for its initial release.
