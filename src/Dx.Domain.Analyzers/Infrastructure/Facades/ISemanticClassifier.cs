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

using System.Collections.Immutable;
using System.Linq;

namespace Dx.Domain.Analyzers.Infrastructure.Semantics
{
    public interface ISemanticClassifier
    {
        bool IsKernelResultType(ITypeSymbol type);
        bool IsDomainErrorType(ITypeSymbol type);
        bool IsInvariantException(ITypeSymbol type);
    }
    public sealed class SemanticClassifier : ISemanticClassifier
    {
        private readonly ImmutableHashSet<INamedTypeSymbol> _resultTypes;
        private readonly INamedTypeSymbol? _domainError;
        private readonly INamedTypeSymbol? _invariantViolation;

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
