// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FactoryAttribute.cs" company="Dx.Domain Team">
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
