// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AggregateKind.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Symbols
{
    /// <summary>
    /// Specifies the kind of aggregate in a domain-driven design context.
    /// </summary>
    /// <remarks>Use this enumeration to distinguish between different aggregate types, such as root
    /// aggregates, entities, and value objects. The value <see cref="AggregateKind.Unknown"/> indicates that the
    /// aggregate kind is not specified.</remarks>
    public enum AggregateKind
    {
        /// <summary>
        /// Represents an unspecified or unrecognized value.
        /// </summary>
        /// <remarks>Use this value when the actual value is unknown or cannot be determined. It is
        /// commonly used as a default or fallback in scenarios where a valid value is required but not
        /// available.</remarks>
        Unknown = 0,

        /// <summary>
        /// Specifies that the item represents the root element in a hierarchy or structure.
        /// </summary>
        Root = 1,

        /// <summary>
        /// Represents an entity node type in the XML document hierarchy.
        /// </summary>
        /// <remarks>Use this value to identify nodes that correspond to XML entities, such as those
        /// defined with an entity declaration. Entity nodes are typically used for representing reusable content or
        /// references within an XML document.</remarks>
        Entity = 2,

        /// <summary>
        /// Represents a value object, which is defined by its properties rather than a distinct identity.
        /// </summary>
        /// <remarks>Value objects are typically immutable and are considered equal if all their property
        /// values are equal. Use value objects to model concepts that do not require unique identification.</remarks>
        ValueObject = 3
    }
}
