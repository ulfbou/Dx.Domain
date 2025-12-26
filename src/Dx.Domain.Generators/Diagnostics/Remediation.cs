// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Remediation.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Generators.Diagnostics
{
    /// <summary>
    /// Represents a remediation option for a diagnostic as defined in DX-003.
    /// Provides actionable steps to fix the diagnostic with impact level tagging.
    /// </summary>
    public sealed class Remediation
    {
        /// <summary>
        /// Gets the description of the remediation step.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the impact level of applying this remediation.
        /// </summary>
        public ImpactLevel Impact { get; }

        /// <summary>
        /// Gets the code fix text, if available.
        /// </summary>
        public string? CodeFix { get; }

        /// <summary>
        /// Gets a value indicating whether this remediation can be auto-applied.
        /// </summary>
        public bool CanAutoApply { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Remediation"/> class.
        /// </summary>
        public Remediation(string description, ImpactLevel impact, string? codeFix = null, bool canAutoApply = false)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Impact = impact;
            CodeFix = codeFix;
            CanAutoApply = canAutoApply;
        }
    }
}
