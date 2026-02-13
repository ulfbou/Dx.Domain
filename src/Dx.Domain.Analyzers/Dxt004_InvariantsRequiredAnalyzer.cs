// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dxt004_InvariantsRequiredAnalyzer.cs" company="Dx.Domain Team">
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
using System.Collections.Immutable;
using System.Linq;

using Dx.Domain.Analyzers.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class Dxt004_InvariantsRequiredAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Dxt.DXT004);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationAction(compilationContext =>
            {
                if (IsAuthorityLayer(compilationContext.Options.AnalyzerConfigOptionsProvider))
                    return;

                var dxtFile = compilationContext.Options.AdditionalFiles.FirstOrDefault(file =>
                    file.Path.Replace('\\', '/').EndsWith("/.dx/invariants.json", StringComparison.OrdinalIgnoreCase));

                if (dxtFile == null)
                    compilationContext.ReportDiagnostic(Diagnostic.Create(Dxt.DXT004, Location.None));
            });
        }

        private static bool IsAuthorityLayer(AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!optionsProvider.GlobalOptions.TryGetValue("build_property.DxLayer", out var layer))
            {
                optionsProvider.GlobalOptions.TryGetValue("dx.layer", out layer);
            }

            if (string.IsNullOrWhiteSpace(layer))
                return false;

            return string.Equals(layer, "Kernel", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(layer, "Primitives", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(layer, "Annotations", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(layer, "Authority", StringComparison.OrdinalIgnoreCase);
        }
    }
}
