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

namespace Dx.Domain.Metadata
{
    public sealed record AggregateMetadata(
        string Name,
        ImmutableArray<string> Entities,
        ImmutableArray<string> ValueObjects,
        ImmutableArray<string> Invariants);
    public sealed record EntityMetadata(
        string Name,
        ImmutableArray<string> Properties,
        string? IdentityProperty);
    public sealed record ValueObjectMetadata(
        string Name,
        ImmutableArray<string> Components);
    public sealed record DomainEventMetadata(
        string Name,
        ImmutableArray<string> PayloadProperties);
}
