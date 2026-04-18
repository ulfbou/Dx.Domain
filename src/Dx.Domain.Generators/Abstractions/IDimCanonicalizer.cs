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

using Dx.Domain.Generators.Model;

namespace Dx.Domain.Generators.Canonicalization
{
    /// <summary>
    /// Responsible for converting raw, schema-validated YAML input into the 
    /// authoritative, deterministically ordered Domain Intent Model (DIM).
    /// </summary>
    public interface IDimCanonicalizer
    {
        /// <summary>
        /// Canonicalizes the raw YAML input into a deterministic Intent Model.
        /// </summary>
        /// <param name="rawYamlContent">The raw file content.</param>
        /// <param name="filePath">Source path for diagnostic location.</param>
        /// <returns>
        /// A Result containing the Canonical JSON string (for fingerprinting) 
        /// and the typed Model (for generation).
        /// </returns>
        Result<(string CanonicalJson, DomainIntentModel Model)> Canonicalize(
            string rawYamlContent,
            string filePath);
    }
}
