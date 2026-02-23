// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks a public static class as an approved facade for domain construction in S1/S2 scopes.
/// </summary>
/// <remarks>
/// <para>
/// Facade classes provide the public API for constructing domain types with proper
/// invariant enforcement. Factory methods on facades must return <c>Result&lt;T&gt;</c>
/// and perform validation.
/// </para>
/// <para>
/// Analyzed by DXA010 (Construction Authority), DXA011 (Public Factory Exposure),
/// and DXA080 (Facade Invariant Enforcement).
/// </para>
/// <example>
/// <code>
/// [DxFacade]
/// public static class OrderFacade
/// {
///     public static Result&lt;Order&gt; CreateOrder(CustomerId customerId, ...)
///     {
///         Require.That(customerId.IsValid(), "Invalid customer ID");
///         return Result.Success(new Order(customerId, ...));
///     }
/// }
/// </code>
/// </example>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DxFacadeAttribute : Attribute
{
}
