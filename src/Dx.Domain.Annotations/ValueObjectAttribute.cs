// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ValueObjectAttribute.cs" company="Dx.Domain Team">
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
    /// Marks a type as a Value Object (pure metadata marker).
    /// </summary>
    /// <remarks>
    /// This attribute imposes no runtime semantics. Analyzers classify value objects for
    /// immutability and value‑equality checks. See the Kernel specification for value
    /// objects and the value object discipline rule charter.
    /// 
    /// <para><b>Example (Kernel realization, non‑prescriptive):</b></para>
    /// <code><![CDATA[
    /// [ValueObject]
    /// public readonly struct Money
    /// {
    ///     public decimal Amount { get; }
    ///     public string Currency { get; }
    ///     public Money(decimal amount, string currency) { Amount = amount; Currency = currency; }
    /// }
    /// ]]></code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ValueObjectAttribute : Attribute { }
}
