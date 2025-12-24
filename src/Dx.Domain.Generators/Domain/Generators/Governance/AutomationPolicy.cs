// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AutomationPolicy.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Generators.Governance
{
    /// <summary>
    /// Represents an automation policy for a specific context (Section 6 of specification).
    /// </summary>
    public sealed class AutomationPolicy
    {
        /// <summary>
        /// Gets the automation level for this policy.
        /// </summary>
        public AutoFixLevel AutoFixLevel { get; }

        /// <summary>
        /// Gets the maximum impact level that can be auto-applied.
        /// </summary>
        public Diagnostics.ImpactLevel MaxAutoApplyImpact { get; }

        /// <summary>
        /// Gets a value indicating whether this policy applies to CI environments.
        /// </summary>
        public bool AppliesInCi { get; }

        /// <summary>
        /// Gets a value indicating whether this policy applies to IDE environments.
        /// </summary>
        public bool AppliesInIde { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationPolicy"/> class.
        /// </summary>
        public AutomationPolicy(
            AutoFixLevel autoFixLevel,
            Diagnostics.ImpactLevel maxAutoApplyImpact,
            bool appliesInCi = true,
            bool appliesInIde = true)
        {
            AutoFixLevel = autoFixLevel;
            MaxAutoApplyImpact = maxAutoApplyImpact;
            AppliesInCi = appliesInCi;
            AppliesInIde = appliesInIde;
        }

        /// <summary>
        /// Determines if a fix with the given impact level can be auto-applied in the specified context.
        /// </summary>
        /// <param name="impactLevel">The impact level of the fix.</param>
        /// <param name="isCiContext">Whether the context is CI.</param>
        /// <returns>True if the fix can be auto-applied, false otherwise.</returns>
        public bool CanAutoApply(Diagnostics.ImpactLevel impactLevel, bool isCiContext)
        {
            // Check if policy applies to this context
            if (isCiContext && !AppliesInCi)
                return false;
            if (!isCiContext && !AppliesInIde)
                return false;

            // Check auto-fix level
            if (AutoFixLevel == AutoFixLevel.None)
                return false;
            if (AutoFixLevel == AutoFixLevel.Suggest)
                return false;

            // Check impact level
            if (AutoFixLevel == AutoFixLevel.Safe)
                return impactLevel == Diagnostics.ImpactLevel.Safe;

            // AutoFixLevel.Apply allows up to MaxAutoApplyImpact
            return impactLevel <= MaxAutoApplyImpact;
        }

        /// <summary>
        /// Creates a default CI automation policy (strict).
        /// </summary>
        public static AutomationPolicy DefaultCiPolicy => new(
            AutoFixLevel.Safe,
            Diagnostics.ImpactLevel.Safe,
            appliesInCi: true,
            appliesInIde: false);

        /// <summary>
        /// Creates a default IDE automation policy (permissive).
        /// </summary>
        public static AutomationPolicy DefaultIdePolicy => new(
            AutoFixLevel.Suggest,
            Diagnostics.ImpactLevel.Behavioral,
            appliesInCi: false,
            appliesInIde: true);
    }
}
