// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
