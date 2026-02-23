// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Dx.Domain.Annotations;

/// <summary>
/// Defines the architectural scope boundaries for Dx.Domain enforcement.
/// </summary>
/// <remarks>
/// <para><b>S0 (Kernel)</b>: Core kernel implementation. Can construct kernel types directly.</para>
/// <para><b>S1 (Domain Facades)</b>: Public domain API layer. Must use [DxFacade] for construction.</para>
/// <para><b>S2 (Application Layer)</b>: Application services. Must use facades for domain construction.</para>
/// <para><b>S3 (Infrastructure/Consumer)</b>: Infrastructure and consumer code. Strictest enforcement.</para>
/// </remarks>
public enum Scope
{
    /// <summary>
    /// S0: Kernel implementation scope.
    /// Can construct kernel types directly. Subject to kernel laws and DPI.
    /// </summary>
    S0 = 0,

    /// <summary>
    /// S1: Domain facade scope.
    /// Public domain API layer. Must expose construction via [DxFacade] classes.
    /// </summary>
    S1 = 1,

    /// <summary>
    /// S2: Application layer scope.
    /// Application services and orchestration. Must use S1 facades.
    /// </summary>
    S2 = 2,

    /// <summary>
    /// S3: Infrastructure/consumer scope (default).
    /// Infrastructure adapters, UI, external integrations. Strictest enforcement.
    /// </summary>
    S3 = 3
}
