// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxFacadeAttribute.cs" company="Dx.Domain Team">
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
/// Marks a class as an approved domain facade boundary (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute imposes no runtime semantics. Analyzers classify construction boundaries
/// per DXA010/011/080. See Rule Charter → DXA010 Construction Discipline.
///
/// <para><b>Example (Kernel realization, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// [DxFacade]
/// public static class OrderFacade
/// {
///     public static Result<Order> CreateOrder(CustomerId customerId,...)
///     {
///         // boundary validation + construction
///         return Result.Success(new Order(customerId,...));
///     }
/// }
/// ]]></code>
///
/// <para><b>Example (Usage, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// var created = OrderFacade.CreateOrder(customerId,...);
/// return created.Match(
///     onSuccess: o => Ok(o),
///     onFailure: e => BadRequest(e.Code));
/// ]]></code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DxFacadeAttribute : Attribute
{
}
