// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageDeclaration.cs" company="Dx.Domain Team">
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
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Dx.Domain.Generators.Pipeline
{
    /// <summary>
    /// Represents the declaration of a generator pipeline stage.
    /// Each stage must declare its metadata for incrementality and caching (Section 4 of specification).
    /// </summary>
    public sealed class StageDeclaration
    {
        /// <summary>
        /// Gets the unique name of the stage.
        /// </summary>
        public string StageName { get; }

        /// <summary>
        /// Gets the input keys consumed by this stage.
        /// </summary>
        public ImmutableList<string> InputKeys { get; }

        /// <summary>
        /// Gets the output keys produced by this stage.
        /// </summary>
        public ImmutableList<string> OutputKeys { get; }

        /// <summary>
        /// Gets the version of this stage implementation.
        /// </summary>
        public string StageVersion { get; }

        /// <summary>
        /// Gets a value indicating whether this stage is cacheable.
        /// Cacheable stages must be deterministic and cannot read external resources.
        /// </summary>
        public bool Cacheable { get; }

        /// <summary>
        /// Gets the declared dependencies of this stage.
        /// </summary>
        public ImmutableList<string> DeclaredDependencies { get; }

        /// <summary>
        /// Gets the capabilities provided by this stage.
        /// </summary>
        public ImmutableList<string> Capabilities { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StageDeclaration"/> class.
        /// </summary>
        /// <param name="stageName">The stage name.</param>
        /// <param name="inputKeys">The input keys.</param>
        /// <param name="outputKeys">The output keys.</param>
        /// <param name="stageVersion">The stage version.</param>
        /// <param name="cacheable">Whether the stage is cacheable.</param>
        /// <param name="declaredDependencies">The declared dependencies.</param>
        /// <param name="capabilities">The capabilities.</param>
        public StageDeclaration(
            string stageName,
            IEnumerable<string> inputKeys,
            IEnumerable<string> outputKeys,
            string stageVersion,
            bool cacheable,
            IEnumerable<string>? declaredDependencies = null,
            IEnumerable<string>? capabilities = null)
        {
            StageName = stageName ?? throw new ArgumentNullException(nameof(stageName));
            InputKeys = inputKeys?.ToImmutableList() ?? ImmutableList<string>.Empty;
            OutputKeys = outputKeys?.ToImmutableList() ?? ImmutableList<string>.Empty;
            StageVersion = stageVersion ?? throw new ArgumentNullException(nameof(stageVersion));
            Cacheable = cacheable;
            DeclaredDependencies = declaredDependencies?.ToImmutableList() ?? ImmutableList<string>.Empty;
            Capabilities = capabilities?.ToImmutableList() ?? ImmutableList<string>.Empty;
        }

        /// <summary>
        /// Computes the cache key for this stage given an input fingerprint and policy versions.
        /// Cache key = SHA256(InputFingerprint || stageName || stageVersion || policyVersions).
        /// </summary>
        /// <param name="inputFingerprint">The input fingerprint.</param>
        /// <param name="policyVersions">The policy versions.</param>
        /// <returns>The computed cache key.</returns>
        public string ComputeCacheKey(Core.InputFingerprint inputFingerprint, string policyVersions)
        {
            ArgumentNullException.ThrowIfNull(inputFingerprint);
            ArgumentNullException.ThrowIfNull(policyVersions);

            var combined = string.Concat(
                inputFingerprint.Value,
                StageName,
                StageVersion,
                policyVersions);

            var hash = System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(combined));

            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
