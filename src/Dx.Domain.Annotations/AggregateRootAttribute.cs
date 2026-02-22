// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AggregateRootAttribute.cs" company="Dx.Domain Team">
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
    /// Marks a class as an <b>Aggregate Root</b> in the domain model.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute is a <b>pure marker</b>. It enables analyzers/generators to enforce
    /// aggregate rules (e.g., aggregate boundaries, ownership of identity, reference constraints).
    /// </para>
    /// <para>
    /// <b>Usage:</b> Apply to the single type that defines the aggregate boundary and
    /// acts as the entry point for modifications to the aggregate.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AggregateRootAttribute : Attribute { }
}
