# Zentient.Metadata Attribute Specification

## Purpose

This document specifies the design, semantics, and discoverability rules for attributes in the Zentient.Metadata ecosystem, focusing on their role in generating and discovering `IMetadata` and `IMetadataDefinition` instances. It also covers integration and usage of `Zentient.Abstractions.Common.Metadata` attributes for legacy and cross-package compatibility.

-----

## 1\. **Attribute Categories**

### Core Attribute Types

| Attribute Name                       | Namespace                            | Purpose                                                        | Targets                   | Repeatable | Notes                                       |
|--------------------------------------|--------------------------------------|----------------------------------------------------------------|---------------------------|------------|---------------------------------------------|
| **`BehaviorDefinitionAttribute`**    | Zentient.Metadata.Attributes         | Declares a type as a metadata behavior definition.             | Interface, Class          | No         | Must implement `IBehaviorDefinition`         |
| **`CategoryDefinitionAttribute`**    | Zentient.Metadata.Attributes         | Declares a type as a metadata category definition.             | Interface, Class          | No         | Must implement `ICategoryDefinition`         |
| **`MetadataTagAttribute`**           | Zentient.Metadata.Attributes         | Associates a metadata tag on a type or member.                 | Class, Property, Method   | Yes        | Requires a tag type and a value              |
| **`DefinitionCategoryAttribute`**    | Zentient.Abstractions.Common.Metadata| Declares a category for a definition.                          | Interface, Class          | No         | Used for legacy and cross-package integration|
| **`DefinitionTagAttribute`**         | Zentient.Abstractions.Common.Metadata| Declares tags for a definition.                                | Interface, Class          | Yes        | Used for legacy and cross-package integration|
| *Custom Attributes*                  | Zentient.Metadata.Attributes.*       | User-defined attributes for extensions, diagnostics, etc.      | Any                       | Varies     | Must inherit from `MetadataAttribute`        |

-----

## 2\. **Attribute Semantics**

  * **Declarative Only:** Attributes do NOT directly execute logic; they declare metadata intent.
  * **Conversion:** Attribute scanners convert discovered attributes to equivalent `IMetadata` instances using a well-defined mapping.
  * **Immutability:** Attribute metadata is immutable at runtime.
  * **Inheritance:** Attribute discovery respects inheritance where `[Inherited = true]` is specified. Some attributes are only applied to the declaring type.
  * **Cross-Package Compatibility:** `DefinitionCategoryAttribute` and `DefinitionTagAttribute` from `Zentient.Abstractions.Common.Metadata` are supported for legacy, interoperability, and registry purposes.

-----

## 3\. **Discovery Rules**

### General Discovery Workflow:

  * **Enumerate Types:** Assembly scan for all types with attribute(s) deriving from `MetadataAttribute`, as well as those using `DefinitionCategoryAttribute` and `DefinitionTagAttribute`.
  * **Attribute Extraction:** For each type/member, retrieve all relevant metadata attributes (including via inheritance as applicable).
  * **Mapping:** Map attributes to corresponding metadata keys and values using the following rules:
      * **`BehaviorDefinitionAttribute`**: The attribute itself serves as a marker. The scanner creates a behavior instance from the attributed type.
      * **`CategoryDefinitionAttribute`**: The attribute itself serves as a marker. The scanner creates a category instance from the attributed type.
      * **`MetadataTagAttribute`**: The key is the full name of the tag type (`tagType.FullName`), and the value is the provided `value`.
      * **`DefinitionCategoryAttribute`**: The key is `"category"`, the value is the `CategoryName` property. Used for registry and catalog integration.
      * **`DefinitionTagAttribute`**: The key is `"tags"`, the value is the array of tag strings (`Tags` property). Used for registry and catalog integration.
  * **Conflict Resolution:**
      * If multiple attributes declare the same key, resolution order is:
          * Most-derived type wins.
          * Explicit `Order`/`Priority` property wins if present.
          * Otherwise, last declared wins.
  * **Composition:** Construct an `IMetadataBuilder`, apply all discovered tags/behaviors/categories, then build the final immutable `IMetadata`.

