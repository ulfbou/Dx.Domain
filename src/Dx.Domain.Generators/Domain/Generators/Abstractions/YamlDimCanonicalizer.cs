// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FileName.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Generators.Model;
using Dx.Domain.Generators.Preview;

using System.Collections.Immutable;
using System.Text.Json;

using YamlDotNet.Serialization;

using YamlDotNet.Serialization.NamingConventions;
namespace Dx.Domain.Generators.Canonicalization
{
    public sealed class YamlDimCanonicalizer : IDimCanonicalizer
    {
        private readonly IDeserializer _deserializer;

        // Canonical JSON options: No indentation, strictly ordered by implementation logic below
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public YamlDimCanonicalizer()
        {
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties() // Schema validation handles strictness; we ignore extras here
                .Build();
        }

        public Result<(string CanonicalJson, DomainIntentModel Model)> Canonicalize(
            string rawYamlContent,
            string filePath)
        {
            try
            {
                // 1. Parse YAML to Raw DTOs (Loose Typing)
                var raw = _deserializer.Deserialize<RawDomainModel>(rawYamlContent);

                if (raw == null)
                    return Dx.Result.Failure<(string, DomainIntentModel)>(
                        Dx.Faults.Factory.Create("DX-DIM-001", $"Empty or invalid YAML in {filePath}"));

                // 2. Validate Version (Phase 1 Gate)
                if (!raw.ModelVersion.StartsWith("1.", StringComparison.Ordinal))
                    return Dx.Result.Failure<(string, DomainIntentModel)>(
                        Dx.Faults.Factory.Create("DX-DIM-002", $"Unsupported modelVersion '{raw.ModelVersion}' in {filePath}. Only 1.x is supported."));

                // 3. Map & Normalize (The Sort Law application)
                // We order ALL collections by Name/Key using Ordinal comparison to ensure
                // that logical identity results in byte-identity for hashing.
                var model = new DomainIntentModel
                {
                    ModelVersion = raw.ModelVersion.Trim(),

                    // Flatten templateOptions into Metadata and Sort by Key
                    Metadata = NormalizeMetadata(raw.Metadata, raw.TemplateOptions),

                    ValueObjects = raw.ValueObjects.OrderBy(x => x.Name, StringComparer.Ordinal)
                        .Select(MapValueObject).ToImmutableArray(),

                    Entities = raw.Entities.OrderBy(x => x.Name, StringComparer.Ordinal)
                        .Select(MapEntity).ToImmutableArray(),

                    Aggregates = raw.Aggregates.OrderBy(x => x.Name, StringComparer.Ordinal)
                        .Select(MapAggregate).ToImmutableArray(),

                    Snapshots = raw.Snapshots.OrderBy(x => x.Name, StringComparer.Ordinal)
                        .Select(MapSnapshot).ToImmutableArray(),

                    Events = raw.Events.OrderBy(x => x.Name, StringComparer.Ordinal)
                        .Select(MapEvent).ToImmutableArray(),

                    Repositories = raw.Repositories.OrderBy(x => x.Name, StringComparer.Ordinal)
                        .Select(MapRepository).ToImmutableArray(),

                    Collections = raw.Collections.OrderBy(x => x.Name, StringComparer.Ordinal)
                        .Select(MapCollection).ToImmutableArray()
                };

                // 4. Generate Canonical JSON (Input Fingerprint Source)
                var canonicalJson = JsonSerializer.Serialize(model, JsonOptions);

                return Dx.Result.Ok((canonicalJson, model));
            }
            catch (Exception ex)
            {
                // We trap parser errors and convert them to proper Domain Errors
                return Dx.Result.Failure<(string, DomainIntentModel)>(
                    Dx.Faults.Factory.Create("DX-DIM-000", $"Canonicalization failed for {filePath}: {ex.Message}"));
            }
        }

        // --- Normalization Helpers ---

        private static ImmutableDictionary<string, string> NormalizeMetadata(
            Dictionary<string, string>? metadata,
            Dictionary<string, string>? options)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.Ordinal);

            if (metadata != null)
                foreach (var kvp in metadata)
                    builder[kvp.Key.Trim()] = kvp.Value?.Trim() ?? "";

            if (options != null)
                foreach (var kvp in options)
                    builder[$"opt:{kvp.Key.Trim()}"] = kvp.Value?.Trim() ?? "";

