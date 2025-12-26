// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FileName.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Generators.Artifacts;

using System.Text.Json.Serialization;

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Defines the contract for generators capable of producing "Signed Artifacts".
    /// </summary>
    /// <remarks>
    /// An authoritative emitter differs from an advisory emitter (SourceGen) by 
    /// mandating the computation of a content-based signature before the artifact is committed.
    /// </remarks>
    public interface IAuthoritativeEmitter
    {
        /// <summary>
        /// Executes the two-phase emission protocol: produces the content, then signs it.
        /// </summary>
        /// <param name="intent">The canonical domain intent model.</param>
        /// <param name="context">The stage-specific generation context containing policy and fingerprints.</param>
        /// <returns>A <see cref="SignedArtifact"/> containing the payload and the validated header.</returns>
        SignedArtifact Emit(object intent, StageContext context);
    }
}
