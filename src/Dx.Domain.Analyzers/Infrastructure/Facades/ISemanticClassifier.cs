// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ISemanticClassifier.cs" company="Dx.Domain Team">
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

using System;
using System.Collections.Immutable;
using System.Linq;


namespace Dx.Domain.Analyzers.Infrastructure.Facades
{
    /// <summary>
    /// Classifies types according to Dx semantic categories.
    /// </summary>
    /// <remarks>
    /// Distinguishes Kernel Result types, DomainError, invariant exceptions, and domain types marked by attributes.
    /// </remarks>
    public interface ISemanticClassifier
    {
        /// <summary>Determines whether the specified type is a Kernel Result type.</summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>True if the type is Result or Result{T}; otherwise, false.</returns>
        bool IsKernelResultType(ITypeSymbol type);

        /// <summary>Determines whether the specified type is DomainError.</summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>True if the type is DomainError; otherwise, false.</returns>
        bool IsDomainErrorType(ITypeSymbol type);

        /// <summary>Determines whether the specified type is an invariant violation exception.</summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>True if the type represents an invariant violation; otherwise, false.</returns>
        bool IsInvariantException(ITypeSymbol type);

        /// <summary>Determines whether the specified type is a domain type.</summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>True if the type is classified as a domain type; otherwise, false.</returns>
        bool IsDomainType(ITypeSymbol type);
    }

    /// <summary>
    /// Default implementation of <see cref="ISemanticClassifier"/> based on compilation symbols.
    /// </summary>
    /// <remarks>
    /// Caches well-known Kernel types for performance. Classification is conservative.
    /// </remarks>
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

        /// <summary>Initializes a new instance of the <see cref="SemanticClassifier"/> class.</summary>
        /// <param name="compilation">The compilation to analyze.</param>
        public SemanticClassifier(Compilation compilation)
        {
            _resultTypes = LoadResultTypes(compilation);
            _domainError = compilation.GetTypeByMetadataName("Dx.Domain.DomainError");
            _invariantViolation =
                compilation.GetTypeByMetadataName("Dx.Domain.InvariantViolationException");
        }

        /// <inheritdoc/>
        public bool IsKernelResultType(ITypeSymbol type) =>
            type is INamedTypeSymbol nt &&
            _resultTypes.Any(r =>
                SymbolEqualityComparer.Default.Equals(nt.ConstructedFrom, r));

        /// <inheritdoc/>
        public bool IsDomainErrorType(ITypeSymbol type) =>
            _domainError != null &&
            SymbolEqualityComparer.Default.Equals(type, _domainError);

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool IsDomainType(ITypeSymbol type)
        {
            if (type is not INamedTypeSymbol named)
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

            return builder.ToImmutable();
        }
    }
}
