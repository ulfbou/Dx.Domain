// ----------------------------------------------------------------------------------
// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AggregateMetadata.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Immutable;

namespace Dx.Domain.Metadata
{
    /// <summary>
    /// Represents metadata describing an aggregate, including its name, associated entities, value objects, and
    /// invariants.
    /// </summary>
    /// <param name="Name">The unique name of the aggregate. Cannot be null or empty.</param>
    /// <param name="Entities">The collection of entity type names that are part of the aggregate. May be empty if no entities are defined.</param>
    /// <param name="ValueObjects">The collection of value object type names associated with the aggregate. May be empty if no value objects are
    /// defined.</param>
    /// <param name="Invariants">The collection of invariant descriptions that must hold true for the aggregate. May be empty if no invariants
    /// are specified.</param>
    public sealed record AggregateMetadata(
        string Name,
        ImmutableArray<string> Entities,
        ImmutableArray<string> ValueObjects,
        ImmutableArray<string> Invariants);

    /// <summary>
    /// Represents metadata information for an entity, including its name, properties, and optional identity property.
    /// </summary>
    /// <param name="Name">The name of the entity. This value is typically used to identify the entity type within a data model.</param>
    /// <param name="Properties">A collection of property names that define the structure of the entity. The array must not be empty.</param>
    /// <param name="IdentityProperty">The name of the property that serves as the entity's unique identifier, or <see langword="null"/> if the entity
    /// does not have an identity property.</param>
    public sealed record EntityMetadata(
        string Name,
        ImmutableArray<string> Properties,
        string? IdentityProperty);

    /// <summary>
    /// Represents metadata describing a value object, including its name and component members.
    /// </summary>
    /// <param name="Name">The name of the value object. This is typically used to identify the type or purpose of the value object within
    /// a domain.</param>
    /// <param name="Components">An immutable array containing the names of the components that make up the value object. Each component
    /// represents a constituent part of the value object's state.</param>
    public sealed record ValueObjectMetadata(
        string Name,
        ImmutableArray<string> Components);

    /// <summary>
    /// Represents metadata describing a domain event, including its name and the set of payload property names.
    /// </summary>
    /// <param name="Name">The unique name identifying the domain event.</param>
    /// <param name="PayloadProperties">An immutable array containing the names of properties included in the event's payload.</param>
    public sealed record DomainEventMetadata(
        string Name,
        ImmutableArray<string> PayloadProperties);
}
