// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxAssemblyRole.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain
{
    /// <summary>
    /// Defines the architectural role of an assembly within the Dx.Domain framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This enumeration is used by analyzers to enforce architectural boundaries and
    /// dependency rules across the application layers.
    /// </para>
    /// </remarks>
    public enum DxAssemblyRole
    {
        /// <summary>
        /// Contract assemblies containing DTOs, interfaces, and data contracts.
        /// </summary>
        Contracts = 0,

        /// <summary>
        /// Domain assemblies containing domain logic, entities, value objects, and domain services.
        /// </summary>
        Domain = 1,

        /// <summary>
        /// Application assemblies containing application services, use cases, and orchestration logic.
        /// </summary>
        Application = 2,

        /// <summary>
        /// Infrastructure assemblies containing implementations for persistence, external services, etc.
        /// </summary>
        Infrastructure = 3,

        /// <summary>
        /// Host assemblies containing API controllers, composition root, and startup logic.
        /// </summary>
        Host = 4,

        /// <summary>
        /// Shared assemblies containing cross-cutting utilities and helpers.
        /// </summary>
        Shared = 5
    }
}
