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
    public sealed class ResultFlowEngineWrapper
    {
        private readonly ResultFlowEngine _engine = new();
        private readonly ConcurrentDictionary<string, FlowGraph> _cache = new();

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
