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

using Dx.Domain.Annotations;

using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Diagnostics
{
    internal static class Dxk
    {
        public static readonly DiagnosticDescriptor DXK001 = new(
            DxRuleIds.DXK001,
            "Assembly role required",
            "Assembly must declare exactly one DxAssemblyRole",
            DxCategories.Kernel,
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        public static readonly DiagnosticDescriptor DXK002 = new(
            DxRuleIds.DXK002,
            "Illegal role dependency",
            "Assembly role '{0}' may not reference '{1}'",
            DxCategories.Kernel,
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        public static readonly DiagnosticDescriptor DXK003 = new(
            DxRuleIds.DXK003,
            "Domain purity violation",
            "Domain assemblies may not reference '{0}'",
            DxCategories.Kernel,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor DXK004 = new(
            DxRuleIds.DXK004,
            "Primitive obsession",
            "Use domain primitive '{0}' instead of '{1}'",
            DxCategories.Kernel,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor DXK005 = new(
            DxRuleIds.DXK005,
            "Illegal exception flow",
            "Exceptions may not be used for flow control in {0} assemblies",
            DxCategories.Kernel,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor DXK006 = new(
            DxRuleIds.DXK006,
            "Invalid outbox payload",
            "Only IDomainFact may cross the outbox boundary",
            DxCategories.Kernel,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor DXK007 = new(
            DxRuleIds.DXK007,
            "Contract hygiene violation",
            "Contracts assemblies may not reference Dx.Domain.Kernel",
            DxCategories.Kernel,
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        public static readonly DiagnosticDescriptor DXK008 = new(
            DxRuleIds.DXK008,
            "Observability invariant violated",
            "Host assemblies must propagate CorrelationId",
            DxCategories.Kernel,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor DXK009 = new(
            DxRuleIds.DXK009,
            "Forbidden Dx.Domain internal package reference",
            "Consumer projects must not reference internal package '{0}'",
            DxCategories.Kernel,
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.CompilationEnd);
    }
}
