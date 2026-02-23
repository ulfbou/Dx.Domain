// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks a method as an approved handler for Result types, exempting it from DXA030 warnings.
/// </summary>
/// <remarks>
/// <para>
/// Apply this attribute to methods that legitimately consume Result types as part of
/// their contract (e.g., result mappers, combinators, validation pipelines).
/// </para>
/// <para>
/// Prevents DXA030 (Unapproved Handler Usage) from flagging the method.
/// </para>
/// <example>
/// <code>
/// [DxApprovedHandler]
/// public static Result&lt;T&gt; MapErrors&lt;T&gt;(Result&lt;T&gt; result, Func&lt;DomainError, DomainError&gt; mapper)
/// {
///     return result.IsFailure
///         ? Result.Failure&lt;T&gt;(mapper(result.Error))
///         : result;
/// }
/// </code>
/// </example>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class DxApprovedHandlerAttribute : Attribute
{
}
