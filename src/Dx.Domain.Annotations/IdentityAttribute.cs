// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IdentityAttribute.cs" company="Dx.Domain Team">
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
/// Marks a type as a domain identity primitive (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute imposes no runtime semantics. Analyzers classify identity intent; the
/// *normative requirements* are defined in the Kernel specification and the identity
/// discipline rule charter. See the Kernel specification for identity primitives and the
/// rule charter for identity discipline.
///
/// <para><b>Example (Kernel realization, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// [Identity]
/// public readonly struct OrderId : IIdentity
/// {
///     private readonly Guid _value;
///     private OrderId(Guid value) => _value = value;
///     public static Result<OrderId> Create(Guid value) => /* guarded creation */;
/// }
/// ]]></code>
/// 
/// <para><b>Example (Usage, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// var maybeId = OrderId.Create(guid);
/// if (maybeId.IsFailure) return maybeId.Error;
/// OrderId id = maybeId.Value;
/// ]]></code>
/// </remarks>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class IdentityAttribute : Attribute
{
}
