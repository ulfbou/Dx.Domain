// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Documents a deviation from standard patterns with a DPI‑aligned justification (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute imposes no runtime semantics; it records rationale for code review
/// and audit trails. SEE: Governance Docs → DPI / ADR Process.
///
/// <para><b>Example (non‑prescriptive):</b></para>
/// <code><![CDATA[
/// [DpiJustified("Mutable internal cache is mechanical optimization, not semantic state")]
/// private Dictionary<string, CompiledPattern> _patternCache;
/// ]]></code>
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
