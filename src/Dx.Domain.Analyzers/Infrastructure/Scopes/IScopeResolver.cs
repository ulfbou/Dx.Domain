// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IScopeResolver.cs" company="Dx.Domain Team">
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

    /// <summary>
    /// Resolves the scope for assemblies and symbols based on analyzer configuration options.
    /// </summary>
    /// <remarks>ScopeResolver provides mapping between assemblies or symbols and their associated scopes, as
    /// defined by configuration options. This is typically used in code analysis scenarios to determine how different
    /// assemblies or symbols should be treated according to their configured scope.</remarks>
    internal sealed class ScopeResolver : IScopeResolver
    {
        private static readonly char[] ScopeSeparator = { ';' };

        private readonly ImmutableDictionary<string, Scope> _assemblyMap;
        private readonly ImmutableArray<string> _rootNamespaces;

        /// <summary>
        /// Initializes a new instance of the ScopeResolver class using the specified analyzer configuration options.
        /// </summary>
        /// <param name="config">An AnalyzerConfigOptionsProvider that supplies configuration options for resolving assemblies and root
        /// namespaces. Cannot be null.</param>
        public ScopeResolver(AnalyzerConfigOptionsProvider config)
        {
            _assemblyMap = ParseAssemblyMap(config);
            _rootNamespaces = ParseRootNamespaces(config);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
