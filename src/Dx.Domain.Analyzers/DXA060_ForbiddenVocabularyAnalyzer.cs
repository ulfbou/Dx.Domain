// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA060_ForbiddenVocabularyAnalyzer.cs" company="Dx.Domain Team">
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

using Dx.Domain.Annotations;
using Dx.Domain.Analyzers.Infrastructure;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers
{
    /// <summary>
    /// Analyzer for DXA060: Forbidden Vocabulary.
    /// Detects use of forbidden pattern vocabulary in consumer code.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DXA060_ForbiddenVocabularyAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DxRuleIds.DXA060;
        private const string Category = DxCategories.DomainArchitecture;

        private static readonly LocalizableString Title =
            "Forbidden Vocabulary";
        private static readonly LocalizableString MessageFormat =
            "Forbidden vocabulary '{0}' used in consumer code. Move to adapter or rename to mechanical term.";
        private static readonly LocalizableString Description =
            "Pattern-based terms like 'Repository', 'Saga', 'Apply' belong in adapters rather than consumer core logic.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        // Forbidden vocabulary from the Dx.Domain manifesto
        private static readonly ImmutableHashSet<string> ForbiddenTerms = ImmutableHashSet.Create(
            "AggregateRoot",
            "Repository",
            "Saga",
            "Apply",
            "Handle",
            "TransitionTo",
            "Emit",
            "Publish",
            "Subscribe",
            "Command",
            "Query",
            "Event",
            "Projection",
            "ReadModel",
            "WriteModel",
            "EventStore",
            "Snapshot"
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        private static readonly char[] separator = new char[] { '\n' };

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                var scopeResolver = new ScopeResolver(startContext.Options.AnalyzerConfigOptionsProvider);
                if (scopeResolver.ResolveAssembly(startContext.Compilation.Assembly) != Scope.S3)
                    return;

                var services = CreateServices(startContext);
                ImmutableArray<string> allowList = LoadAllowList(startContext.Options.AnalyzerConfigOptionsProvider);

                startContext.RegisterSymbolAction(symbolContext =>
                {
                    AnalyzeSymbol(symbolContext, services, allowList);
                }, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);
            });
        }

        private static ImmutableArray<string> LoadAllowList(AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider)
        {
            // Retrieve the options from the analyzerConfigOptionsProvider
            var options = analyzerConfigOptionsProvider.GlobalOptions;

            // Read allowList value from global options
            if (!options.TryGetValue("allowList", out var raw) || string.IsNullOrWhiteSpace(raw))
            {
                if (!options.TryGetValue("dx_forbidden_vocab_allow", out raw) || string.IsNullOrWhiteSpace(raw))
                    return ImmutableArray<string>.Empty;
            }

            // Split the options into a list and create an ImmutableArray
            var allowList = raw.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .ToImmutableArray();

            return allowList;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "MicrosoftCodeAnalysisCorrectness",
            "RS1012:Start action has no registered actions",
            Justification = "Actions are registered in the enclosing lambda; this helper only builds services.")]
        private static AnalyzerServices CreateServices(CompilationStartAnalysisContext context)
        {
            var config = context.Options.AnalyzerConfigOptionsProvider;
            return new AnalyzerServices(
                new ScopeResolver(config),
                new DxFacadeResolver(context.Compilation, config),
                new SemanticClassifier(context.Compilation),
                new Infrastructure.Exceptions.ExceptionIntentClassifier(context.Compilation, config),
                new Infrastructure.Flow.ResultFlowEngineWrapper(),
                new GeneratedCodeDetector(config));
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context, AnalyzerServices services, ImmutableArray<string> allowList)
        {
            var symbol = context.Symbol;

            // Skip if generated code
            if (services.Generated.IsGenerated(symbol))
                return;

            var scope = services.Scope.ResolveSymbol(symbol);
            if (scope != Scope.S3)
                return;

            // Honor allow list
            var displayName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (allowList.Contains(displayName) || allowList.Contains(symbol.Name))
                return;

            // Check if symbol name contains forbidden vocabulary
            var symbolName = symbol.Name;
            var forbiddenTerm = ForbiddenTerms.FirstOrDefault(term => symbolName.Contains(term));
            if (forbiddenTerm != null)
            {
                if (symbol.Locations.Any())
                {
                    var location = symbol.Locations.First();
                    var diagnostic = Diagnostic.Create(Rule, location, forbiddenTerm);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
