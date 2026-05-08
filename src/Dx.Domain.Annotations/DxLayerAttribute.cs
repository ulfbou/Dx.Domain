// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxLayerAttribute.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Annotations;

/// <summary>
/// Declares the architectural layer of an assembly for Dx.Domain analyzer scope resolution.
/// </summary>
/// <remarks>
/// <para>
/// Apply this attribute at the assembly level to specify which layer rules apply.
/// </para>
/// <example>
/// <code><![CDATA[
/// [assembly: DxLayer("Kernel")]
/// ]]></code>
/// </example>
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class DxLayerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DxLayerAttribute"/> class.
    /// </summary>
    /// <param name="layer">The architectural layer of the assembly.</param>
    public DxLayerAttribute(string layer)
    {
        Layer = layer;
    }

    /// <summary>
    /// Gets the architectural layer of the assembly.
    /// </summary>
    public string Layer { get; }
}
