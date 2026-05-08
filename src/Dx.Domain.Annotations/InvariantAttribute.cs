// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="InvariantAttribute.cs" company="Dx.Domain Team">
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
