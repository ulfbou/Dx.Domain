// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DX1002_MonotonicKnowledgeAnalyzer.cs" company="Dx.Domain Team">
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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers.Analyzers.Generators
{
    /// <summary>
    /// Analyzer for DX1002: Monotonic Knowledge.
    /// Detects contradictions between pipeline stage assertions.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DX1002_MonotonicKnowledgeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DX1002";
        private const string Category = "Domain.Generators.Invariants";

        private static readonly LocalizableString Title = "Monotonic Knowledge Violation";
        private static readonly LocalizableString MessageFormat =
            "Stage assertion contradicts prior stage: {0}";
        private static readonly LocalizableString Description =
            "Pipeline stages may add derived facts but must not contradict facts emitted by earlier stages.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: "https://github.com/ulfbou/Dx-Framework/blob/main/docs/internal/dx.domain.generators.md#12-monotonic-knowledge");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // This analyzer would require runtime pipeline context
            // Actual implementation would be in the generator runtime/orchestrator
            // Here we provide the diagnostic descriptor for use by the runtime
        }
    }
}
