// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dxt003_TemplateForbiddenReferencesAnalyzer.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Immutable;
using System.Linq;

using Dx.Domain.Analyzers.Diagnostics;
using Dx.Domain.Analyzers.Templates;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class Dxt003_TemplateForbiddenReferencesAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Dxt.DXT003);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationAction(compilationContext =>
            {
                var templateId = TemplateIntentResolver.Resolve(compilationContext.Compilation);
                if (string.IsNullOrWhiteSpace(templateId))
                    return;

                if (!TemplateRoleMap.TryGet(templateId!, out var definition))
                    return;

                var references = compilationContext.Compilation.ReferencedAssemblyNames
                    .Select(r => r.Name)
                    .ToImmutableHashSet(System.StringComparer.OrdinalIgnoreCase);

                foreach (var forbidden in definition.ForbiddenReferences)
                {
                    if (!references.Contains(forbidden))
                        continue;

                    compilationContext.ReportDiagnostic(
                        Diagnostic.Create(Dxt.DXT003, Location.None, definition.Role, forbidden));
                }
            });
        }
    }
}
