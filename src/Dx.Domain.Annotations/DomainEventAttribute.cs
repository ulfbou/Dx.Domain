// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DomainEventAttribute.cs" company="Dx.Domain Team">
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
    /// Marks a class as a <b>Domain Event</b>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute is a <b>pure marker</b> that may be used by analyzers/generators to drive
    /// event naming, versioning, and structure checks. It has <b>no runtime behavior</b>.
    /// </para>
    /// <para>
    /// <b>Usage:</b> Apply to types that represent something that has occurred within the domain and
    /// may be of interest to other parts of the system.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DomainEventAttribute : Attribute { }
}
