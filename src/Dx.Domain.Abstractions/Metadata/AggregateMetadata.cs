// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AggregateMetadata.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Metadata
{
    /// <summary>
    /// Versioned metadata describing an aggregate. Passive DTO only.
    /// </summary>
    // DPI: Immutable data structure representing aggregate metadata for tooling and analysis.
    public sealed record AggregateMetadata(
        string Name,
        ImmutableArray<string> Entities,
        ImmutableArray<string> ValueObjects,
        ImmutableArray<string> Invariants,
        string SchemaVersion);

    /// <summary>
    /// Metadata describing an entity. Passive DTO only.
    /// </summary>
    // DPI: Immutable data structure representing entity metadata for tooling and analysis.
    public sealed record EntityMetadata(
        string Name,
        ImmutableArray<string> Properties,
        string? IdentityProperty,
        string SchemaVersion);

    /// <summary>
    /// Metadata describing a value object. Passive DTO only.
    /// </summary>
    // DPI: Immutable data structure representing value object metadata for tooling and analysis.
    public sealed record ValueObjectMetadata(
        string Name,
        ImmutableArray<string> Components,
        string SchemaVersion);

    /// <summary>
    /// Metadata describing a domain event. Passive DTO only.
    /// </summary>
    // DPI: Immutable data structure representing domain event metadata for tooling and analysis.
    public sealed record DomainEventMetadata(
        string Name,
        ImmutableArray<string> PayloadProperties,
        string SchemaVersion);
}
