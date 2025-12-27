// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="CommonDiagnostics.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain
{
    // -------------------------------------------------------------------------
    // Shared Diagnostic Constants & Enums
    // -------------------------------------------------------------------------
    // Shared between Runtime, Analyzers, and Generators to ensure code sync.

    /// <summary>
    /// Canonical Rule IDs for Dx Analysis.
    /// </summary>
    public static class DxRuleIds
    {
        // Architecture
        public const string ConstructionAuthority = "DXA010";
        public const string PublicFactoryExposure = "DXA011";

        // Results
        public const string ResultIgnored = "DXA020";
        public const string DomainControlException = "DXA022";
        public const string UnapprovedHandler = "DXA030";

        // Kernel
        public const string KernelFreeze = "DXA040";
        public const string TemporalCoupling = "DXA050";
        public const string ForbiddenVocabulary = "DXA060";

        // Generators
        public const string GeneratedCodeTagging = "DXA070";
        public const string FacadeInvariant = "DXA080";
    }
}
