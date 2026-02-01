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
using Dx.Domain.Analyzers.Roles;

namespace Dx.Domain.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DependencyAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Dxk.DXK002, Dxk.DXK007);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(c =>
            {
                if (IsKernelLikeLayer(c.Options.AnalyzerConfigOptionsProvider))
                    return;

                var assemblyName = c.Compilation.AssemblyName;
                if (assemblyName != null &&
                    (assemblyName.Equals("Dx.Domain.Kernel", StringComparison.OrdinalIgnoreCase) ||
                     assemblyName.Equals("Dx.Domain.Primitives", StringComparison.OrdinalIgnoreCase) ||
                     assemblyName.Equals("Dx.Domain.Annotations", StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }

                if (c.Compilation.SyntaxTrees.Any(tree => IsKernelLikePath(tree.FilePath)))
                {
                    return;
                }

                var role = RoleResolver.Resolve(c.Compilation);
                if (role is null)
                    return;

                var scopeResolver = new Infrastructure.Scopes.ScopeResolver(c.Options.AnalyzerConfigOptionsProvider);
                if (scopeResolver.ResolveAssembly(c.Compilation.Assembly) == Infrastructure.Scopes.Scope.S3)
                    return;

                foreach (var reference in c.Compilation.ReferencedAssemblyNames)
                {
                    if (!Enum.TryParse(reference.Name.Split('.').Last(), out DxAssemblyRole target))
                        continue;

                    if (!RoleMatrix.IsAllowed(role.Value, target))
                        c.ReportDiagnostic(Diagnostic.Create(Dxk.DXK002, Location.None, role, target));

                    if (role == DxAssemblyRole.Contracts && reference.Name.Contains("Dx.Domain.Kernel"))
                        c.ReportDiagnostic(Diagnostic.Create(Dxk.DXK007, Location.None));
                }
            });
        }

        private static bool IsKernelLikeLayer(AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!optionsProvider.GlobalOptions.TryGetValue("build_property.DxLayer", out var layer))
                return false;

            return string.Equals(layer, "Kernel", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(layer, "Primitives", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(layer, "Annotations", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKernelLikePath(string? path)
        {
            return path != null &&
                   (path.IndexOf("Dx.Domain.Kernel", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    path.IndexOf("Dx.Domain.Primitives", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    path.IndexOf("Dx.Domain.Annotations", StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
