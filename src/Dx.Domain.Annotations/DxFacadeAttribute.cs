// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks a class as an approved domain facade boundary (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute does not impose return-type or behavioral contracts. Analyzers
/// classify construction boundaries per DXA010/011/080.
/// SEE: Rule Charter → DXA010 Construction Discipline.
///
/// <para><b>Example (Kernel realization, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// [DxFacade]
/// public static class OrderFacade
/// {
///     public static Result<Order> CreateOrder(CustomerId customerId, ...)
///     {
///         // boundary validation + construction
///         return Result.Success(new Order(customerId, ...));
///     }
/// }
/// ]]></code>
/// 
/// <para><b>Example (Usage, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// var created = OrderFacade.CreateOrder(customerId, ...);
/// return created.Match(
///   onSuccess: o => Ok(o),
///   onFailure: e => BadRequest(e.Code));
/// ]]></code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DxFacadeAttribute : Attribute
{
}
