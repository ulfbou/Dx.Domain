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
    /// <remarks>
    /// Aggregate roots are entry points for modifying and retrieving aggregate data.
    /// This interface is a pure semantic marker and defines no members.
    /// </remarks>
    public interface IAggregateRoot { }

    /// <summary>
    /// Defines a contract for an entity type that participates in the domain model.
    /// </summary>
    /// <remarks>
    /// This interface is a pure semantic marker and defines no members.
    /// </remarks>
    public interface IEntity { }

    /// <summary>
    /// Represents a domain event that signals a significant change in the domain model.
    /// </summary>
    /// <remarks>
    /// This interface is a pure semantic marker and defines no members.
    /// </remarks>
    public interface IDomainEvent { }

    /// <summary>
    /// Defines a marker interface for value objects, which are compared by value.
    /// </summary>
    /// <remarks>
    /// This interface is a pure semantic marker and defines no members.
    /// </remarks>
    public interface IValueObject { }
}
