// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="HandlerRegistry.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Analyzers.ResultFlow
{
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
