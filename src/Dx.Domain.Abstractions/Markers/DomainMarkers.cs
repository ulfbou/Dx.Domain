// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="MarkerInterfaces.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain
{
    /// <summary>
    /// Represents the root entity of an aggregate in a domain-driven design context.
    /// </summary>
    /// <remarks>Aggregate roots are entry points for modifying and retrieving aggregate data. Only aggregate
    /// roots should be referenced from outside the aggregate to ensure consistency and encapsulation. This interface is
    /// typically used as a marker for generic constraints and domain modeling.</remarks>
    public interface IAggregateRoot : IEntity { }

    /// <summary>
    /// Defines a contract for an entity type that can be used as a base for domain or data model objects.
    /// </summary>
    /// <remarks>Implement this interface to indicate that a class represents an entity within a domain model or
    /// persistence layer. This interface is commonly used as a marker to enable generic handling of entities in
    /// repositories, services, or frameworks.</remarks>
    public interface IEntity { }

    /// <summary>
    /// Represents a domain event that signals a significant change or occurrence within the domain model.
    /// </summary>
    /// <remarks>Domain events are used to capture and communicate important business events that have occurred within
    /// the system. Implement this interface to define custom events that can be handled by other components, such as event
    /// handlers or message dispatchers. Domain events are typically used in domain-driven design (DDD) to decouple event
    /// producers from consumers and to enable eventual consistency across bounded contexts.</remarks>
    public interface IDomainEvent { }

    /// <summary>
    /// Defines a marker interface for value objects, which are compared based on their values rather than their
    /// identities.
    /// </summary>
    /// <remarks>Implement this interface to indicate that an object represents a value in the domain model
    /// and should be considered equal to other instances with the same value. Value objects are typically immutable and
    /// do not have a distinct identity.</remarks>
    public interface IValueObject { }
}
