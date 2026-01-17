// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="SemanticAttributes.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;

namespace Dx.Domain.Attributes
{
    // -------------------------------------------------------------------------
    // Semantic Attributes (The Contract)
    // -------------------------------------------------------------------------
    // These attributes drive the Analyzers and Generators. They contain no logic.

    /// <summary>
    /// Marks an aggregate root. Marker only; no runtime semantics.
    /// </summary>
    // DPI: Marker attribute to identify aggregate root classes in domain-driven design.
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AggregateRootAttribute : Attribute { }

    /// <summary>
    /// Marks a value object. Marker only; no runtime semantics.
    /// </summary>
    // DPI: Marker attribute to identify value object types in domain-driven design.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public sealed class ValueObjectAttribute : Attribute { }

    // DPI: Marker attribute to identify domain event classes in domain-driven design.
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DomainEventAttribute : Attribute { }

    /// <summary>
    /// Marks an invariant method. Marker only; analyzers use this to locate invariants.
    /// </summary>
    // DPI: Marker attribute to identify methods that enforce domain invariants.
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class InvariantAttribute : Attribute { }

    /// <summary>
    /// Marks a factory method. Marker only; analyzers/generators use this.
    /// </summary>
    // DPI: Marker attribute to identify factory methods that create instances of aggregate roots or other domain objects.
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class FactoryAttribute : Attribute { }

    /// <summary>
    /// Marks a property as the identity of an entity.
    /// </summary>
    // DPI: Marker attribute to designate the identity property of an entity in domain-driven design.
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class IdentityAttribute : Attribute { }
}
