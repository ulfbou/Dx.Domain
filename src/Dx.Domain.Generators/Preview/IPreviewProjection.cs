// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IPreviewProjection.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Generators.Preview
{
    /// <summary>
    /// Represents an in-memory preview of generated code, displayed as IDE overlays.
    /// </summary>
    /// <remarks>
    /// Preview projections run entirely in-memory without disk writes.
    /// They provide "ghost outlines" showing what will be generated.
    /// The developer sees intent materialized before compilation.
    /// </remarks>
    public interface IPreviewProjection
    {
        /// <summary>
        /// Gets the type name for this preview.
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Gets the preview members (methods, properties, fields).
        /// </summary>
        IReadOnlyList<PreviewMember> Members { get; }

        /// <summary>
        /// Gets additional metadata for IDE presentation.
        /// </summary>
        IReadOnlyDictionary<string, string> Metadata { get; }
    }

    /// <summary>
    /// Represents a single member in a preview projection.
    /// </summary>
    public sealed class PreviewMember
    {
        /// <summary>
        /// Gets the member kind (Method, Property, Field, Event).
        /// </summary>
        public string Kind { get; init; } = string.Empty;

        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets the member signature or declaration.
        /// </summary>
        public string Signature { get; init; } = string.Empty;

        /// <summary>
        /// Gets the explanation of why this member exists.
        /// </summary>
        public string Explanation { get; init; } = string.Empty;

        /// <summary>
        /// Gets whether this member is generated or user-defined.
        /// </summary>
        public bool IsGenerated { get; init; }
    }

    /// <summary>
    /// Scanner that converts source code into an Intent Model.
    /// </summary>
    /// <remarks>
    /// This is the first stage of the Reactive Feedback Engine.
    /// It scans syntax and symbols to emit a canonical Intent Model.
    /// </remarks>
    public interface IIntentScanner
    {
        /// <summary>
        /// Scans the source and produces an intent model.
        /// </summary>
        /// <param name="source">The source code to scan.</param>
        /// <returns>The intent model extracted from source.</returns>
        IntentModel Scan(string source);
    }

    /// <summary>
    /// Represents the canonical intent model extracted from source code.
    /// </summary>
    public sealed class IntentModel
    {
        /// <summary>
        /// Gets the entities discovered.
        /// </summary>
        public IReadOnlyList<EntityIntent> Entities { get; init; } = Array.Empty<EntityIntent>();

        /// <summary>
        /// Gets the completeness assessment for pedagogical feedback.
        /// </summary>
        public CompletenessVector Completeness { get; init; } = new();
    }

    /// <summary>
    /// Represents the intent for a single entity.
    /// </summary>
    public sealed class EntityIntent
    {
        /// <summary>
        /// Gets the entity name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets the entity kind (Aggregate, Entity, ValueObject).
        /// </summary>
        public string Kind { get; init; } = string.Empty;

        /// <summary>
        /// Gets the declared attributes.
        /// </summary>
        public IReadOnlyList<string> Attributes { get; init; } = Array.Empty<string>();
    }

    /// <summary>
    /// Represents a completeness assessment for predictive diagnostics.
    /// </summary>
    public sealed class CompletenessVector
    {
        /// <summary>
        /// Gets the required elements that are missing.
        /// </summary>
        public IReadOnlyList<string> MissingRequired { get; init; } = Array.Empty<string>();

        /// <summary>
        /// Gets optional enrichments that could improve the design.
        /// </summary>
        public IReadOnlyList<string> OptionalEnrichments { get; init; } = Array.Empty<string>();

        /// <summary>
        /// Gets forbidden constructs that were detected.
        /// </summary>
        public IReadOnlyList<string> ForbiddenConstructs { get; init; } = Array.Empty<string>();
    }
}
