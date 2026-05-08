// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxGeneratedAttribute.cs" company="Dx.Domain Team">
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
/// Marks code as generated (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute imposes no runtime semantics. It is recognized by analyzers that
/// skip generated code (e.g., DXA070). Use it when
/// <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/> is not feasible.
///
/// <para><b>Example (non‑prescriptive):</b></para>
/// <code><![CDATA[
/// [DxGenerated]
/// public partial class GeneratedEntity
/// {
///     // Auto-generated property implementations
/// }
/// ]]></code>
/// </remarks>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method |
    AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Field,
    AllowMultiple = false,
    Inherited = false)]
public sealed class DxGeneratedAttribute : Attribute
{
}
