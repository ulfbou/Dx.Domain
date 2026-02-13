// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AssemblyRoleAnalyzer.cs" company="Dx.Domain Team">
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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using Dx.Domain.Analyzers.Diagnostics;
using Dx.Domain.Analyzers.Infrastructure.Scopes;
using Dx.Domain.Analyzers.Roles;

namespace Dx.Domain.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AssemblyRoleAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Dxk.DXK001);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(c =>
            {
                var scopeResolver = new ScopeResolver(c.Options.AnalyzerConfigOptionsProvider);
                if (scopeResolver.ResolveAssembly(c.Compilation.Assembly) != Scope.S3)
                    return;

                if (RoleResolver.Resolve(c.Compilation, c.Options.AnalyzerConfigOptionsProvider) is null)
                    c.ReportDiagnostic(Diagnostic.Create(Dxk.DXK001, Location.None));
            });
        }
    }
}
