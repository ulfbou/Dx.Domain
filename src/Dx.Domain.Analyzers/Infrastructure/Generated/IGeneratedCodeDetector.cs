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
    public interface IGeneratedCodeDetector
    {
        bool IsGenerated(ISymbol symbol);
    }
    public sealed class GeneratedCodeDetector : IGeneratedCodeDetector
    {
        private static readonly char[] NamespaceSeparators = { ';' };
        private readonly HashSet<string> _namespaceMarkers;

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
