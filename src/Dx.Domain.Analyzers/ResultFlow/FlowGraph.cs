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
    /// Represents the complete flow graph for a method.
    /// </summary>
    /// <remarks>
    /// Contains all discovered Result nodes, their final states, and any analysis diagnostics. Graphs are immutable.
    /// </remarks>
    public sealed class FlowGraph
    {
        /// <summary>Initializes a new instance of the <see cref="FlowGraph"/> class.</summary>
        /// <param name="resultNodes">The discovered Result nodes.</param>
        /// <param name="nodeStates">The final state for each node.</param>
        /// <param name="diagnostics">Analysis diagnostics.</param>
        /// <param name="isValid">Indicates whether analysis completed successfully.</param>
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

        /// <summary>Gets the Result nodes.</summary>
        public ImmutableArray<ResultNode> ResultNodes { get; }

        /// <summary>Gets the final states.</summary>
        public ImmutableDictionary<ResultNode, ResultState> NodeStates { get; }

        /// <summary>Gets the diagnostics.</summary>
        public ImmutableArray<FlowDiagnostic> Diagnostics { get; }

        /// <summary>Gets a value indicating whether the graph is valid.</summary>
        public bool IsValid { get; }
    }
}
