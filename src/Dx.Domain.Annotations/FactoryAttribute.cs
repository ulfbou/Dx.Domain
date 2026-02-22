// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks a method as a domain factory, indicating it creates domain entities or value objects.
/// </summary>
/// <remarks>
/// <para>
/// Factory methods are the approved construction mechanism for domain types.
/// They must return <c>Result&lt;T&gt;</c> and perform invariant validation.
/// </para>
/// <para>
/// Enforced by DXA010 (Construction Authority) and DXA011 (Public Factory Exposure).
/// </para>
/// <example>
/// <code>
/// [Factory]
/// public static Result&lt;Order&gt; CreateOrder(CustomerId customerId, ...)
/// {
///     Require.That(customerId.IsValid(), "Invalid customer");
///     return Result.Success(new Order(customerId, ...));
/// }
/// </code>
/// </example>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class FactoryAttribute : Attribute
{
}
