// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ResultFlowEngine.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Performs control-flow-based analysis of Result values within methods.
    /// </summary>
    /// <remarks>
    /// Discovers Result producers, tracks propagation through locals and parameters, and promotes states based on inspection, return, and handler usage. Analysis is fail-open and returns an empty graph on error.
    /// </remarks>
    public sealed class ResultFlowEngine : IResultFlowEngine
    {
        private readonly ResultFlowEngineOptions _options;

        /// <summary>Initializes a new instance of the <see cref="ResultFlowEngine"/> class.</summary>
        /// <param name="options">Optional engine options. Defaults are used when null.</param>
        public ResultFlowEngine(ResultFlowEngineOptions? options = null)
        {
            _options = options ?? ResultFlowEngineOptions.Default;
        }

        /// <inheritdoc/>
        public FlowGraph Analyze(
            IMethodSymbol method,
            Compilation compilation,
            AnalyzerConfigOptions options,
            CancellationToken cancellationToken)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));

            cancellationToken.ThrowIfCancellationRequested();
            var model = compilation.GetSemanticModel(method.DeclaringSyntaxReferences.First().SyntaxTree);
            var body = method.DeclaringSyntaxReferences.First().GetSyntax(cancellationToken);
            var operation = model.GetOperation(body, cancellationToken) as IBlockOperation;

            if (operation is null)
            {
                return new FlowGraph(
                ImmutableArray<ResultNode>.Empty,
                ImmutableDictionary<ResultNode, ResultState>.Empty,
                ImmutableArray<FlowDiagnostic>.Empty);
            }

            var cfg = ControlFlowGraph.Create(operation, CancellationToken.None);

            if (cfg is null)
            {
                return new FlowGraph(
                    ImmutableArray<ResultNode>.Empty,
                    ImmutableDictionary<ResultNode, ResultState>.Empty,
                    ImmutableArray<FlowDiagnostic>.Empty);
            }

            var context = new AnalysisContext(method, compilation, model, options, _options, cancellationToken);
            var analyzer = new MethodFlowAnalyzer(context, cfg);
            return analyzer.Run();
        }

        private sealed class AnalysisContext
        {
            public AnalysisContext(
            IMethodSymbol method,
            Compilation compilation,
            SemanticModel semanticModel,
            AnalyzerConfigOptions options,
            ResultFlowEngineOptions engineOptions,
            CancellationToken cancellationToken)
            {
                Method = method;
                Compilation = compilation;
                SemanticModel = semanticModel;
                Options = options;
                EngineOptions = engineOptions;
                CancellationToken = cancellationToken;
                ResultTypeResolver = new ResultTypeResolver(compilation, options, engineOptions);
                HandlerRegistry = new HandlerRegistry(compilation, options, engineOptions);
            }
            public IMethodSymbol Method { get; }
            public Compilation Compilation { get; }
            public SemanticModel SemanticModel { get; }
            public AnalyzerConfigOptions Options { get; }
            public ResultFlowEngineOptions EngineOptions { get; }
            public CancellationToken CancellationToken { get; }
            public ResultTypeResolver ResultTypeResolver { get; }
            public HandlerRegistry HandlerRegistry { get; }
        }
        private sealed class MethodFlowAnalyzer
        {
            private readonly AnalysisContext _ctx;
            private readonly ControlFlowGraph _cfg;
            private readonly List<ResultNode> _nodes = new();
            private readonly Dictionary<IOperation, ResultNode> _producerToNode = new Dictionary<IOperation, ResultNode>();
            private readonly List<FlowDiagnostic> _diagnostics = new();
            public MethodFlowAnalyzer(AnalysisContext ctx, ControlFlowGraph cfg)
            {
                _ctx = ctx;
                _cfg = cfg;
            }
            public FlowGraph Run()
            {
                _ctx.CancellationToken.ThrowIfCancellationRequested();
                DiscoverProducers();
                AnalyzeUsage();
                var nodeStates = _nodes.ToDictionary(
                n => n,
                n => n.State == ResultState.Created ? ResultState.Ignored : n.State);
                return new FlowGraph(
                _nodes.ToImmutableArray(),
                nodeStates.ToImmutableDictionary(),
                _diagnostics.ToImmutableArray());
            }
            private void DiscoverProducers()
            {
                foreach (var block in _cfg.Blocks)
                {
                    foreach (var op in block.Operations)
                    {
                        DiscoverProducersInOperation(op);
                    }
                }
            }
            private void DiscoverProducersInOperation(IOperation op)
            {
                if (op == null)
                    return;
                switch (op)
                {
                    case IInvocationOperation invocation
                when _ctx.ResultTypeResolver.IsResultType(invocation.Type):
                        RegisterNode(invocation);
                        break;
                    case IObjectCreationOperation creation
                when _ctx.ResultTypeResolver.IsResultType(creation.Type):
                        RegisterNode(creation);
                        break;
                    case IPropertyReferenceOperation propertyRef
                when _ctx.ResultTypeResolver.IsResultType(propertyRef.Type):
                        RegisterNode(propertyRef);
                        break;
                }
                foreach (var child in op.ChildOperations)
                {
                    DiscoverProducersInOperation(child);
                }
            }
            private ResultNode RegisterNode(IOperation producer)
            {
                if (_producerToNode.TryGetValue(producer, out var existing))
                    return existing;
                var node = new ResultNode(
                id: _nodes.Count,
                producer: producer,
                type: producer.Type!);
                node.State = ResultState.Created;
                _nodes.Add(node);
                _producerToNode.Add(producer, node);
                return node;
            }
            private void AnalyzeUsage()
            {
                foreach (var block in _cfg.Blocks)
                {
                    foreach (var op in block.Operations)
                    {
                        AnalyzeOperationUsage(op);
                    }
                    if (block.BranchValue is { } branch)
                    {
                        AnalyzeOperationUsage(branch);
                    }
                }
            }
            private void AnalyzeOperationUsage(IOperation op)
            {
                if (op is null)
                    return;
                switch (op)
                {
                    case IReturnOperation ret:
                        HandleReturn(ret);
                        break;
                    case IInvocationOperation invocation:
                        HandleInvocation(invocation);
                        break;
                    case IConditionalAccessOperation or
                IConditionalAccessInstanceOperation or
                IConditionalOperation or
                IIsPatternOperation or
                IIsTypeOperation:
                        HandleCondition(op);
                        break;
                }
                foreach (var child in op.ChildOperations)
                {
                    AnalyzeOperationUsage(child);
                }
            }
            private void HandleReturn(IReturnOperation ret)
            {
                if (ret.ReturnedValue is null)
                    return;
                var value = ret.ReturnedValue;
                var node = FindNodeFor(value);
                if (node is null)
                    return;
                PromoteState(node, ResultState.Propagated);
            }
            private void HandleInvocation(IInvocationOperation invocation)
            {
                if (_ctx.ResultTypeResolver.IsResultType(invocation.Type))
                {
                    RegisterNode(invocation);
                }
                foreach (var arg in invocation.Arguments)
                {
                    var node = FindNodeFor(arg.Value);
                    if (node is null)
                        continue;
                    var isTerminal = _ctx.HandlerRegistry.IsTerminalizer(invocation.TargetMethod);
                    var isHandler = _ctx.HandlerRegistry.IsHandler(invocation.TargetMethod);
                    if (isTerminal)
                    {
                        PromoteState(node, ResultState.Terminated);
                    }
                    else if (isHandler)
                    {
                        PromoteState(node, ResultState.Propagated);
                    }
                    else
                    {
                    }
                }
            }
            private void HandleCondition(IOperation op)
            {
                foreach (var descendant in op.Descendants())
                {
                    if (descendant is IInvocationOperation invocation &&
                    invocation.Instance is { } instance &&
                    _ctx.ResultTypeResolver.IsResultLikeInstance(instance))
                    {
                        var node = FindNodeFor(instance);
                        if (node is not null)
                        {
                            PromoteState(node, ResultState.Checked);
                        }
                    }
                    if (descendant is IPropertyReferenceOperation property &&
                    property.Instance is { } instance2 &&
                    _ctx.ResultTypeResolver.IsResultLikeInstance(instance2))
                    {
                        var node = FindNodeFor(instance2);
                        if (node is not null)
                        {
                            PromoteState(node, ResultState.Checked);
                        }
                    }
                }
            }
            private ResultNode? FindNodeFor(IOperation value)
            {
                if (_producerToNode.TryGetValue(value, out var direct))
                    return direct;
                switch (value)
                {
                    case ILocalReferenceOperation localRef:
                        return FindNodeThroughLocal(localRef.Local);
                    case IParameterReferenceOperation pRef:
                        return FindNodeThroughParameter(pRef.Parameter);
                    case IConversionOperation conv:
                        return FindNodeFor(conv.Operand);
                    case IParenthesizedOperation paren:
                        return FindNodeFor(paren.Operand);
                }
                return null;
            }
            private ResultNode? FindNodeThroughLocal(ILocalSymbol local)
            {
                foreach (var block in _cfg.Blocks)
                {
                    foreach (var op in block.Operations)
                    {
                        foreach (var descendant in op.DescendantsAndSelf())
                        {
                            if (descendant is ISimpleAssignmentOperation assignment &&
                            assignment.Target is ILocalReferenceOperation localRef &&
                            SymbolEqualityComparer.Default.Equals(localRef.Local, local))
                            {
                                if (_producerToNode.TryGetValue(assignment.Value, out var node))
                                    return node;
                            }
                        }
                    }
                }
                return null;
            }
            private ResultNode? FindNodeThroughParameter(IParameterSymbol parameter)
            {
                // Walk all blocks and look for simple assignments where a parameter is assigned into a local,
                // then try to resolve the node for the assigned value. This mirrors the local tracking logic
                // and gives the engine a basic ability to follow parameter-originated results.
                foreach (var block in _cfg.Blocks)
                {
                    foreach (var op in block.Operations)
                    {
                        foreach (var descendant in op.DescendantsAndSelf())
                        {
                            if (descendant is ISimpleAssignmentOperation assignment)
                            {
                                // Case 1: parameter is assigned into a local, reuse local resolution.
                                if (assignment.Value is IParameterReferenceOperation paramRef &&
                                    SymbolEqualityComparer.Default.Equals(paramRef.Parameter, parameter) &&
                                    assignment.Target is ILocalReferenceOperation localTarget)
                                {
                                    var viaLocal = FindNodeThroughLocal(localTarget.Local);
                                    if (viaLocal is not null)
                                        return viaLocal;
                                }

                                // Case 2: parameter is on the right-hand side of an assignment we already track.
                                if (assignment.Value is IParameterReferenceOperation paramRef2 &&
                                    SymbolEqualityComparer.Default.Equals(paramRef2.Parameter, parameter) &&
                                    _producerToNode.TryGetValue(assignment.Value, out var direct))
                                {
                                    return direct;
                                }
                            }
                        }
                    }
                }

                return null;
            }
            private static void PromoteState(ResultNode node, ResultState newState)
            {
                if (newState <= node.State)
                    return;
                node.State = newState;
            }
        }
    }
    /// <summary>
    /// Provides configuration for the Result flow engine.
    /// </summary>
    public sealed class ResultFlowEngineOptions
    {
        /// <summary>Gets the default options.</summary>
        public static ResultFlowEngineOptions Default { get; } = new();

        /// <summary>Gets the metadata names recognized as Result types.</summary>
        public ImmutableHashSet<string> ResultTypeMetadataNames { get; init; } =
            ImmutableHashSet.Create(
            "Dx.Domain.Result",
            "Dx.Domain.Result`1");

        /// <summary>Gets the member names considered inspections.</summary>
        public ImmutableHashSet<string> InspectionMemberNames { get; init; } =
            ImmutableHashSet.Create("IsSuccess", "IsFailure", "Match", "Map", "Bind");

        /// <summary>Gets the configuration key for handlers.</summary>
        public string HandlerConfigKey { get; init; } = "dx.result.handlers";

        /// <summary>Gets the configuration key for terminalizers.</summary>
        public string TerminalizerConfigKey { get; init; } = "dx.result.terminalizers";
    }
}
