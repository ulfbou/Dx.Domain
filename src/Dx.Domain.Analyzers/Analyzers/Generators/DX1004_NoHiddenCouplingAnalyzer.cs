// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DX1004_NoHiddenCouplingAnalyzer.cs" company="Dx.Domain Team">
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
    /// Analyzer for DX1004: No Hidden Coupling.
    /// Detects stages attempting to access facts from PriorFacts unless those keys are listed
    /// in the stage's DeclaredDependencies, per DX-001 and DX-002.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DX1004_NoHiddenCouplingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DX1004";
        private const string Category = "Domain.Generators.Invariants";

        private static readonly LocalizableString Title = "No Hidden Coupling Violation";
        private static readonly LocalizableString MessageFormat =
            "Undeclared dependency detected: Stage accesses fact '{0}' without declaring it in DeclaredDependencies.";
        private static readonly LocalizableString Description =
            "Generator stages must not rely on undeclared side effects from other stages. " +
            "All dependencies and capabilities must be declared per DX-001 and DX-002.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: "https://github.com/ulfbou/Dx-Framework/blob/main/docs/internal/dx-001.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // This analyzer requires runtime pipeline context to validate actual fact access
            // The diagnostic descriptor is provided for use by the runtime/orchestrator
            // A full implementation would analyze calls to context.PriorFacts[key] or 
            // context.PriorFacts.TryGetValue(key, ...) and validate against DeclaredDependencies
        }
    }
}
