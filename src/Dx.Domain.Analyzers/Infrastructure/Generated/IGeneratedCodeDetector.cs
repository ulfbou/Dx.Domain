// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IGeneratedCodeDetector.cs" company="Dx.Domain Team">
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
    /// Detects symbols originating from generated code.
    /// </summary>
    /// <remarks>
    /// Supplements Roslyn's built-in detection with Dx-specific markers.
    /// </remarks>
    public interface IGeneratedCodeDetector
    {
        /// <summary>Determines whether the specified symbol is generated.</summary>
        /// <param name="symbol">The symbol to evaluate.</param>
        /// <returns>True if the symbol is generated; otherwise, false.</returns>
        bool IsGenerated(ISymbol symbol);
    }

    /// <summary>
    /// Default implementation of <see cref="IGeneratedCodeDetector"/> using configuration and attributes.
    /// </summary>
    /// <remarks>
    /// Evaluates GeneratedCodeAttribute and configured namespace markers. Implementation is fail-open.
    /// </remarks>
    public sealed class GeneratedCodeDetector : IGeneratedCodeDetector
    {
        private static readonly char[] NamespaceSeparators = { ';' };
        private readonly HashSet<string> _namespaceMarkers;

        /// <summary>Initializes a new instance of the <see cref="GeneratedCodeDetector"/> class.</summary>
        /// <param name="config">The analyzer configuration provider.</param>
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
