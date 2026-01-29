// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="PrimitiveCatalog.cs" company="Dx.Domain Team">
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
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Infrastructure.Primitives
{
    internal sealed class PrimitiveCatalog
    {
        private readonly ImmutableArray<PrimitiveMapping> _mappings;

        private PrimitiveCatalog(ImmutableArray<PrimitiveMapping> mappings)
        {
            _mappings = mappings;
        }

        public static PrimitiveCatalog Create(Compilation compilation)
        {
            var mappings = ImmutableArray.CreateBuilder<PrimitiveMapping>();

            var guidType = compilation.GetTypeByMetadataName("System.Guid");
            var stringType = compilation.GetSpecialType(SpecialType.System_String);

            void TryAdd(string marker, ITypeSymbol? primitive, params string[] domainTypeNames)
            {
                if (primitive == null)
                    return;

                foreach (var name in domainTypeNames)
                {
                    var domainType = compilation.GetTypeByMetadataName(name);
                    if (domainType != null)
                    {
                        mappings.Add(new PrimitiveMapping(marker, primitive, domainType));
                        return;
                    }
                }
            }

            TryAdd("ActorId", guidType, "Dx.Domain.Primitives.ActorId", "Dx.Domain.ActorId");
            TryAdd("CorrelationId", guidType, "Dx.Domain.Primitives.CorrelationId", "Dx.Domain.CorrelationId");
            TryAdd("FactId", guidType, "Dx.Domain.Primitives.FactId", "Dx.Domain.FactId");
            TryAdd("TraceId", stringType, "Dx.Domain.Primitives.TraceId", "Dx.Domain.TraceId");
            TryAdd("SpanId", stringType, "Dx.Domain.Primitives.SpanId", "Dx.Domain.SpanId");

            return new PrimitiveCatalog(mappings.ToImmutable());
        }

        public bool TryGetReplacement(ITypeSymbol type, string symbolName, out INamedTypeSymbol replacement)
        {
            replacement = null!;

            var normalizedType = UnwrapNullable(type);

            foreach (var mapping in _mappings)
            {
                if (!mapping.Matches(symbolName, normalizedType))
                    continue;

                replacement = mapping.DomainType;
                return true;
            }

            return false;
        }

        private static ITypeSymbol UnwrapNullable(ITypeSymbol type)
        {
            if (type is INamedTypeSymbol named && named.IsGenericType && named.MetadataName == "Nullable`1")
                return named.TypeArguments[0];

            return type;
        }

        private sealed record PrimitiveMapping(string Marker, ITypeSymbol PrimitiveType, INamedTypeSymbol DomainType)
        {
            public bool Matches(string symbolName, ITypeSymbol type)
            {
                if (!SymbolEqualityComparer.Default.Equals(type, PrimitiveType))
                    return false;

                return symbolName.EndsWith(Marker, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
