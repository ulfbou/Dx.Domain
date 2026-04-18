// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
