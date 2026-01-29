// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="OutboxBoundaryDetector.cs" company="Dx.Domain Team">
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
using System.Linq;

using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Infrastructure.Boundaries
{
    internal sealed class OutboxBoundaryDetector
    {
        private static readonly string[] NameMarkers =
        {
            "Outbox",
            "Messaging",
            "MessageBus"
        };

        public bool IsBoundary(IMethodSymbol method) =>
            method.ContainingType != null && IsBoundary(method.ContainingType);

        private static bool IsBoundary(INamedTypeSymbol type)
        {
            var typeName = type.Name;
            var ns = type.ContainingNamespace?.ToDisplayString() ?? string.Empty;

            return NameMarkers.Any(marker =>
                typeName.IndexOf(marker, StringComparison.OrdinalIgnoreCase) >= 0 ||
                ns.IndexOf(marker, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
