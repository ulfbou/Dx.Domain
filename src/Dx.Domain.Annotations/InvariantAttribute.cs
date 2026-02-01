// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks a method or type as an invariant enforcement point (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute imposes no runtime semantics. Analyzers classify invariant checks
/// and verify error‑handling patterns. SEE: Kernel Specification → Invariants &amp; Require;
/// Rule Charter → Invariant/Require Discipline.
/// </remarks>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct,
    AllowMultiple = false,
    Inherited = false)]
public sealed class InvariantAttribute : Attribute
{
}
