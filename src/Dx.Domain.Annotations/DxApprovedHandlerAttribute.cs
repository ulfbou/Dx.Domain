// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxApprovedHandlerAttribute.cs" company="Dx.Domain Team">
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
/// Marks a method as an approved Result handler (pure metadata marker).
/// </summary>
/// <remarks>
/// This attribute imposes no runtime semantics. Analyzers classify approved handlers
/// for Result flow. See the result handling rule charter.
///
/// <para><b>Example (Kernel realization, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// [DxApprovedHandler]
/// public static Result<T> MapErrors<T>(Result<T> result, Func<DomainError, DomainError> map)
///     => result.IsFailure ? Result.Failure<T>(map(result.Error)) : result;
/// ]]></code>
/// 
/// <para><b>Example (Usage, non‑prescriptive):</b></para>
/// <code><![CDATA[
/// var handled = ResultHandlers.MapErrors(result, e => e.WithCode("normalized"));
/// ]]></code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class DxApprovedHandlerAttribute : Attribute
{
}
