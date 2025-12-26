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

using System.Collections.Immutable;

namespace Dx.Domain.Generators.Model
{
    /// <summary>
    /// The root of the Domain Intent Model (DIM).
    /// Represents the canonicalized, immutable state of a domain definition file.
    /// This is the source of truth for all downstream generators.
    /// </summary>
    public sealed record DomainIntentModel
    {
        public string ModelVersion { get; init; } = "1.0";

        // Metadata is flattened and sorted by key to ensure deterministic hashing
        public ImmutableDictionary<string, string> Metadata { get; init; } = ImmutableDictionary<string, string>.Empty;

        public ImmutableArray<ValueObjectIntent> ValueObjects { get; init; } = ImmutableArray<ValueObjectIntent>.Empty;
        public ImmutableArray<EntityIntent> Entities { get; init; } = ImmutableArray<EntityIntent>.Empty;
        public ImmutableArray<AggregateIntent> Aggregates { get; init; } = ImmutableArray<AggregateIntent>.Empty;
        public ImmutableArray<SnapshotIntent> Snapshots { get; init; } = ImmutableArray<SnapshotIntent>.Empty;
        public ImmutableArray<EventIntent> Events { get; init; } = ImmutableArray<EventIntent>.Empty;
        public ImmutableArray<RepositoryIntent> Repositories { get; init; } = ImmutableArray<RepositoryIntent>.Empty;
        public ImmutableArray<CollectionIntent> Collections { get; init; } = ImmutableArray<CollectionIntent>.Empty;
    }

    public sealed record ValueObjectIntent(string Name, string Kind, ImmutableArray<FieldIntent> Fields);

    public sealed record EntityIntent(string Name, string Id, string IdType, ImmutableArray<PropertyIntent> Properties, ImmutableArray<FieldIntent> FactoryParams);

    public sealed record AggregateIntent(string Name, string Snapshot, ImmutableArray<FieldIntent> CreateParams, ImmutableArray<string> Events);

    public sealed record SnapshotIntent(string Name, string Schema, string Rows, string VersionField, string StagedKey, string StagedValue);

    public sealed record EventIntent(string Name, ImmutableArray<FieldIntent> Params);

    public sealed record RepositoryIntent(string Name, string AggregateType, string IdType, ImmutableArray<MethodIntent> Methods);

    public sealed record CollectionIntent(string Name, string ItemType, ImmutableArray<string> Methods);

    // Shared Shapes
    public sealed record FieldIntent(string Name, string Type, bool Nullable, string? Normalize, string? DefaultValue);
    public sealed record PropertyIntent(string Name, string Type, bool Nullable, string? DefaultValue);
    public sealed record MethodIntent(string Name, string Signature);
}
