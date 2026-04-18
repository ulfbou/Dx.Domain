// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
