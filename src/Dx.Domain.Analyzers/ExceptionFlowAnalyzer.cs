// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ExceptionFlowAnalyzer.cs" company="Dx.Domain Team">
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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using Dx.Domain.Analyzers.Diagnostics;
using Dx.Domain.Annotations;
using Dx.Domain.Analyzers.Roles;

namespace Dx.Domain.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ExceptionFlowAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Dxk.DXK005);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(startContext =>
            {
                var scopeResolver = new Infrastructure.Scopes.ScopeResolver(startContext.Options.AnalyzerConfigOptionsProvider);
                var scope = scopeResolver.ResolveAssembly(startContext.Compilation.Assembly);
                if (scope != Infrastructure.Scopes.Scope.S3)
                    return;

                var services = new Infrastructure.AnalyzerServices(
                    new Infrastructure.Scopes.ScopeResolver(startContext.Options.AnalyzerConfigOptionsProvider),
                    new Infrastructure.Facades.DxFacadeResolver(startContext.Compilation, startContext.Options.AnalyzerConfigOptionsProvider),
                    new Infrastructure.Facades.SemanticClassifier(startContext.Compilation),
                    new Infrastructure.Exceptions.ExceptionIntentClassifier(startContext.Compilation, startContext.Options.AnalyzerConfigOptionsProvider),
                    new Infrastructure.Flow.ResultFlowEngineWrapper(),
                    new Infrastructure.Generated.GeneratedCodeDetector(startContext.Options.AnalyzerConfigOptionsProvider));

                startContext.RegisterSyntaxNodeAction(ctx =>
                {
                    var containingSymbol = ctx.ContainingSymbol;
                    if (containingSymbol == null)
                        return;

                    var scope = services.Scope.ResolveSymbol(containingSymbol);
                    if (scope != Infrastructure.Scopes.Scope.S3)
                        return;

                    var role = RoleResolver.Resolve(ctx.SemanticModel.Compilation, ctx.Options.AnalyzerConfigOptionsProvider);
                    if (role is not (DxAssemblyRole.Domain or DxAssemblyRole.Application))
                        return;

                    ctx.ReportDiagnostic(Diagnostic.Create(Dxk.DXK005, ctx.Node.GetLocation(), role));
                }, SyntaxKind.ThrowStatement);
            });
        }

    }
}
