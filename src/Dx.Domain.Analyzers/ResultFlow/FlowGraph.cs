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
    public sealed class FlowGraph
    {
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
        public ImmutableArray<ResultNode> ResultNodes { get; }
        public ImmutableDictionary<ResultNode, ResultState> NodeStates { get; }
        public ImmutableArray<FlowDiagnostic> Diagnostics { get; }
        public bool IsValid { get; }
    }
}
