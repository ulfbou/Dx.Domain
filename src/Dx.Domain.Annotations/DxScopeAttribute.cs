// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
