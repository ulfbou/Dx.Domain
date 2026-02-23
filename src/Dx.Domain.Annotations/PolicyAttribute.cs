// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Dx.Domain.Annotations;

/// <summary>
/// Marks a class as implementing domain policy logic.
/// </summary>
/// <remarks>
/// <para>
/// Policy classes encapsulate domain rules and business logic in a reusable,
/// testable form. They are structural, not infrastructural.
/// </para>
/// <para>
/// Policies must be pure (no side effects) and return <c>Result&lt;T&gt;</c>
/// to indicate success or failure.
/// </para>
/// <example>
/// <code>
/// [Policy]
/// public class OrderValidationPolicy
/// {
///     public Result Validate(Order order)
///     {
///         // Pure validation logic
///     }
/// }
/// </code>
/// </example>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class PolicyAttribute : Attribute
{
}
