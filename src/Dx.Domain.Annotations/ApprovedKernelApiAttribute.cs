// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Justifies a new public API addition to the Dx.Domain kernel, exempting it from DXA040 errors.
/// </summary>
/// <remarks>
/// <para>
/// The kernel public surface is frozen. Any new public API must carry this attribute
/// with a DPI-aligned justification explaining why the addition is necessary and structural.
/// </para>
/// <para>
/// Enforced by DXA040 (Kernel Public Surface Freeze) and API baseline tooling.
/// </para>
/// <example>
/// <code>
/// [ApprovedKernelApi("DPI: Adding SpanId for distributed tracing correlation, structural primitive")]
/// public readonly struct SpanId : IIdentity
/// {
///     // ...
/// }
/// </code>
/// </example>
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
    /// <param name="justification">
    /// DPI-aligned justification for the new API. Must explain structural necessity.
    /// </param>
    public ApprovedKernelApiAttribute(string justification)
    {
        Justification = justification ?? throw new ArgumentNullException(nameof(justification));
    }

    /// <summary>
    /// Gets the DPI-aligned justification for this API addition.
    /// </summary>
    public string Justification { get; }
}
