// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DX1003_NoSemanticGuessingAnalyzer.cs" company="Dx.Domain Team">
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
    /// Analyzer for DX1003: No Semantic Guessing.
    /// Detects ambiguous intent that requires explicit resolution.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DX1003_NoSemanticGuessingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DX1003";
        private const string Category = "Domain.Generators.Invariants";

        private static readonly LocalizableString Title = "No Semantic Guessing Violation";
        private static readonly LocalizableString MessageFormat =
            "Ambiguous intent requires explicit resolution: {0}";
        private static readonly LocalizableString Description =
            "If intent is ambiguous and no policy resolves it, the generator must fail. Silent defaults and heuristics are forbidden.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: "https://github.com/ulfbou/Dx-Framework/blob/main/docs/internal/dx.domain.generators.md#13-no-semantic-guessing");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // This analyzer would require semantic analysis of generator configurations
            // Actual implementation would be in the generator runtime
            // Here we provide the diagnostic descriptor for use by the runtime
        }
    }
}
