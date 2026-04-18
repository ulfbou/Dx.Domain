// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks a class as a domain policy (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute imposes no runtime semantics. Analyzers classify policy types
/// for structural checks. See the Kernel specification for policy structure and
/// the policy discipline rule charter.
///
/// <para><b>Example (Kernel realization, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// [Policy]
/// public class OrderValidationPolicy
/// {
///     public Result Validate(Order order) => /* pure validation logic */;
/// }
/// ]]></code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class PolicyAttribute : Attribute
{
}
