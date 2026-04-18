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

namespace Dx.Domain.Annotations
{
    /// <summary>
    /// Marks a class as a domain Entity (pure metadata marker).
    /// </summary>
    /// <remarks>
    /// This attribute imposes no runtime semantics. Analyzers classify entity types
    /// for identity and modeling checks. See the Kernel specification for entities and
    /// the entity discipline rule charter.
    /// 
    /// <para><b>Example (Kernel realization, non‑prescriptive):</b></para>
    /// <code><![CDATA[
    /// [Entity]
    /// public sealed class LineItem
    /// {
    ///     public LineItemId Id { get; }
    ///     public Money Price { get; }
    /// }
    /// ]]></code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class EntityAttribute : Attribute { }
}
