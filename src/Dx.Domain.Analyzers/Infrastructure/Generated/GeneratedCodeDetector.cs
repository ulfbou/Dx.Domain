// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="GeneratedCodeDetector.cs" company="Dx.Domain Team">
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

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace Dx.Domain.Analyzers.Infrastructure.Generated
{
    /// <summary>
    /// Detects whether a symbol originates from generated code.
    /// </summary>
    /// <remarks>
    /// This type is used exclusively by analyzers to suppress diagnostics on generated sources.
    /// Detection is based on <see cref="GeneratedCodeAttribute"/>, <c>CompilerGeneratedAttribute</c>, and namespace markers configured via the <c>dx_generated_markers</c> analyzer option.
    /// It carries analysis configuration only and imposes no runtime semantics outside compilation analysis.
    /// </remarks>
    public sealed class GeneratedCodeDetector : IGeneratedCodeDetector
    {
        private static readonly char[] NamespaceSeparators = { ';' };
        private readonly HashSet<string> _namespaceMarkers;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedCodeDetector"/> class with the specified configuration.
        /// </summary>
        /// <param name="config">The analyzer configuration provider used to read global options.</param>
        public GeneratedCodeDetector(AnalyzerConfigOptionsProvider config)
        {
            if (!config.GlobalOptions.TryGetValue("dx_generated_markers", out var raw))
            {
                _namespaceMarkers = new HashSet<string>();
                return;
            }

            _namespaceMarkers = new HashSet<string>(
                raw.Split(NamespaceSeparators, StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => s.Trim()));

        }

        /// <inheritdoc/>
        public bool IsGenerated(ISymbol symbol)
        {
            if (symbol.GetAttributes().Any(a =>
                a.AttributeClass?.ToDisplayString() ==
                typeof(GeneratedCodeAttribute).FullName))
                return true;

            if (symbol.GetAttributes().Any(a =>
                a.AttributeClass?.Name == "CompilerGeneratedAttribute"))
                return true;

            var ns = symbol.ContainingNamespace?.ToDisplayString();
            if (ns != null && _namespaceMarkers.Any(ns.StartsWith))
                return true;

            return false;
        }
    }
}
