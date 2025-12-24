// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="CommitConflict.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Factors;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Dx.Domain.Generators.Core
{
    /// <summary>
    /// Represents a monotonicity violation for a single fact key during commit.
    /// </summary>
    public sealed record CommitConflict(
        string Key,
        IDomainFact ExistingFact,
        object ProposedValue)
    {
        public override string ToString()
            => $"Conflict on '{Key}': existing value differs from proposed value.";
    }
    /// <summary>
    /// Represents a failed atomic commit due to one or more monotonic conflicts.
    /// </summary>
    public sealed class CommitFailure
    {
        public ImmutableArray<CommitConflict> Conflicts { get; }

        public CommitFailure(IEnumerable<CommitConflict> conflicts)
        {
            ArgumentNullException.ThrowIfNull(conflicts);

            var materialized = conflicts.ToImmutableArray();
            if (materialized.IsEmpty)
                throw new ArgumentException("CommitFailure must contain at least one conflict.", nameof(conflicts));

            Conflicts = materialized;
        }

        /// <summary>
        /// Converts this failure into a generator resolution request suitable
        /// for UI, analyzers, or policy engines.
        /// </summary>
        public ResolutionRequest ToResolutionRequest()
        {
            var first = Conflicts[0];

            var candidates = Conflicts.Select(c =>
                new CandidateResolution(
                    name: $"Resolve:{c.Key}",
                    description: $"Resolve conflict for '{c.Key}'.",
                    recommendedAction: $"Define a refinement policy for '{c.Key}'."))
                .ToImmutableList();

            return new ResolutionRequest(
                ambiguousNode: first.Key,
                candidateResolutions: candidates,
                requiredPolicyHint:
                    $"Define refinement policies for keys: {string.Join(", ", Conflicts.Select(c => c.Key))}.",
                description:
                    $"Atomic commit failed due to {Conflicts.Length} monotonic conflict(s).");
        }

        public override string ToString()
            => $"CommitFailure ({Conflicts.Length} conflict(s))";
    }
}
