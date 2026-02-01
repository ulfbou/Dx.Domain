// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FileName.cs" company="Dx.Domain Team">
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
                if (IsKernelLikeLayer(startContext.Options.AnalyzerConfigOptionsProvider) ||
                    scope != Infrastructure.Scopes.Scope.S3 ||
                    IsKernelLikeAssembly(startContext.Compilation.AssemblyName) ||
                    IsKernelLikeCompilation(startContext.Compilation))
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

                    var assemblyName = containingSymbol.ContainingAssembly?.Name ??
                                       ctx.SemanticModel.Compilation.AssemblyName;
                    if (IsKernelLikeAssembly(assemblyName))
                        return;

                    if (IsKernelLikeLocation(containingSymbol))
                        return;

                    var filePath = ctx.Node.SyntaxTree?.FilePath;
                    if (filePath != null && IsKernelLikeAssemblyFromPath(filePath))
                        return;

                    var scope = services.Scope.ResolveSymbol(containingSymbol);
                    if (scope != Infrastructure.Scopes.Scope.S3)
                        return;

                    var role = RoleResolver.Resolve(ctx.SemanticModel.Compilation);
                    if (role is not (DxAssemblyRole.Domain or DxAssemblyRole.Application))
                        return;

                    ctx.ReportDiagnostic(Diagnostic.Create(Dxk.DXK005, ctx.Node.GetLocation(), role));
                }, SyntaxKind.ThrowStatement);
            });
        }

        private static bool IsKernelLikeAssembly(string? assemblyName)
        {
            return string.Equals(assemblyName, "Dx.Domain.Kernel", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(assemblyName, "Dx.Domain.Primitives", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(assemblyName, "Dx.Domain.Annotations", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKernelLikeAssemblyFromPath(string path)
        {
            return path.IndexOf("Dx.Domain.Kernel", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   path.IndexOf("Dx.Domain.Primitives", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   path.IndexOf("Dx.Domain.Annotations", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsKernelLikeCompilation(Compilation compilation)
        {
            return compilation.SyntaxTrees.Any(tree => IsKernelLikeAssemblyFromPath(tree.FilePath));
        }

        private static bool IsKernelLikeLayer(AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!optionsProvider.GlobalOptions.TryGetValue("build_property.DxLayer", out var layer))
                return false;

            return string.Equals(layer, "Kernel", System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(layer, "Primitives", System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(layer, "Annotations", System.StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKernelLikeLocation(ISymbol symbol)
        {
            return symbol.Locations.Any(location =>
                IsKernelLikeAssemblyFromPath(location.SourceTree?.FilePath ?? string.Empty));
        }
    }
}
