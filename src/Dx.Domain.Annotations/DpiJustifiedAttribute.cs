// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Documents a deviation from standard patterns with a DPI-aligned justification.
/// </summary>
/// <remarks>
/// <para>
/// Use this attribute to explicitly document design decisions that might appear
/// unconventional but are justified by the Dx.Domain Perturbation Index (DPI) principles.
/// </para>
/// <para>
/// This is a documentation-only attribute; it does not affect analyzer behavior
/// but aids code review and architectural decision tracking.
/// </para>
/// <example>
/// <code>
/// [DpiJustified("Mutable internal cache is mechanical optimization, not semantic state")]
/// private Dictionary&lt;string, CompiledPattern&gt; _patternCache;
/// </code>
/// </example>
/// </remarks>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public sealed class DpiJustifiedAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DpiJustifiedAttribute"/> class.
    /// </summary>
    /// <param name="reason">The DPI-aligned justification for this design decision.</param>
    public DpiJustifiedAttribute(string reason)
    {
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
    }

    /// <summary>
    /// Gets the justification for the design decision.
    /// </summary>
    public string Reason { get; }
}
