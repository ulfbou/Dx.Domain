// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA065_CodeFixProvider.cs" company="Dx.Domain Team">
// Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
// This software is licensed under the MIT License.
// See the project's root <c>LICENSE</c> file for details.
// Contributions are welcome, subject to the terms of the project's license.
// See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dx.Domain.Analyzers.CodeFixes;

/// <summary>
/// Code fix provider for Dx.Domain rule DXA065: Construction Authority Violation.
/// </summary>
/// <remarks>
/// This code fix provider offers a code action to replace unresolved XML documentation
/// references with <see cref="T:System.Object"/> references for Dx.Domain types.
///
/// DX-first principle:
/// Documentation references must be compile-time verified and rename-safe.
/// </remarks>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DXA065_CodeFixProvider)), Shared]
public sealed class DXA065_CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(Analyzers.DXA065_UnresolvedXmlDocReferenceAnalyzer.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
            return;

        var diagnostic = context.Diagnostics.First();
        var span = diagnostic.Location.SourceSpan;

        var token = root.FindToken(span.Start);
        var trivia = token.Parent?.AncestorsAndSelf()
           .SelectMany(n => n.GetLeadingTrivia().Concat(n.GetTrailingTrivia()))
           .FirstOrDefault(t => t.FullSpan.Contains(span));

        if (trivia == null)
            return;

        var typeName = diagnostic.Properties.TryGetValue("TypeName", out var tn)
           ? tn
            : diagnostic.GetMessage().Split('\'')[1];

        context.RegisterCodeFix(
            CodeAction.Create(
                $"Use <see cref=\"{typeName}\"/>",
                async ct => await ReplaceWithSeeCrefAsync(context.Document, trivia.Value, span, typeName, ct),
                equivalenceKey: $"see_{typeName}"),
            diagnostic);
    }

    private static async Task<Document> ReplaceWithSeeCrefAsync(
        Document document,
        SyntaxTrivia trivia,
        TextSpan span,
        string typeName,
        CancellationToken ct)
    {
        var root = await document.GetSyntaxRootAsync(ct).ConfigureAwait(false);
        if (root == null)
            return document;

        var text = trivia.ToFullString();
        var relativeStart = span.Start - trivia.FullSpan.Start;

        var before = text.Substring(0, relativeStart);
        var after = text.Substring(relativeStart + typeName.Length);

        var crefValue = typeName == "Result" ? "Result{T}" : typeName;
        var replacement = $"<see cref=\"{crefValue}\"/>";

        var newText = before + replacement + after;
        var newTrivia = SyntaxFactory.ParseLeadingTrivia(newText).First();

        var newRoot = root.ReplaceTrivia(trivia, newTrivia);
        return document.WithSyntaxRoot(newRoot);
    }
}
