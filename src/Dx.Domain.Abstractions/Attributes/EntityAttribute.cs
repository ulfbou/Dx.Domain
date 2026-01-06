// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="EntityAttribute.cs" company="Dx.Domain Team">
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
    /// <summary>
    /// Specifies that a class represents a data entity for use with an object-relational mapping framework or data
    /// access layer.
    /// </summary>
    /// <remarks>Apply this attribute to a class to indicate that it should be treated as a persistent entity.
    /// This attribute is not inherited by derived classes.</remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class EntityAttribute : Attribute { }
}
