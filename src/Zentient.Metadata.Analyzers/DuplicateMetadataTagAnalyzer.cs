// <copyright file="DuplicateMetadataTagAnalyzer.cs" company="Zentient Framework Team">
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
    public class DuplicateMetadataTagAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ZMD002";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Duplicate or conflicting metadata tags",
            "Multiple MetadataTagAttribute tags with the same key and different values detected",
            "Usage",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Warns when multiple MetadataTagAttribute tags with the same key and different values are detected on a type.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var decl = (TypeDeclarationSyntax)context.Node;
            var attrs = decl.AttributeLists.SelectMany(a => a.Attributes);
            var tagGroups = attrs
                .Where(a => a.Name.ToString().Contains("MetadataTag"))
                .GroupBy(a => a.ArgumentList?.Arguments.FirstOrDefault()?.ToString());
            foreach (var group in tagGroups)
            {
                if (group.Count() > 1)
                {
                    var values = group.Select(a => a.ArgumentList?.Arguments.ElementAtOrDefault(1)?.ToString()).Distinct().ToList();
                    if (values.Count > 1)
                    {
                        foreach (var attr in group)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, attr.GetLocation()));
                        }
                    }
                }
            }
        }
    }
}
