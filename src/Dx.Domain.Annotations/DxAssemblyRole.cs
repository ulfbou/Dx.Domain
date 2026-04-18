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

namespace Dx.Domain.Annotations
{
    /// <summary>
    /// Defines the architectural role vocabulary (pure metadata).
    /// </summary>
    /// <remarks>
    /// This enumeration imposes no runtime semantics. Analyzers use it to classify roles
    /// and apply dependency/boundary rules. SEE: Rule Charter → Role/Boundary Enforcement.
    /// </remarks>
    public enum DxAssemblyRole
    {
        /// <summary>
        /// Contract assemblies containing DTOs, interfaces, and data contracts.
        /// </summary>
        Contracts = 0,

        /// <summary>
        /// Domain assemblies containing domain model code (entities, value objects, domain services).
        /// </summary>
        Domain = 1,

        /// <summary>
        /// Application assemblies hosting use cases/orchestration.
        /// </summary>
        Application = 2,

        /// <summary>
        /// Infrastructure assemblies (persistence, external service adapters, etc.).
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
