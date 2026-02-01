// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dxk.cs" company="Dx.Domain Team">
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
    internal static class Dxk
    {
        public static readonly DiagnosticDescriptor DXK001 = new(
            "DXK001",
            "Assembly role required",
            "Assembly must declare exactly one DxAssemblyRole",
            Categories.Kernel,
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        public static readonly DiagnosticDescriptor DXK002 = new(
            "DXK002",
            "Illegal role dependency",
            "Assembly role '{0}' may not reference '{1}'",
            Categories.Kernel,
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        public static readonly DiagnosticDescriptor DXK003 = new(
            "DXK003",
            "Domain purity violation",
            "Domain assemblies may not reference '{0}'",
            Categories.Kernel,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor DXK004 = new(
            "DXK004",
            "Primitive obsession",
            "Use domain primitive '{0}' instead of '{1}'",
            Categories.Kernel,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor DXK005 = new(
            "DXK005",
            "Illegal exception flow",
            "Exceptions may not be used for flow control in {0} assemblies",
            Categories.Kernel,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor DXK006 = new(
            "DXK006",
            "Invalid outbox payload",
            "Only IDomainFact may cross the outbox boundary",
            Categories.Kernel,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor DXK007 = new(
            "DXK007",
            "Contract hygiene violation",
            "Contracts assemblies may not reference Dx.Domain.Kernel",
            Categories.Kernel,
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        public static readonly DiagnosticDescriptor DXK008 = new(
            "DXK008",
            "Observability invariant violated",
            "Host assemblies must propagate CorrelationId",
            Categories.Kernel,
            DiagnosticSeverity.Error,
            true);
    }
}
