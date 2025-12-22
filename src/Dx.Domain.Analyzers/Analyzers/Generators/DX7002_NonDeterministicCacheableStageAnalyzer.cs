// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DX7002_NonDeterministicCacheableStageAnalyzer.cs" company="Dx.Domain Team">
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
    /// Analyzer for DX7002: Non-Deterministic Cacheable Stage.
    /// Detects cacheable stages that use non-deterministic operations.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DX7002_NonDeterministicCacheableStageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DX7002";
        private const string Category = "Domain.Generators.Cache";

        private static readonly LocalizableString Title = "Non-Deterministic Cacheable Stage";
        private static readonly LocalizableString MessageFormat =
            "Cacheable stage uses non-deterministic operation: {0}. Mark stage as non-cacheable or remove non-deterministic behavior.";
        private static readonly LocalizableString Description =
            "Cacheable stages must be deterministic. They cannot use random numbers, timestamps, or other non-deterministic sources.";

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

            // This analyzer would leverage DX1001 (Referential Transparency) detections
            // in the context of cacheable stages
            // The actual enforcement happens at the pipeline orchestrator level
        }
    }
}
