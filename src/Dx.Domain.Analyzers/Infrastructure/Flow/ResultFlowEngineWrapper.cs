// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ResultFlowEngineWrapper.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Analyzers.ResultFlow;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Dx.Domain.Analyzers.Infrastructure.Flow
{
    /// <summary>
    /// Provides a cached wrapper around <see cref="ResultFlowEngine"/> for analyzing result-flow graphs during compilation.
    /// </summary>
    /// <remarks>
    /// This type is used exclusively by analyzers. It caches analysis results by method signature and syntax checksum to avoid redundant work.
    /// Analysis failures are handled fail-open by returning an invalid empty <see cref="FlowGraph"/>.
    /// This type is thread-safe for concurrent analyzer execution.
    /// </remarks>
    public sealed class ResultFlowEngineWrapper
    {
        private readonly ResultFlowEngine _engine = new();
        private readonly ConcurrentDictionary<string, FlowGraph> _cache = new();

        /// <summary>
        /// Analyzes the result flow for the specified method, returning a cached graph when available.
        /// </summary>
        /// <param name="method">The method symbol to analyze.</param>
        /// <param name="compilation">The compilation containing the method.</param>
        /// <param name="options">The analyzer configuration options.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The <see cref="FlowGraph"/> for <paramref name="method"/>. If analysis fails, returns an invalid empty graph.</returns>
        /// <remarks>
        /// Results are cached using a key composed of the fully qualified method display string and the syntax tree checksum.
        /// The method never throws; exceptions during analysis result in a fail-open invalid graph with <c>isValid: false</c>.
        /// </remarks>
        public FlowGraph Analyze(
            IMethodSymbol method,
            Compilation compilation,
            AnalyzerConfigOptions options,
            CancellationToken ct)
        {
            try
            {
                var key = BuildKey(method);
                return _cache.GetOrAdd(key,
                    _ => _engine.Analyze(method, compilation, options, ct));
            }
            catch
            {
                // Fail-open: return invalid empty graph on any error
                return new FlowGraph(
                    ImmutableArray<ResultNode>.Empty,
                    ImmutableDictionary<ResultNode, ResultState>.Empty,
                    ImmutableArray<FlowDiagnostic>.Empty,
                    isValid: false);
            }
        }

        private static string BuildKey(IMethodSymbol method)
        {
            var syntax = method.DeclaringSyntaxReferences.First();
            var tree = syntax.SyntaxTree;
            var checksumBytes = tree.GetText().GetChecksum();
            var checksum = BitConverter.ToString(checksumBytes.ToArray()).Replace("-", string.Empty);

            var symbolId = method.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            return $"{symbolId}::{checksum}";
        }
    }

}
