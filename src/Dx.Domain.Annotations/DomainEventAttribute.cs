// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DomainEventAttribute.cs" company="Dx.Domain Team">
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
    /// Marks a class as a Domain Event (pure metadata marker).
    /// </summary>
    /// <remarks>
    /// This attribute imposes no runtime semantics. Analyzers classify event types
    /// for naming/versioning/structure checks. See the Kernel specification for facts and
    /// structural history, and the event/fact structure rule charter.
    ///
    /// <para><b>Example (Kernel realization, non‑prescriptive):</b></para>
    /// <code><![CDATA[
    /// [DomainEvent]
    /// public sealed class OrderSubmitted
    /// {
    ///     public OrderId OrderId { get; init; }
    ///     public DateTimeOffset OccurredAt { get; init; }
    /// }
    /// ]]></code>
    /// 
    /// <para><b>Example (Usage, non‑prescriptive):</b></para>
    /// <code><![CDATA[
    /// // Producing a structural event/fact
    /// var evt = new OrderSubmitted { OrderId = id, OccurredAt = DateTimeOffset.UtcNow };
    /// // The meaning/dispatch is outside the Kernel; analyzers ensure structure only
    /// ]]></code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DomainEventAttribute : Attribute { }
}