            return builder.ToImmutable();
        }

        private static ValueObjectIntent MapValueObject(RawValueObject raw) => new(
            Name: raw.Name.Trim(),
            Kind: string.IsNullOrWhiteSpace(raw.Kind) ? "small" : raw.Kind.Trim(), // Explicit Default
            Fields: raw.Fields.OrderBy(f => f.Name, StringComparer.Ordinal).Select(MapField).ToImmutableArray()
        );

        private static Model.EntityIntent MapEntity(RawEntity raw) => new(
            Name: raw.Name.Trim(),
            Id: raw.Id.Trim(),
            IdType: raw.IdType?.Trim() ?? $"{raw.Name}Id", // Inferred Default
            Properties: raw.Properties.OrderBy(p => p.Name, StringComparer.Ordinal).Select(MapProperty).ToImmutableArray(),
            FactoryParams: raw.FactoryParams?.Select(MapField).ToImmutableArray() ?? ImmutableArray<FieldIntent>.Empty
        );

        private static AggregateIntent MapAggregate(RawAggregate raw) => new(
            Name: raw.Name.Trim(),
            Snapshot: raw.Snapshot.Trim(),
            CreateParams: raw.CreateParams.OrderBy(f => f.Name, StringComparer.Ordinal).Select(MapField).ToImmutableArray(),
            Events: raw.Events.OrderBy(e => e, StringComparer.Ordinal).ToImmutableArray()
        );

        private static SnapshotIntent MapSnapshot(RawSnapshot raw) => new(
            Name: raw.Name.Trim(),
            Schema: raw.Schema.Trim(),
            Rows: raw.Rows.Trim(),
            VersionField: raw.VersionField?.Trim() ?? "Version",
            StagedKey: raw.StagedKey?.Trim() ?? "Key",
            StagedValue: raw.StagedValue?.Trim() ?? "Value"
        );

        private static EventIntent MapEvent(RawEvent raw) => new(
            Name: raw.Name.Trim(),
            Params: raw.Params.OrderBy(f => f.Name, StringComparer.Ordinal).Select(MapField).ToImmutableArray()
        );

        private static RepositoryIntent MapRepository(RawRepository raw) => new(
            Name: raw.Name.Trim(),
            AggregateType: raw.AggregateType.Trim(),
            IdType: raw.IdType?.Trim() ?? $"{raw.AggregateType}Id",
            Methods: raw.Methods?.OrderBy(m => m.Name, StringComparer.Ordinal).Select(MapMethod).ToImmutableArray()
                     ?? ImmutableArray<MethodIntent>.Empty
        );

        private static CollectionIntent MapCollection(RawCollection raw) => new(
            Name: raw.Name.Trim(),
            ItemType: raw.ItemType.Trim(),
            Methods: raw.Methods?.OrderBy(m => m, StringComparer.Ordinal).ToImmutableArray() ?? ImmutableArray<string>.Empty
        );

        private static FieldIntent MapField(RawField raw) => new(
            Name: raw.Name.Trim(),
            Type: NormalizeType(raw.Type),
            Nullable: raw.Nullable,
            Normalize: raw.Normalize?.Trim(),
            DefaultValue: raw.Default?.ToString() // Simple string mapping for default values
        );

        private static PropertyIntent MapProperty(RawProperty raw) => new(
            Name: raw.Name.Trim(),
            Type: NormalizeType(raw.Type),
            Nullable: raw.Nullable,
            DefaultValue: raw.Default?.ToString()
        );

        private static MethodIntent MapMethod(RawMethod raw) => new(
            Name: raw.Name.Trim(),
            Signature: raw.Signature.Trim()
        );

        private static string NormalizeType(string type)
        {
            var t = type.Trim();
            // Basic C# type normalization could go here (e.g. Int32 -> int)
            // For Phase 1, trimming is sufficient.
            return t;
        }

        // --- Raw DTOs (Internal serialization targets) ---
        // These match the YAML structure but are loose/mutable for the deserializer.
        private sealed class RawDomainModel
        {
            public string ModelVersion { get; set; } = "";
            public Dictionary<string, string>? Metadata { get; set; }
            public Dictionary<string, string>? TemplateOptions { get; set; }
            public List<RawValueObject> ValueObjects { get; set; } = new();
            public List<RawEntity> Entities { get; set; } = new();
            public List<RawAggregate> Aggregates { get; set; } = new();
            public List<RawSnapshot> Snapshots { get; set; } = new();
            public List<RawEvent> Events { get; set; } = new();
            public List<RawRepository> Repositories { get; set; } = new();
            public List<RawCollection> Collections { get; set; } = new();
        }

        private sealed class RawValueObject { public string Name { get; set; } = ""; public string? Kind { get; set; } public List<RawField> Fields { get; set; } = new(); }
        private sealed class RawEntity { public string Name { get; set; } = ""; public string Id { get; set; } = ""; public string? IdType { get; set; } public List<RawProperty> Properties { get; set; } = new(); public List<RawField>? FactoryParams { get; set; } }
        private sealed class RawAggregate { public string Name { get; set; } = ""; public string Snapshot { get; set; } = ""; public List<RawField> CreateParams { get; set; } = new(); public List<string> Events { get; set; } = new(); }
        private sealed class RawSnapshot { public string Name { get; set; } = ""; public string Schema { get; set; } = ""; public string Rows { get; set; } = ""; public string? VersionField { get; set; } public string? StagedKey { get; set; } public string? StagedValue { get; set; } }
        private sealed class RawEvent { public string Name { get; set; } = ""; public List<RawField> Params { get; set; } = new(); }
        private sealed class RawRepository { public string Name { get; set; } = ""; public string AggregateType { get; set; } = ""; public string? IdType { get; set; } public List<RawMethod>? Methods { get; set; } }
        private sealed class RawCollection { public string Name { get; set; } = ""; public string ItemType { get; set; } = ""; public List<string>? Methods { get; set; } }
        private sealed class RawField { public string Name { get; set; } = ""; public string Type { get; set; } = ""; public bool Nullable { get; set; } public string? Normalize { get; set; } public object? Default { get; set; } }
        private sealed class RawProperty { public string Name { get; set; } = ""; public string Type { get; set; } = ""; public bool Nullable { get; set; } public object? Default { get; set; } }
        private sealed class RawMethod { public string Name { get; set; } = ""; public string Signature { get; set; } = ""; }
    }
}
