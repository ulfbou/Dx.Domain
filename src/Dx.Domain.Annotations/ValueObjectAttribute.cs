// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ValueObjectAttribute.cs" company="Dx.Domain Team">
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
    /// Marks a type as a <b>Value Object</b>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute is a <b>pure marker</b>. Typical expectations (checked by analyzers)
    /// include immutability and equality by value rather than identity.
    /// </para>
    /// <para>
    /// <b>Non-goals:</b> This attribute does not implement or enforce equality or immutability at runtime.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ValueObjectAttribute : Attribute { }
}
