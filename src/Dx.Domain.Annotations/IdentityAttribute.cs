// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
