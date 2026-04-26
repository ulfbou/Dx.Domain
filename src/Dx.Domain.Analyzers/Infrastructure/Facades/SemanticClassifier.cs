// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="SemanticClassifier.cs" company="Dx.Domain Team">
// Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
// This software is licensed under the MIT License.
// See the project's root <c>LICENSE</c> file for details.
// Contributions are welcome, subject to the terms of the project's license.
// See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace Dx.Domain.Analyzers.Infrastructure.Facades
{
    public sealed class SemanticClassifier : ISemanticClassifier
    {
        private readonly ImmutableHashSet<INamedTypeSymbol> _resultTypes;
        private readonly INamedTypeSymbol? _domainError;
        private readonly INamedTypeSymbol? _invariantViolation;
        private static readonly string[] DomainMarkerAttributes =
        {
            "AggregateRootAttribute",
            "ValueObjectAttribute"
        };

        public SemanticClassifier(Compilation compilation)
        {
            _resultTypes = LoadResultTypes(compilation);
            _domainError = compilation.GetTypeByMetadataName("Dx.Domain.DomainError");
            _invariantViolation =
                compilation.GetTypeByMetadataName("Dx.Domain.InvariantViolationException");
        }

        public bool IsKernelResultType(ITypeSymbol type) =>
            type is INamedTypeSymbol nt &&
            _resultTypes.Any(r =>
                SymbolEqualityComparer.Default.Equals(nt.ConstructedFrom, r));

        public bool IsDomainErrorType(ITypeSymbol type) =>
            _domainError != null &&
            SymbolEqualityComparer.Default.Equals(type, _domainError);

        public bool IsInvariantException(ITypeSymbol type)
        {
            if (_invariantViolation == null)
                return false;

            var current = type;
            while (current != null)
            {
                if (SymbolEqualityComparer.Default.Equals(current, _invariantViolation))
                    return true;
                current = current.BaseType;
            }

            return false;
        }

        public bool IsDomainType(ITypeSymbol type)
        {
            if (type is not INamedTypeSymbol named)
                return false;

            // Exclude kernel Result types
            if (IsKernelResultType(type))
                return false;

            // Exclude framework types (annotations, attributes, etc.)
            if (IsFrameworkType(named))
                return false;

            if (HasDomainMarker(named))
                return true;

            if (ImplementsDomainInterface(named))
                return true;

            var ns = type.ContainingNamespace?.ToDisplayString();
            if (ns == null)
                return false;

            return ns.StartsWith("Dx.Domain", System.StringComparison.Ordinal) ||
                   ns.Contains(".Domain.") ||
                   ns.EndsWith(".Domain", System.StringComparison.Ordinal);
        }

        private static bool HasDomainMarker(INamedTypeSymbol type)
        {
            foreach (var attribute in type.GetAttributes())
            {
                var name = attribute.AttributeClass?.Name;
                if (name == null)
                    continue;

                if (DomainMarkerAttributes.Contains(name, System.StringComparer.Ordinal))
                    return true;
            }

            return false;
        }

        private static bool ImplementsDomainInterface(INamedTypeSymbol type)
        {
            foreach (var iface in type.AllInterfaces)
            {
                if (iface.Name == "IAggregateRoot" || iface.Name == "IValueObject")
                    return true;
            }

            return false;
        }

        private static bool IsFrameworkType(INamedTypeSymbol type)
        {
            var ns = type.ContainingNamespace?.ToDisplayString();
            if (ns == null)
                return false;

            // Exclude Dx framework types
            if (ns == "Dx.Domain.Annotations" ||
                ns == "Dx.Domain.Attributes" ||
                ns.StartsWith("Dx.Domain.Annotations.", System.StringComparison.Ordinal) ||
                ns.StartsWith("Dx.Domain.Attributes.", System.StringComparison.Ordinal))
                return true;

            // Exclude types that inherit from System.Attribute
            var current = type.BaseType;
            while (current != null)
            {
                if (current.ToDisplayString() == "System.Attribute")
                    return true;
                current = current.BaseType;
            }

            return false;
        }

        private static ImmutableHashSet<INamedTypeSymbol> LoadResultTypes(Compilation c)
        {
            var builder = ImmutableHashSet.CreateBuilder<INamedTypeSymbol>(
                SymbolEqualityComparer.Default);

            void TryAdd(string name)
            {
                var t = c.GetTypeByMetadataName(name);
                if (t != null)
                    builder.Add(t);
            }

            TryAdd("Dx.Domain.Result");
            TryAdd("Dx.Domain.Result`1");
            TryAdd("Dx.Domain.Result`2");
            TryAdd("Dx.Domain.Unit"); // AC5

            return builder.ToImmutable();
        }
    }
}
