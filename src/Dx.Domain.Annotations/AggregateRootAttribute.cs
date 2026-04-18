// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AggregateRootAttribute.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Annotations
{
    /// <summary>
    /// Marks a class as an Aggregate Root (pure metadata marker).
    /// </summary>
    /// <remarks>
    /// This attribute imposes no runtime semantics. Analyzers classify aggregate boundaries;
    /// the normative guidance is defined in the Kernel specification and relevant rule charters.
    /// See the Kernel specification for aggregates and the aggregate discipline rule charter.
    /// 
    /// <para><b>Example (Kernel realization, non‑prescriptive):</b></para>
    /// <code><![CDATA[
    /// [AggregateRoot]
    /// public sealed class Order
    /// {
    ///     public OrderId Id { get; }
    ///     // Aggregate behavior and invariants live here
    /// }
    /// ]]></code>
    /// 
    /// <para><b>Example (Usage, non‑prescriptive):</b></para>
    /// <code><![CDATA[
    /// // Aggregate used via facade/factory (simplified)
    /// var created = OrderFacade.CreateOrder(customerId, ...);
    /// if (created.IsFailure) return created.Error;
    /// var order = created.Value;
    /// ]]></code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AggregateRootAttribute : Attribute { }
}
