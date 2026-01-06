// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxRuleIds.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Diagnostics
{
    /// <summary>
    /// Canonical rule identifiers used by analyzers and tooling.
    /// Keep this file stable and versioned.
    /// </summary>
    // DPI: Centralized repository of diagnostic rule IDs for Dx analysis tools.
    public static class DxRuleIds
    {
        // Architecture and construction
        public const string ConstructionAuthority = "DXA010";
        public const string PublicFactoryExposure = "DXA011";

        // Result handling
        public const string ResultIgnored = "DXA020";
        public const string DomainControlException = "DXA022";

        // Kernel integrity
        public const string KernelFreeze = "DXA040";
        public const string TemporalCoupling = "DXA050";
        public const string ForbiddenVocabulary = "DXA060";

        // Generators and tagging
        public const string GeneratedCodeTagging = "DXA070";
        public const string FacadeInvariant = "DXA080";

        // Guard composer invariant error
        // caught exception in AND composition
        public const string GuardComposerAndException = "DXA090";
    }
}
