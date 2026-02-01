// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
