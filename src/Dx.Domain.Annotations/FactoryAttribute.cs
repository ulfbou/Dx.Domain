// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks a method as a domain factory (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute imposes no runtime semantics. Analyzers classify factory methods
/// for construction discipline (DXA010/011). SEE: Rule Charter → DXA010 Construction Discipline.
///
/// <para><b>Example (Kernel realization, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// [Factory]
/// public static Result<Order> CreateOrder(CustomerId customerId, ...)
/// {
///     // validation + construction
///     return Result.Success(new Order(customerId, ...));
/// }
/// ]]></code>
/// 
/// <para><b>Example (Usage, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// var created = OrderFactory.CreateOrder(customerId, ...);
/// if (created.IsFailure) return created.Error;
/// var order = created.Value;
/// ]]></code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class FactoryAttribute : Attribute
{
}
