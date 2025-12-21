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
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers.ResultFlow
{
    public enum ResultState
    {
        Created = 0,
        Checked = 1,
        Propagated = 2,
        Terminated = 3,
        Ignored = 4
    }
    [DebuggerDisplay("{Id} {Type.Name} State={State}")]
    public sealed class ResultNode : IEquatable<ResultNode>
    {
        public ResultNode(int id, IOperation producer, ITypeSymbol type)
        {
            Id = id;
            Producer = producer ?? throw new ArgumentNullException(nameof(producer));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
        public int Id { get; }
        public IOperation Producer { get; }
        public ITypeSymbol Type { get; }
        internal ResultState State { get; set; }
        public bool Equals(ResultNode? other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null)
                return false;
            return Id == other.Id;
        }
        public override bool Equals(object? obj) => Equals(obj as ResultNode);
        public override int GetHashCode() => Id;
        public override string ToString() => $"ResultNode#{Id} Type={Type.ToDisplayString()} State={State}";
    }
    [DebuggerDisplay("{Message}")]
    public sealed class FlowDiagnostic
    {
        public FlowDiagnostic(string message, IOperation? operation = null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Operation = operation;
        }
        public string Message { get; }
        public IOperation? Operation { get; }
    }
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
    public interface IResultFlowEngine
    {
        FlowGraph Analyze(
        IMethodSymbol method,
        Compilation compilation,
        AnalyzerConfigOptions options,
        CancellationToken cancellationToken);
    }
    public sealed class ResultFlowEngine : IResultFlowEngine
    {
        private readonly ResultFlowEngineOptions _options;
        public ResultFlowEngine(ResultFlowEngineOptions? options = null)
        {
            _options = options ?? ResultFlowEngineOptions.Default;
        }
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
    public sealed class ResultFlowEngineOptions
    {
        public static ResultFlowEngineOptions Default { get; } = new();
        public ImmutableHashSet<string> ResultTypeMetadataNames { get; init; } =
        ImmutableHashSet.Create(
        "Dx.Domain.Result",
        "Dx.Domain.Result`1");
        public ImmutableHashSet<string> InspectionMemberNames { get; init; } =
        ImmutableHashSet.Create("IsSuccess", "IsFailure", "Match", "Map", "Bind");
        public string HandlerConfigKey { get; init; } = "dx.result.handlers";
        public string TerminalizerConfigKey { get; init; } = "dx.result.terminalizers";
    }
    internal sealed class ResultTypeResolver
    {
        private readonly Compilation _compilation;
        private readonly AnalyzerConfigOptions _options;
        private readonly ResultFlowEngineOptions _engineOptions;
        private ImmutableHashSet<INamedTypeSymbol>? _resultTypesCache;
        public ResultTypeResolver(Compilation compilation, AnalyzerConfigOptions options, ResultFlowEngineOptions engineOptions)
        {
            _compilation = compilation;
            _options = options;
            _engineOptions = engineOptions;
        }
        private ImmutableHashSet<INamedTypeSymbol> GetResultTypes()
        {
            if (_resultTypesCache is not null)
                return _resultTypesCache;
            var builder = ImmutableHashSet.CreateBuilder<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            foreach (var metadataName in _engineOptions.ResultTypeMetadataNames)
            {
                if (_compilation.GetTypeByMetadataName(metadataName) is { } symbol)
                {
                    builder.Add(symbol);
                }
            }
            _resultTypesCache = builder.ToImmutable();
            return _resultTypesCache;
        }
        public bool IsResultType(ITypeSymbol? type)
        {
            if (type is null)
                return false;
            if (type is IErrorTypeSymbol)
                return false;
            if (type is not INamedTypeSymbol named)
                return false;
            var resultTypes = GetResultTypes();
            foreach (var result in resultTypes)
            {
                if (SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, result))
                    return true;
            }
            return false;
        }
        public bool IsResultLikeInstance(IOperation instance)
        {
            return IsResultType(instance.Type);
        }
    }
    internal sealed class HandlerRegistry
    {
        private readonly ImmutableHashSet<HandlerKey> _handlers;
        private readonly ImmutableHashSet<HandlerKey> _terminalizers;
        private static readonly char[] HandlerSeparator = { ';' };
        public HandlerRegistry(Compilation compilation, AnalyzerConfigOptions options, ResultFlowEngineOptions engineOptions)
        {
            _handlers = ParseConfig(compilation, options, engineOptions.HandlerConfigKey);
            _terminalizers = ParseConfig(compilation, options, engineOptions.TerminalizerConfigKey);
        }
        public bool IsHandler(IMethodSymbol method) => IsInSet(method, _handlers);
        public bool IsTerminalizer(IMethodSymbol method) => IsInSet(method, _terminalizers);
        private static ImmutableHashSet<HandlerKey> ParseConfig(
        Compilation compilation,
        AnalyzerConfigOptions options,
        string key)
        {
            if (!options.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                return ImmutableHashSet<HandlerKey>.Empty;
            var builder = ImmutableHashSet.CreateBuilder<HandlerKey>();
            foreach (var token in value.Split(HandlerSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = token.Trim();
                if (trimmed.Length == 0)
                    continue;
                var lastDot = trimmed.LastIndexOf('.');
                if (lastDot <= 0 || lastDot == trimmed.Length - 1)
                    continue;
                var containingTypeName = trimmed.Substring(0, lastDot);
                var methodName = trimmed.Substring(lastDot + 1);
                builder.Add(new HandlerKey(containingTypeName, methodName));
            }
            return builder.ToImmutable();
        }
        private static bool IsInSet(IMethodSymbol method, ImmutableHashSet<HandlerKey> set)
        {
            if (set.IsEmpty)
                return false;
            var containingTypeName = method.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", string.Empty);
            var key = new HandlerKey(containingTypeName, method.Name);
            return set.Contains(key);
        }
        private readonly struct HandlerKey : IEquatable<HandlerKey>
        {
            public HandlerKey(string containingType, string methodName)
            {
                ContainingType = containingType;
                MethodName = methodName;
            }
            public string ContainingType { get; }
            public string MethodName { get; }
            public bool Equals(HandlerKey other)
            => string.Equals(ContainingType, other.ContainingType, StringComparison.Ordinal) &&
            string.Equals(MethodName, other.MethodName, StringComparison.Ordinal);
            public override bool Equals(object? obj) => obj is HandlerKey other && Equals(other);
            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 17;
                    hash = (hash * 31) + StringComparer.Ordinal.GetHashCode(ContainingType);
                    hash = (hash * 31) + StringComparer.Ordinal.GetHashCode(MethodName);
                    return hash;
                }
            }
        }
    }
}
