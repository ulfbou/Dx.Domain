// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IGeneratorStage.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain;
using Dx.Domain.Generators.Abstractions;

using System.Threading;
using System.Threading.Tasks;

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Core interface for all generator stages as defined in DX-002.
    /// All implementations MUST adhere to this interface to ensure pipeline compatibility.
    /// </summary>
    public interface IGeneratorStage
    {
        /// <summary>
        /// Gets the unique name of this stage.
        /// </summary>
        string StageName { get; }

        /// <summary>
        /// Gets the version of this stage implementation.
        /// Note: This is distinct from GeneratorVersion used in the fingerprint.
        /// </summary>
        string StageVersion { get; }

        /// <summary>
        /// Gets the capabilities of this stage (I/O, Network, etc.).
        /// </summary>
        StageCapabilities Capabilities { get; }

        /// <summary>
        /// Executes the stage with the provided context.
        /// </summary>
        /// <param name="context">The stage context containing input fingerprint, manifest, policy, and prior facts.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// A canonical <see cref="Result{TValue, TError}"/> carrying either a
        /// <see cref="StageSuccessPayload"/> or a <see cref="StageFailurePayload"/>,
        /// never throwing for domain or semantic failures.
        /// </returns>
        Task<Result<StageSuccessPayload, StageFailurePayload>> ExecuteAsync(
            StageContext context,
            CancellationToken ct);
    }
}
