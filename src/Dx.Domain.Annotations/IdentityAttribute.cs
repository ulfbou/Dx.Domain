// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks a type as a domain identity primitive.
/// </summary>
/// <remarks>
/// <para>
/// Identity types must be <c>readonly struct</c>, implement <c>IIdentity</c>,
/// have no public constructors, and use guarded creation only.
/// </para>
/// <para>
/// Enforced by DXA030 (Identity violations) and related analyzers.
/// </para>
/// <example>
/// <code>
/// [Identity]
/// public readonly struct OrderId : IIdentity
/// {
///     private readonly Guid _value;
///     
///     private OrderId(Guid value) => _value = value;
///     
///     public static Result&lt;OrderId&gt; Create(Guid value) => ...;
/// }
/// </code>
/// </example>
/// </remarks>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class IdentityAttribute : Attribute
{
}
