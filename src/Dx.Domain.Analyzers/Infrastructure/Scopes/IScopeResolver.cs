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
        private readonly string? _dxLayer;
        private readonly string? _dxResolvedRole;
        private readonly bool _isTestProject;

        /// <summary>
        /// Initializes a new instance of the ScopeResolver class using the specified analyzer configuration options.
        /// </summary>
        /// <param name="config">An AnalyzerConfigOptionsProvider that supplies configuration options for resolving assemblies and root
        /// namespaces. Cannot be null.</param>
        public ScopeResolver(AnalyzerConfigOptionsProvider config)
        {
            var options = config.GlobalOptions;
            options.TryGetValue("build_property.DxLayer", out _dxLayer);
            if (string.IsNullOrWhiteSpace(_dxLayer))
            {
                options.TryGetValue("dx.layer", out _dxLayer);
            }

            options.TryGetValue("build_property.DxResolvedRole", out _dxResolvedRole);

            if (options.TryGetValue("build_property.IsTestProject", out var isTest))
            {
                _isTestProject = string.Equals(isTest, "true", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <inheritdoc/>
        public Scope ResolveAssembly(IAssemblySymbol assembly)
        {
            if (IsKernelLikeAssembly(assembly.Name))
                return Scope.S0;

            if (_isTestProject)
                return Scope.S0;

            if (TryResolveScopeFromLayer(_dxLayer, out var layerScope))
                return layerScope;

            if (TryResolveScopeFromLayer(_dxResolvedRole, out var roleScope))
                return roleScope;

            if (TryResolveScopeFromAttribute(assembly, out var attributeScope))
                return attributeScope;

            return Scope.S3;
        }

        /// <inheritdoc/>
        public Scope ResolveSymbol(ISymbol symbol) =>
            symbol is IAssemblySymbol assembly
                ? ResolveAssembly(assembly)
                : ResolveAssembly(symbol.ContainingAssembly);

        private static bool TryResolveScopeFromAttribute(IAssemblySymbol assembly, out Scope scope)
        {
            foreach (var attribute in assembly.GetAttributes())
            {
                if (!string.Equals(attribute.AttributeClass?.Name, "DxLayerAttribute", StringComparison.Ordinal))
                    continue;

                if (attribute.ConstructorArguments.Length == 1 && attribute.ConstructorArguments[0].Value is string raw)
                {
                    if (TryResolveScopeFromLayer(raw, out var attributeScope))
                    {
                        scope = attributeScope;
                        return true;
                    }
                }
            }

            scope = Scope.S3;
            return false;
        }

        private static bool TryResolveScopeFromLayer(string? raw, out Scope scope)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                scope = Scope.S3;
                return false;
            }

            if (string.Equals(raw, "Kernel", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(raw, "Primitives", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(raw, "Annotations", StringComparison.OrdinalIgnoreCase))
            {
                scope = Scope.S0;
                return true;
            }

            if (string.Equals(raw, "Consumer", StringComparison.OrdinalIgnoreCase))
            {
                scope = Scope.S3;
                return true;
            }

            if (string.Equals(raw, "Test", StringComparison.OrdinalIgnoreCase))
            {
                scope = Scope.S0;
                return true;
            }

            scope = Scope.S3;
            return false;
        }

        private static bool IsKernelLikeAssembly(string? assemblyName)
        {
            return string.Equals(assemblyName, "Dx.Domain.Kernel", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(assemblyName, "Dx.Domain.Primitives", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(assemblyName, "Dx.Domain.Annotations", StringComparison.OrdinalIgnoreCase);
        }
    }
}
