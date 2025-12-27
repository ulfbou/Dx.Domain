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

namespace Dx.Domain.Attributes
{
    // -------------------------------------------------------------------------
    // Semantic Attributes (The Contract)
    // -------------------------------------------------------------------------
    // These attributes drive the Analyzers and Generators. They contain no logic.

    /// <summary>
    /// Indicates that the attributed class is an aggregate root in a domain-driven design context.
    /// </summary>
    /// <remarks>Apply this attribute to a class to designate it as the root entity of an aggregate. Aggregate
    /// roots are responsible for maintaining the consistency of changes within the aggregate and serve as the primary
    /// entry point for interacting with related entities. This attribute is intended for use by code analyzers and
    /// source generators and does not affect runtime behavior.</remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AggregateRootAttribute : Attribute { }

    /// <summary>
    /// Specifies that a class represents a data entity for use with an object-relational mapping framework or data
    /// access layer.
    /// </summary>
    /// <remarks>Apply this attribute to a class to indicate that it should be treated as a persistent entity.
    /// This attribute is not inherited by derived classes.</remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class EntityAttribute : Attribute { }

    /// <summary>
    /// Indicates that a class or struct represents a value object, which is defined by its value rather than its
    /// identity.
    /// </summary>
    /// <remarks>Apply this attribute to types that should be treated as value objects, typically in the
    /// context of domain-driven design. Value objects are immutable and considered equal if all their properties are
    /// equal. This attribute is intended for use by frameworks or tools that recognize value object
    /// semantics.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public sealed class ValueObjectAttribute : Attribute { }

    /// <summary>
    /// Specifies that a class represents a domain event in the domain-driven design pattern.
    /// </summary>
    /// <remarks>Apply this attribute to classes that define domain events to enable identification and
    /// processing of such events within the application. This attribute is not inherited by derived classes.</remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DomainEventAttribute : Attribute { }

    /// <summary>
    /// Indicates that a method represents an invariant condition for a class or type.
    /// </summary>
    /// <remarks>Apply this attribute to methods that define invariants, which are conditions that must always
    /// hold true for the type. Invariant methods are typically used in code contracts or static analysis tools to
    /// specify the consistent state of an object. This attribute is not inherited by derived methods.</remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class InvariantAttribute : Attribute { }

    /// <summary>
    /// Indicates that a method is a factory method used to create instances of a type.
    /// </summary>
    /// <remarks>Apply this attribute to methods that are intended to serve as factories, typically for use
    /// with frameworks or tools that recognize factory patterns. The attribute can only be applied to methods and is
    /// not inherited by derived classes.</remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class FactoryAttribute : Attribute { }

    /// <summary>
    /// Specifies that a method represents a command that can be invoked by a command framework or dispatcher.
    /// </summary>
    /// <remarks>Apply this attribute to methods to indicate that they should be treated as commands, such as
    /// in command-line interfaces, scripting engines, or custom command dispatchers. This attribute is not inherited
    /// and cannot be applied multiple times to the same method.</remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute { }

    /// <summary>
    /// Indicates that a property represents an identity or primary key value.
    /// </summary>
    /// <remarks>Apply this attribute to a property to designate it as the unique identifier for an entity,
    /// such as a primary key in a database model. This attribute is typically used by frameworks or libraries that
    /// require knowledge of identity properties for operations like persistence or object tracking.</remarks>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class IdentityAttribute : Attribute { }
}
