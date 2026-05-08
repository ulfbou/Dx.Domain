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

using System.Threading;

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Defines a contract for analyzing result-flow graphs during compilation.
    /// </summary>
    /// <remarks>
    /// This interface is used exclusively by analyzers. Implementations analyze data-flow state for Result values and produce a <see cref="FlowGraph"/>.
    /// It imposes no runtime semantics outside compilation analysis.
    /// </remarks>
    public interface IResultFlowEngine
    {
        /// <summary>
        /// Analyzes the result flow for the specified method.
        /// </summary>
        /// <param name="method">The method symbol to analyze.</param>
        /// <param name="compilation">The compilation containing the method.</param>
        /// <param name="options">The analyzer configuration options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="FlowGraph"/> representing the analyzed result flow.</returns>
        FlowGraph Analyze(
            IMethodSymbol method,
            Compilation compilation,
            AnalyzerConfigOptions options,
            CancellationToken cancellationToken);
    }
}
