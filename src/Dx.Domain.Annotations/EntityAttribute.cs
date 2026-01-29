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

namespace Dx.Domain
{
    /// <summary>
    /// Marks a class as a domain <b>Entity</b>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute is a <b>pure marker</b> used by analyzers and generators to enforce
    /// domain modeling rules (e.g., identity semantics). It has <b>no runtime behavior</b>.
    /// </para>
    /// <para>
    /// <b>Non-goals:</b> This attribute does not imply persistence mapping, lifecycle, or
    /// infrastructure concerns. Those are out of scope and should be handled outside the domain model.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class EntityAttribute : Attribute { }
}
