// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Dx.Domain.Annotations;

/// <summary>
/// Defines scope vocabulary for Dx.Domain enforcement (pure metadata).
/// </summary>
/// <remarks>
/// This enum imposes no runtime semantics. Analyzers interpret these values according
/// to authority modes. See the refactoring spec for scope resolution and authority modes.
/// <para><b>S0 (Kernel)</b>: Kernel implementation.</para>
/// <para><b>S1 (Domain Facades)</b>: Public domain API layer.</para>
/// <para><b>S2 (Application)</b>: Application services / orchestration.</para>
/// <para><b>S3 (Infrastructure/Consumer)</b>: Strictest enforcement (default).</para>
/// </remarks>
public enum Scope
{
    /// <summary>
    /// S0: Kernel implementation scope.
    /// </summary>
    S0 = 0,

    /// <summary>
    /// S1: Domain facade scope (public domain API layer).
    /// </summary>
    S1 = 1,

    /// <summary>
    /// S2: Application layer scope (services/orchestration).
    /// </summary>
    S2 = 2,

    /// <summary>
    /// S3: Infrastructure/consumer scope (default; strictest enforcement).
    /// </summary>
    S3 = 3
}
