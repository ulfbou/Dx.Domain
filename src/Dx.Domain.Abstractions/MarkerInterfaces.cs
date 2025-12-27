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
    // -------------------------------------------------------------------------
    // Marker Interfaces
    // -------------------------------------------------------------------------
    // Pure contracts with trivial or no shape. 
    // Used for generic constraints and analyzer detection.

    /// <summary>
    /// Marker interface for Aggregate Roots.
    /// </summary>
    public interface IAggregateRoot : IEntity { }

    /// <summary>
    /// Marker interface for Entities.
    /// </summary>
    public interface IEntity { }

    /// <summary>
    /// Marker interface for Domain Events.
    /// </summary>
    public interface IDomainEvent { }

    /// <summary>
    /// Marker interface for Value Objects.
    /// </summary>
    public interface IValueObject { }
}
