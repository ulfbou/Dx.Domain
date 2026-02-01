// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DomainPurityAnalyzer.cs" company="Dx.Domain Team">
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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Dx.Domain.Analyzers.Diagnostics;
using Dx.Domain.Analyzers.Roles;

namespace Dx.Domain.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DomainPurityAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string[] ForbiddenNamespaces =
        {
            "Microsoft.EntityFrameworkCore",
            "System.Data",
            "System.Net",
            "Newtonsoft.Json",
            "Microsoft.AspNetCore"
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Dxk.DXK003);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(ctx =>
            {
                var role = RoleResolver.Resolve(ctx.SemanticModel.Compilation);
                if (role != DxAssemblyRole.Domain)
                    return;

                var symbol = ctx.SemanticModel.GetSymbolInfo(ctx.Node).Symbol;
                var ns = symbol?.ContainingNamespace?.ToString();

                if (ns != null && ForbiddenNamespaces.Any(f => ns.StartsWith(f, System.StringComparison.Ordinal)))
                    ctx.ReportDiagnostic(Diagnostic.Create(Dxk.DXK003, ctx.Node.GetLocation(), ns));

            }, SyntaxKind.IdentifierName);
        }
    }
}
