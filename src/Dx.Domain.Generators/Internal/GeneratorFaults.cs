// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FactKey.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Generators.Internal
{
    /// <summary>
    /// Bridges the Generator infrastructure to the Kernel's Fault system.
    /// Utilizes the public Faults.Factory to maintain package isolation.
    /// </summary>
    internal static class GeneratorFaults
    {
        private const string GenNamespace = "Dx.Gen";

        /// <summary>
        /// Produces a DomainError for failed stage prerequisites.
        /// </summary>
        /// <param name="details">The specifics of the missing or forbidden facts.</param>
        public static DomainError AssertionFailed(string details)
        {
            // We use the public Factory helper provided in Dx.Faults.cs
            return DxDomain.Faults.Factory.Create(
                $"{GenNamespace}.AssertionFailed",
                $"Stage execution blocked: {details}");
        }

        /// <summary>
        /// Produces a DomainError for monotonicity violations within the FactStore.
        /// </summary>
        public static DomainError MonotonicityViolation(string key)
        {
            return DxDomain.Faults.Factory.Create(
                $"{GenNamespace}.MonotonicityConflict",
                $"Monotonicity violation: Fact '{key}' has already been committed.");
        }
    }
}
