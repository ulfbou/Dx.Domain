// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxScopeAttribute.cs" company="Dx.Domain Team">
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
/// Declares the architectural scope of an assembly (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute imposes no runtime semantics; analyzers use it to classify scope
/// for enforcement. If not specified, scope defaults to <see cref="Scope.S3"/>.
/// SEE: Refactoring Spec → Scope Resolution; Rule Charter → Scope/Authority Modes.
///
/// <para><b>Example (non‑prescriptive):</b></para>
/// <code><![CDATA[
/// [assembly: DxScope(Scope.S1)]
/// ]]></code>
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
