// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dxt.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Diagnostics
{
    internal static class Dxt
    {
        public static readonly DiagnosticDescriptor DXT001 = new(
            "DXT001",
            "Template role completeness",
            "Template-generated project must declare role '{0}'",
            Categories.Template,
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        public static readonly DiagnosticDescriptor DXT002 = new(
            "DXT002",
            "Template required reference missing",
            "Template role '{0}' requires reference to '{1}'",
            Categories.Template,
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        public static readonly DiagnosticDescriptor DXT003 = new(
            "DXT003",
            "Template forbidden reference present",
            "Template role '{0}' must not reference '{1}'",
            Categories.Template,
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.CompilationEnd);
    }
}
