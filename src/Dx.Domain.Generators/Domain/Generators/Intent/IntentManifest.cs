// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IntentManifest.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Dx.Domain.Generators.Intent
{
    /// <summary>
    /// Represents the intent manifest for an entity, providing transparency into generation.
    /// </summary>
    /// <remarks>
    /// Intent manifests make generation explicit, inspectable, and deterministic.
    /// They answer "why does this exist?" for every generated artifact.
    /// </remarks>
    public sealed class IntentManifest
    {
        /// <summary>
        /// Gets the entity name (e.g., "Order").
        /// </summary>
        public string Entity { get; init; } = string.Empty;

        /// <summary>
        /// Gets the entity kind (e.g., "Aggregate", "ValueObject", "Entity").
        /// </summary>
        public string Kind { get; init; } = string.Empty;

        /// <summary>
        /// Gets the source location information.
        /// </summary>
        public SourceLocation Source { get; init; } = new();

        /// <summary>
        /// Gets the declared intent from attributes and code.
        /// </summary>
        public DeclaredIntent DeclaredIntent { get; init; } = new();

        /// <summary>
        /// Gets the derived facts from analysis.
        /// </summary>
        public DerivedFacts DerivedFacts { get; init; } = new();

        /// <summary>
        /// Gets the policies applied during generation.
        /// </summary>
        public IReadOnlyList<string> PoliciesApplied { get; init; } = System.Array.Empty<string>();

        /// <summary>
        /// Gets the generated artifacts produced.
        /// </summary>
        public IReadOnlyList<string> GeneratedArtifacts { get; init; } = System.Array.Empty<string>();
    }

    /// <summary>
    /// Represents the source location of an entity.
    /// </summary>
    public sealed class SourceLocation
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string File { get; init; } = string.Empty;

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int Line { get; init; }
    }

    /// <summary>
    /// Represents the declared intent from code annotations.
    /// </summary>
    public sealed class DeclaredIntent
    {
        /// <summary>
        /// Gets the attributes applied to the entity.
        /// </summary>
        public IReadOnlyList<string> Attributes { get; init; } = System.Array.Empty<string>();

        /// <summary>
        /// Gets the commands declared on the entity.
        /// </summary>
        public IReadOnlyList<string> Commands { get; init; } = System.Array.Empty<string>();

        /// <summary>
        /// Gets the events declared on the entity.
        /// </summary>
        public IReadOnlyList<string> Events { get; init; } = System.Array.Empty<string>();
    }

    /// <summary>
    /// Represents facts derived from code analysis.
    /// </summary>
    public sealed class DerivedFacts
    {
        /// <summary>
        /// Gets the number of invariants detected.
        /// </summary>
        public int Invariants { get; init; }

        /// <summary>
        /// Gets the number of state transitions.
        /// </summary>
        public int StateTransitions { get; init; }

        /// <summary>
        /// Gets the relationships to other entities.
        /// </summary>
        public IReadOnlyDictionary<string, string> Relationships { get; init; } =
            new Dictionary<string, string>();

        /// <summary>
        /// Gets additional derived information.
        /// </summary>
        public IReadOnlyDictionary<string, string> AdditionalFacts { get; init; } =
            new Dictionary<string, string>();
    }
}