### Special Notes:

  * **Composite Attributes:** (e.g., `[Behaviors(...)]`) must be decomposed by the scanner and treated as multiple behaviors.
  * **Repeatable Attributes:** Multiple tags with the same key and different values raise a warning (see analyzer ZMD002).
  * **Legacy/Abstractions Attributes:** `DefinitionCategoryAttribute` and `DefinitionTagAttribute` are treated equivalently to Zentient.Metadata attributes, but are primarily used for registration/catalog scenarios.
  * **Interoperability:** Scanners should unify both Zentient.Metadata and Zentient.Abstractions.Common.Metadata attributes into a single metadata representation, ensuring smooth integration across packages.

-----

## 4\. **Attribute Discovery APIs**

### Core Scanner

```csharp
public interface IAttributeMetadataScanner
{
    IMetadata Scan(Type type);
    IMetadata Scan(MemberInfo member);
    IEnumerable<(MemberInfo member, IMetadata metadata)> ScanAll(Assembly assembly);
}
```

### Conversion Helper

```csharp
public static class AttributeMetadataConverter
{
    public static IMetadata Convert(IEnumerable<Attribute> attributes);
}
```
*Note: The converter must accept both `MetadataAttribute` and `DefinitionCategoryAttribute`/`DefinitionTagAttribute`.*

-----

## 5\. **Extensibility**

  * **Custom Attributes:** Users may define new attributes inheriting from `MetadataAttribute`. These must be registered with the scanner/converter to participate in metadata generation.
  * **Legacy Extension:** Additional attributes from `Zentient.Abstractions.Common.Metadata` may be incorporated for legacy/migration scenarios.
  * **Presets Registry:** Attribute-based presets may be registered and discovered via `[Preset("name")]` attributes.

-----

## 6\. **DX Considerations**

  * **IDE Discoverability:** All attributes should have clear XML documentation and usage examples.
  * **Roslyn Analyzer Rules:** Warn on incorrect usage, duplicates, or missing required attributes.
  * **Unit Tests:** Coverage for attribute-to-metadata conversion, inheritance, and conflict resolution.
  * **Unified Metadata Surface:** Developers should be able to query all metadata, regardless of whether it was declared via Zentient.Metadata or Zentient.Abstractions attributes.

-----

## 7\. **Acceptance Criteria**

  * All public attribute types (including legacy/abstractions) are discoverable via assembly/type/member scan.
  * Attribute scanners yield deterministic, immutable `IMetadata` instances.
  * Duplicate/conflicting attribute applications trigger analyzer warnings.
  * Custom attributes are supported via registration/extension.
  * Registry/catalog flows can utilize both Zentient.Metadata and Zentient.Abstractions.Common.Metadata attributes.

-----

## 8\. **Examples**

### Behavior & Category

```csharp
[BehaviorDefinition]
public class AuditableBehavior : IBehaviorDefinition { }

[CategoryDefinition]
public class ServiceCategory : ICategoryDefinition { }

[DefinitionCategory("service")]
public interface IServiceDefinition { }
```

### Tags

```csharp
[MetadataTag(typeof(VersionTag), "1.2")]
[MetadataTag(typeof(CacheableTag), true)]
public class MyService { }

[DefinitionTag("auditable", "cacheable")]
public interface IServiceDefinition { }
```

### Discovery (Unified)

```csharp
var metadata = AttributeMetadataReader.GetMetadata(typeof(MyService));
// metadata will have tags "VersionTag"="1.2", "CacheableTag"=true

var legacyMetadata = AttributeMetadataReader.GetMetadata(typeof(IServiceDefinition));
// metadata will have "category"="service", "tags"=["auditable", "cacheable"]
```

-----

## 9\. **Integration with Zentient.Abstractions**

  * Attributes discovered in Metadata should be compatible with those declared in Abstractions (e.g., `DefinitionTagAttribute`, `DefinitionCategoryAttribute`).
  * The scanner should unify both sources for IMetadataDefinition generation.
  * Catalog and registry APIs should support querying by both Zentient.Metadata and Zentient.Abstractions.Common.Metadata attributes.

-----

## 10\. **Future-Proofing**

  * Attribute properties may include `Order`, `Priority`, and `PresetName` for advanced scenarios.
  * Consider supporting source generators for compile-time metadata emission.
  * Maintain backward compatibility with legacy abstractions attributes for migration purposes.

-----

**End of Specification**