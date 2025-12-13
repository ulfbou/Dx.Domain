using Zentient.Abstractions.Metadata;

namespace Zentient.Abstractions.Metadata
{
    /// <summary>
    /// Represents a key-value pair in metadata. Keys can be type-based or preset-based.
    /// </summary>
    public interface IMetadataTag
    {
        object Key { get; }
        object? Value { get; }
    }

    /// <summary>
    /// Represents a typed or registered preset identifier.
    /// </summary>
    public interface IPresetKey
    {
        string Name { get; }
    }

    /// <summary>
    /// Definition-level metadata (type-centric). Supports behavior and category declarations.
    /// </summary>
    public interface IMetadataDefinition : Zentient.Abstractions.Metadata.Definitions.IMetadataDefinition
    {
        // Marker for definition-level metadata
    }

    /// <summary>
    /// Scans assemblies, types, or members to generate IMetadata. Must be deterministic and extensible.
    /// </summary>
    public interface IMetadataScanner
    {
        IMetadata Scan(System.Type type);
        IMetadata Scan(System.Reflection.MemberInfo member);
        System.Collections.Generic.IEnumerable<(System.Reflection.MemberInfo member, IMetadata metadata)> ScanAll(System.Reflection.Assembly assembly);
    }
}
