using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Zentient.Metadata.Analyzers
{
    /// <summary>
    /// Analyzer that detects invalid usage of Zentient.Metadata attributes on unsupported targets.
    /// </summary>
    [DiagnosticAnalyzer("CSharp")]
    public class InvalidAttributeTargetAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ZMD001";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Invalid attribute target for Zentient.Metadata attribute",
            "Attribute '{0}' cannot be applied to this target",
            "Usage",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Warns when a Zentient.Metadata attribute is applied to an unsupported target. For example, [MetadataTag] on a method or property when not allowed."
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var attributes = node switch
            {
                ClassDeclarationSyntax cls => cls.AttributeLists.SelectMany(a => a.Attributes),
                InterfaceDeclarationSyntax iface => iface.AttributeLists.SelectMany(a => a.Attributes),
                MethodDeclarationSyntax method => method.AttributeLists.SelectMany(a => a.Attributes),
                PropertyDeclarationSyntax prop => prop.AttributeLists.SelectMany(a => a.Attributes),
                _ => Enumerable.Empty<AttributeSyntax>()
            };

            foreach (var attr in attributes)
            {
                var name = attr.Name.ToString();
                // Check for Zentient.Metadata attributes
                if (name.Contains("BehaviorDefinition") && node is not ClassDeclarationSyntax && node is not InterfaceDeclarationSyntax)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, attr.GetLocation(), "BehaviorDefinitionAttribute"));
                }
                if (name.Contains("CategoryDefinition") && node is not ClassDeclarationSyntax && node is not InterfaceDeclarationSyntax)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, attr.GetLocation(), "CategoryDefinitionAttribute"));
                }
                if (name.Contains("MetadataTag") && node is not ClassDeclarationSyntax && node is not PropertyDeclarationSyntax && node is not MethodDeclarationSyntax)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, attr.GetLocation(), "MetadataTagAttribute"));
                }
            }
        }
    }
}
