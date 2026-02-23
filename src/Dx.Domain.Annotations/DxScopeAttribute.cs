// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Declares the architectural scope of an assembly for Dx.Domain analyzer enforcement.
/// </summary>
/// <remarks>
/// <para>
/// Apply this attribute at the assembly level to specify which scope rules apply.
/// If not specified, the assembly defaults to <see cref="Scope.S3"/> (strictest enforcement).
/// </para>
/// <example>
/// <code>
/// [assembly: DxScope(Scope.S1)]
/// </code>
/// </example>
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class DxScopeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DxScopeAttribute"/> class.
    /// </summary>
    /// <param name="scope">The architectural scope of this assembly.</param>
    public DxScopeAttribute(Scope scope)
    {
        Scope = scope;
    }

    /// <summary>
    /// Gets the architectural scope of the assembly.
    /// </summary>
    public Scope Scope { get; }
}
