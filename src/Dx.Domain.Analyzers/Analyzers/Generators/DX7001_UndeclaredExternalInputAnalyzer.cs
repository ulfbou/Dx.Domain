// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DX7001_UndeclaredExternalInputAnalyzer.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Analyzers.Analyzers.Generators
{
    /// <summary>
    /// Analyzer for DX7001: Undeclared External Input (Sandbox Leak).
    /// Detects cacheable stages or general stages that access external resources (File I/O, Network, Database) 
    /// without declaring the corresponding capability in their StageDeclaration or IGeneratorStage.Capabilities.
    /// Updated per DX-002 spec to enforce the sandbox.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DX7001_UndeclaredExternalInputAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DX7001";
        private const string Category = "Domain.Generators.Cache";

        private static readonly LocalizableString Title = "Undeclared External Input";
        private static readonly LocalizableString MessageFormat =
            "Cacheable stage accesses external resource: {0}. Declare input or mark stage as non-cacheable.";
        private static readonly LocalizableString Description =
            "Stages must not read external resources (files, network, databases) without declaring the corresponding capability. " +
            "This enforces the sandbox per DX-002 and ensures proper cache invalidation.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: "https://github.com/ulfbou/Dx-Framework/blob/main/docs/internal/dx.domain.generators.md#43-cache-rules");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);

            if (symbolInfo.Symbol is not IMethodSymbol method)
                return;

            var containingType = method.ContainingType?.ToDisplayString();

            // Check for File I/O operations
            if (IsFileIoMethod(containingType, method.Name))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    invocation.GetLocation(),
                    $"{containingType}.{method.Name}()");
                context.ReportDiagnostic(diagnostic);
            }

            // Check for network operations
            if (IsNetworkMethod(containingType, method.Name))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    invocation.GetLocation(),
                    $"{containingType}.{method.Name}()");
                context.ReportDiagnostic(diagnostic);
            }

            // Check for database operations
            if (IsDatabaseMethod(containingType, method.Name))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    invocation.GetLocation(),
                    $"{containingType}.{method.Name}()");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsFileIoMethod(string? containingType, string methodName)
        {
            if (containingType == null)
                return false;

            return containingType switch
            {
                "System.IO.File" => true,
                "System.IO.FileInfo" => true,
                "System.IO.Directory" => true,
                "System.IO.DirectoryInfo" => true,
                "System.IO.Path" when methodName == "GetTempPath" || methodName == "GetTempFileName" => true,
                _ => false
            };
        }

        private static bool IsNetworkMethod(string? containingType, string methodName)
        {
            if (containingType == null)
                return false;

            return containingType.StartsWith("System.Net.", System.StringComparison.Ordinal) ||
                   containingType.StartsWith("System.Net.Http.", System.StringComparison.Ordinal);
        }

        private static bool IsDatabaseMethod(string? containingType, string methodName)
        {
            if (containingType == null)
                return false;

            return containingType.Contains(".Data.") ||
                   containingType.Contains(".EntityFramework") ||
                   containingType.Contains(".Dapper");
        }
    }
}
