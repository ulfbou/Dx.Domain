// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IResultFlowEngine.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Defines the contract for Result flow analysis.
    /// </summary>
    public interface IResultFlowEngine
    {
        /// <summary>Analyzes the specified method for Result flow.</summary>
        /// <param name="method">The method to analyze.</param>
        /// <param name="compilation">The compilation context.</param>
        /// <param name="options">Analyzer options.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The resulting flow graph.</returns>
        FlowGraph Analyze(
            IMethodSymbol method,
            Compilation compilation,
            AnalyzerConfigOptions options,
            CancellationToken cancellationToken);
    }
}
