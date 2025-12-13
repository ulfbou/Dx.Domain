// <copyright file="MissingDocumentationAnalyzer.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Zentient.Metadata.Analyzers
{
    [DiagnosticAnalyzer("CSharp")]
    public class MissingDocumentationAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic ID for missing documentation.
        /// </summary>
        public const string DiagnosticId = "ZMD004";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Missing XML documentation",
            "Public API '{0}' is missing XML documentation",
            "Documentation",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "Warns when a public API is missing XML documentation.");

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);
        }

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = context.Symbol;
            if (symbol.DeclaredAccessibility == Accessibility.Public && string.IsNullOrWhiteSpace(symbol.GetDocumentationCommentXml()))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name));
            }
        }
    }
}
