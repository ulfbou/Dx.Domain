// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DX1001_ReferentialTransparencyAnalyzer.cs" company="Dx.Domain Team">
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
    /// Analyzer for DX1001: Referential Transparency.
    /// Detects non-deterministic inputs in generator code that would violate referential transparency.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DX1001_ReferentialTransparencyAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DX1001";
        private const string Category = "Domain.Generators.Invariants";

        private static readonly LocalizableString Title = "Referential Transparency Violation";
        private static readonly LocalizableString MessageFormat =
            "Non-deterministic input detected: {0}. Replace with canonicalized or fingerprint-derived value.";
        private static readonly LocalizableString Description =
            "Generators must be deterministic. Forbidden inputs include timestamps, machine paths, random GUIDs, and undeclared environment variables.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: "https://github.com/ulfbou/Dx-Framework/blob/main/docs/internal/dx.domain.generators.md#11-referential-transparency");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;
            var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccess);
            
            if (symbolInfo.Symbol is not IPropertySymbol property)
                return;

            // Check for forbidden DateTime/DateTimeOffset usage
            var propertyName = property.Name;
            var containingType = property.ContainingType?.ToDisplayString();

            if (IsForbiddenTimeProperty(containingType, propertyName))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    memberAccess.GetLocation(),
                    $"{containingType}.{propertyName}");
                context.ReportDiagnostic(diagnostic);
            }

            // Check for Environment variables
            if (containingType == "System.Environment" && 
                (propertyName == "MachineName" || 
                 propertyName == "UserName" ||
                 propertyName == "CurrentDirectory"))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    memberAccess.GetLocation(),
                    $"{containingType}.{propertyName}");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);

            if (symbolInfo.Symbol is not IMethodSymbol method)
                return;

            var methodName = method.Name;
            var containingType = method.ContainingType?.ToDisplayString();

            // Check for Guid.NewGuid()
            if (containingType == "System.Guid" && methodName == "NewGuid")
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    invocation.GetLocation(),
                    $"{containingType}.{methodName}()");
                context.ReportDiagnostic(diagnostic);
            }

            // Check for Random usage
            if (containingType?.StartsWith("System.Random", System.StringComparison.Ordinal) == true)
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    invocation.GetLocation(),
                    $"{containingType}.{methodName}()");
                context.ReportDiagnostic(diagnostic);
            }

            // Check for Environment.GetEnvironmentVariable
            if (containingType == "System.Environment" && methodName == "GetEnvironmentVariable")
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    invocation.GetLocation(),
                    $"{containingType}.{methodName}()");
                context.ReportDiagnostic(diagnostic);
            }

            // Check for Thread.Sleep (breaks determinism/performance)
            if (containingType == "System.Threading.Thread" && methodName == "Sleep")
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    invocation.GetLocation(),
                    $"{containingType}.{methodName}()");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsForbiddenTimeProperty(string? containingType, string propertyName)
        {
            if (containingType == null)
                return false;

            return (containingType == "System.DateTime" || containingType == "System.DateTimeOffset") &&
                   (propertyName == "Now" || 
                    propertyName == "UtcNow" || 
                    propertyName == "Today");
        }
    }
}
