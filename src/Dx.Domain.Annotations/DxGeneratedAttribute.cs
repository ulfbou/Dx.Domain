// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks code as generated, exempting it from Dx.Domain analyzer enforcement.
/// </summary>
/// <remarks>
/// <para>
/// Use this attribute when <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>
/// is not feasible (e.g., source generators targeting older frameworks).
/// </para>
/// <para>
/// Recognized by DXA070 (Generated Code Tagging) and other analyzers that skip generated code.
/// </para>
/// <example>
/// <code>
/// [DxGenerated]
/// public partial class GeneratedEntity
/// {
///     // Auto-generated property implementations
/// }
/// </code>
/// </example>
/// </remarks>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method |
    AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Field,
    AllowMultiple = false,
    Inherited = false)]
public sealed class DxGeneratedAttribute : Attribute
{
}
