// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="PolicyAttribute.cs" company="Dx.Domain Team">
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
