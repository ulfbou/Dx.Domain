// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright>
//   © 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//   MIT; see LICENSE in repository root.
// </license>
// ----------------------------------------------------------------------------------

using System;

namespace Dx.Domain.Annotations
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
