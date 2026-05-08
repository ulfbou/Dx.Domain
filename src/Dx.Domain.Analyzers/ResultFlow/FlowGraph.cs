// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FlowGraph.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Represents the result-flow graph produced by analyzing a method.
    /// </summary>
    /// <remarks>
    /// This type is immutable and is used exclusively by analyzers to represent data-flow state for Result values.
    /// It carries analysis data only and imposes no runtime semantics outside compilation analysis.
    /// </remarks>
    public sealed class FlowGraph
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlowGraph"/> class with the specified nodes, states, diagnostics, and validity flag.
        /// </summary>
        /// <param name="resultNodes">The collection of result nodes in the graph.</param>
        /// <param name="nodeStates">The mapping of nodes to their analyzed states.</param>
        /// <param name="diagnostics">The diagnostics produced during analysis.</param>
        /// <param name="isValid">A value indicating whether the graph represents a successful analysis. The default is <see langword="true"/>.</param>
        public FlowGraph(
            ImmutableArray<ResultNode> resultNodes,
            ImmutableDictionary<ResultNode, ResultState> nodeStates,
            ImmutableArray<FlowDiagnostic> diagnostics,
            bool isValid = true)
        {
            ResultNodes = resultNodes;
            NodeStates = nodeStates;
            Diagnostics = diagnostics;
            IsValid = isValid;
        }

        /// <summary>Gets the collection of result nodes in the graph.</summary>
        public ImmutableArray<ResultNode> ResultNodes { get; }

        /// <summary>Gets the mapping of nodes to their analyzed states.</summary>
        public ImmutableDictionary<ResultNode, ResultState> NodeStates { get; }

        /// <summary>Gets the diagnostics produced during analysis.</summary>
        public ImmutableArray<FlowDiagnostic> Diagnostics { get; }

        /// <summary>Gets a value indicating whether the graph represents a successful analysis.</summary>
        public bool IsValid { get; }
    }
}

