// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ResolutionRequest.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Generators.Core
{
    /// <summary>
    /// Represents a request for explicit resolution of ambiguous intent.
    /// Generated when the generator cannot deterministically infer required contracts (No Semantic Guessing invariant).
    /// </summary>
    public sealed class ResolutionRequest
    {
        /// <summary>
        /// Gets the ambiguous node or element that requires resolution.
        /// </summary>
        public string AmbiguousNode { get; }

        /// <summary>
        /// Gets the candidate resolutions available.
        /// </summary>
        public ImmutableList<CandidateResolution> CandidateResolutions { get; }

        /// <summary>
        /// Gets the hint for the required policy to resolve the ambiguity.
        /// </summary>
        public string RequiredPolicyHint { get; }

        /// <summary>
        /// Gets the description of the ambiguity.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionRequest"/> class.
        /// </summary>
        /// <param name="ambiguousNode">The ambiguous node or element.</param>
        /// <param name="candidateResolutions">The candidate resolutions.</param>
        /// <param name="requiredPolicyHint">The required policy hint.</param>
        /// <param name="description">The description of the ambiguity.</param>
        public ResolutionRequest(
            string ambiguousNode,
            IEnumerable<CandidateResolution> candidateResolutions,
            string requiredPolicyHint,
            string description)
        {
            AmbiguousNode = ambiguousNode ?? throw new ArgumentNullException(nameof(ambiguousNode));
            CandidateResolutions = candidateResolutions?.ToImmutableList() ?? ImmutableList<CandidateResolution>.Empty;
            RequiredPolicyHint = requiredPolicyHint ?? throw new ArgumentNullException(nameof(requiredPolicyHint));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }

    /// <summary>
    /// Represents a candidate resolution for an ambiguous intent.
    /// </summary>
    public sealed class CandidateResolution
    {
        /// <summary>
        /// Gets the name of the candidate resolution.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of this resolution option.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the recommended action or code to apply this resolution.
        /// </summary>
        public string RecommendedAction { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CandidateResolution"/> class.
        /// </summary>
        /// <param name="name">The name of the resolution.</param>
        /// <param name="description">The description of the resolution.</param>
        /// <param name="recommendedAction">The recommended action.</param>
        public CandidateResolution(string name, string description, string recommendedAction)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            RecommendedAction = recommendedAction ?? throw new ArgumentNullException(nameof(recommendedAction));
        }
    }
}
