// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IDomainFactResolver.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Linq;

using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Infrastructure.Facades
{
    internal interface IDomainFactResolver
    {
        INamedTypeSymbol? DomainFactType { get; }

        bool IsDomainFact(ITypeSymbol type);
    }

    internal sealed class DomainFactResolver : IDomainFactResolver
    {
        public DomainFactResolver(Compilation compilation)
        {
            DomainFactType = compilation.GetTypeByMetadataName("Dx.Domain.Factors.IDomainFact")
                             ?? compilation.GetTypeByMetadataName("Dx.Domain.IDomainFact");
        }

        public INamedTypeSymbol? DomainFactType { get; }

        public bool IsDomainFact(ITypeSymbol type)
        {
            if (DomainFactType == null)
                return false;

            if (SymbolEqualityComparer.Default.Equals(type, DomainFactType))
                return true;

            if (type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, DomainFactType)))
                return true;

            return false;
        }
    }
}
