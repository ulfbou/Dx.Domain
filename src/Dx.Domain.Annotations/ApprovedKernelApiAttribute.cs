// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ApprovedKernelApiAttribute.cs" company="Dx.Domain Team">
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
/// Documents a new public Kernel API with a DPI‑aligned justification (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute imposes no runtime semantics. It is used for documentation + analyzer
/// classification of Kernel API surface changes. See the kernel public surface rule charter.
///
/// <para><b>Example (non‑prescriptive):</b></para>
/// <code><![CDATA[
/// [ApprovedKernelApi("DPI: Adding SpanId as structural tracing primitive")]
/// public readonly struct SpanId : IIdentity { /* ... */ }
/// ]]></code>
/// </remarks>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface |
    AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field |
    AttributeTargets.Enum | AttributeTargets.Delegate,
    AllowMultiple = false,
    Inherited = false)]
public sealed class ApprovedKernelApiAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApprovedKernelApiAttribute"/> class.
    /// </summary>
    /// <param name="justification">DPI-aligned justification for the new API.</param>
    public ApprovedKernelApiAttribute(string justification)
    {
        Justification = justification ?? throw new ArgumentNullException(nameof(justification));
    }

    /// <summary>
    /// Gets the DPI-aligned justification for this API addition.
    /// </summary>
    public string Justification { get; }
}
