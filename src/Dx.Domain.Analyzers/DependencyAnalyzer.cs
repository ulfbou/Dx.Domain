// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DependencyAnalyzer.cs" company="Dx.Domain Team">
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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using Dx.Domain.Analyzers.Diagnostics;
using Dx.Domain.Annotations;
using Dx.Domain.Analyzers.Roles;

namespace Dx.Domain.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DependencyAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Dxk.DXK002, Dxk.DXK007, Dxk.DXK009);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(c =>
            {
                var role = RoleResolver.Resolve(c.Compilation, c.Options.AnalyzerConfigOptionsProvider);
                if (role is null)
                    return;

                var scopeResolver = new Infrastructure.Scopes.ScopeResolver(c.Options.AnalyzerConfigOptionsProvider);
                if (scopeResolver.ResolveAssembly(c.Compilation.Assembly) != Infrastructure.Scopes.Scope.S3)
                    return;

                foreach (var reference in c.Compilation.ReferencedAssemblyNames)
                {
                    if (!Enum.TryParse(reference.Name.Split('.').Last(), out DxAssemblyRole target))
                    {
                        if (IsForbiddenInternalDxPackage(reference.Name))
                            c.ReportDiagnostic(Diagnostic.Create(Dxk.DXK009, Location.None, reference.Name));

                        continue;
                    }

                    if (!RoleMatrix.IsAllowed(role.Value, target))
                        c.ReportDiagnostic(Diagnostic.Create(Dxk.DXK002, Location.None, role, target));

                    if (role == DxAssemblyRole.Contracts && reference.Name.Contains("Dx.Domain.Kernel"))
                        c.ReportDiagnostic(Diagnostic.Create(Dxk.DXK007, Location.None));
                }
            });
        }

        private static bool IsForbiddenInternalDxPackage(string? name)
        {
            var normalized = name ?? string.Empty;
            if (string.IsNullOrWhiteSpace(normalized))
                return false;

            if (!normalized.StartsWith("Dx.Domain.", StringComparison.OrdinalIgnoreCase))
                return false;

            return !string.Equals(normalized, "Dx.Domain.Kernel", StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(normalized, "Dx.Domain.Primitives", StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(normalized, "Dx.Domain.Annotations", StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(normalized, "Dx.Domain.Facts", StringComparison.OrdinalIgnoreCase);
        }
    }
}
