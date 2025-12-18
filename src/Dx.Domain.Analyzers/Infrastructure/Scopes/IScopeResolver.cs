using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using System.Collections.Immutable;
using System;
using System.Linq;

namespace Dx.Domain.Analyzers.Infrastructure.Scopes
{
    public interface IScopeResolver
    {
        Scope ResolveAssembly(IAssemblySymbol assembly);
        Scope ResolveSymbol(ISymbol symbol);
    }

    public sealed class ScopeResolver : IScopeResolver
    {
        private static readonly char[] ScopeSeparator = { ';' };

        private readonly ImmutableDictionary<string, Scope> _assemblyMap;
        private readonly ImmutableArray<string> _rootNamespaces;

        public ScopeResolver(AnalyzerConfigOptionsProvider config)
        {
            _assemblyMap = ParseAssemblyMap(config);
            _rootNamespaces = ParseRootNamespaces(config);
        }

        public Scope ResolveAssembly(IAssemblySymbol assembly)
        {
            if (_assemblyMap.TryGetValue(assembly.Name, out var scope))
                return scope;

            foreach (var ns in _rootNamespaces)
            {
                if (assembly.GlobalNamespace.ToDisplayString().StartsWith(ns, StringComparison.Ordinal))
                    return Scope.S3;
            }

            return Scope.S3;
        }

        public Scope ResolveSymbol(ISymbol symbol) =>
            ResolveAssembly(symbol.ContainingAssembly);

        private static ImmutableDictionary<string, Scope> ParseAssemblyMap(AnalyzerConfigOptionsProvider config)
        {
            var options = config.GlobalOptions;
            if (!options.TryGetValue("dx.scope.map", out var raw))
                return ImmutableDictionary<string, Scope>.Empty;

            var builder = ImmutableDictionary.CreateBuilder<string, Scope>(StringComparer.Ordinal);

            foreach (var entry in raw.Split(ScopeSeparator))
            {
                if (string.IsNullOrWhiteSpace(entry))
                    continue;

                var parts = entry.Split('=');
                if (parts.Length == 2 && Enum.TryParse(parts[1].Trim(), out Scope scope))
                    builder[parts[0].Trim()] = scope;
            }

            return builder.ToImmutable();
        }

        private static ImmutableArray<string> ParseRootNamespaces(AnalyzerConfigOptionsProvider config)
        {
            var options = config.GlobalOptions;
            if (!options.TryGetValue("dx.scope.rootNamespaces", out var raw))
                return ImmutableArray<string>.Empty;

            return raw.Split(ScopeSeparator)
                      .Select(s => s.Trim())
                      .Where(s => s.Length != 0)
                      .ToImmutableArray();
        }
    }
}
