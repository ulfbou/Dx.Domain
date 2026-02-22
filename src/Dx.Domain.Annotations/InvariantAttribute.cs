// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks a method or class as enforcing domain invariants.
/// </summary>
/// <remarks>
/// <para>
/// This is a semantic marker for methods that perform invariant checking
/// using <c>Invariant.That(...)</c> or <c>Require.That(...)</c>.
/// </para>
/// <para>
/// Used by analyzers to identify invariant enforcement points and verify
/// proper error handling patterns.
/// </para>
/// </remarks>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct,
    AllowMultiple = false,
    Inherited = false)]
public sealed class InvariantAttribute : Attribute
{
}
