// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="GeneratorDiagnostic.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Generators.Core;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Dx.Domain.Generators.Diagnostics
{
    /// <summary>
    /// Represents a diagnostic emitted by the generator conforming to the mandatory diagnostic schema (DX-003).
    /// Every diagnostic MUST conform to this shape to support the Pedagogical IDE experience.
    /// </summary>
    public sealed class GeneratorDiagnostic
    {
        /// <summary>
        /// Gets the unique diagnostic identifier (e.g., DX1001, DX2001).
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the failure classification.
        /// </summary>
        public FailureClass Class { get; }

        /// <summary>
        /// Gets the diagnostic title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the diagnostic message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the input fingerprint associated with this diagnostic.
        /// </summary>
        public Core.InputFingerprint InputFingerprint { get; }

        /// <summary>
        /// Gets the stage name where this diagnostic was emitted.
        /// </summary>
        public string Stage { get; }

        /// <summary>
        /// Gets the location information (file, line, column).
        /// </summary>
        public DiagnosticLocation? Location { get; }

        /// <summary>
        /// Gets the remediation options array (required by DX-003).
        /// Each option provides actionable steps to fix the diagnostic.
        /// </summary>
        public ImmutableList<Remediation> RemediationOptions { get; }

        /// <summary>
        /// Gets a value indicating whether this diagnostic has a fix preview available.
        /// Required for Pedagogical IDE support (DX-003).
        /// </summary>
        public bool HasFixPreview { get; }

        /// <summary>
        /// Gets the fix preview text, if available.
        /// </summary>
        public string? FixPreview { get; }

        /// <summary>
        /// Gets the impact level of this diagnostic.
        /// </summary>
        public ImpactLevel Impact { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorDiagnostic"/> class.
        /// </summary>
        public GeneratorDiagnostic(
            string id,
            FailureClass @class,
            string title,
            string message,
            InputFingerprint inputFingerprint,
            string stage,
            DiagnosticLocation? location,
            IEnumerable<Remediation> remediationOptions,
            string? fixPreview,
            ImpactLevel impact)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Class = @class;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            InputFingerprint = inputFingerprint ?? throw new ArgumentNullException(nameof(inputFingerprint));
            Stage = stage ?? throw new ArgumentNullException(nameof(stage));
            Location = location;
            RemediationOptions = remediationOptions?.ToImmutableList() ?? ImmutableList<Remediation>.Empty;
            FixPreview = fixPreview;
            HasFixPreview = !string.IsNullOrEmpty(fixPreview);
            Impact = impact;
        }
    }

    /// <summary>
    /// Represents the location information for a diagnostic.
    /// </summary>
    public readonly struct DiagnosticLocation
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the line number (1-based).
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the column number (1-based).
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticLocation"/> class.
        /// </summary>
        public DiagnosticLocation(string filePath, int line, int column)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Line = line;
            Column = column;
        }
    }
}
