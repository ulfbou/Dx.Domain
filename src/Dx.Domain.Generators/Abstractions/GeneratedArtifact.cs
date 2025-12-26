// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="GeneratedArtifact.cs" company="Dx.Domain Team">
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
    /// Represents a generated artifact from a stage execution.
    /// </summary>
    public sealed class GeneratedArtifact
    {
        /// <summary>
        /// Gets the path of the generated artifact (relative or absolute).
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the content of the generated artifact.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the hash of the artifact content (SHA256).
        /// </summary>
        public string ContentHash { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedArtifact"/> class.
        /// </summary>
        public GeneratedArtifact(string path, string content, string contentHash)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            ContentHash = contentHash ?? throw new ArgumentNullException(nameof(contentHash));
        }
    }
}
