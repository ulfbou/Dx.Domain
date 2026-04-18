// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="SignedArtifact.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Represents a generated artifact that has been signed for provenance.
    /// </summary>
    public sealed class SignedArtifact
    {
        public SignedArtifact(GeneratedArtifact artifact, string signature)
        {
            Artifact = artifact ?? throw new ArgumentNullException(nameof(artifact));
            Signature = signature ?? throw new ArgumentNullException(nameof(signature));
        }

        public GeneratedArtifact Artifact { get; }

        public string Signature { get; }
    }
}
