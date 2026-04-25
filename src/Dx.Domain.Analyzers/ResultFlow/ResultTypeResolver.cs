// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ResultTypeResolver.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers.ResultFlow
{
    internal sealed class ResultTypeResolver
    {
        private readonly Compilation _compilation;
        private readonly AnalyzerConfigOptions _options;
        private readonly ResultFlowEngineOptions _engineOptions;
        private ImmutableHashSet<INamedTypeSymbol>? _resultTypesCache;
        public ResultTypeResolver(Compilation compilation, AnalyzerConfigOptions options, ResultFlowEngineOptions engineOptions)
        {
            _compilation = compilation;
            _options = options;
            _engineOptions = engineOptions;
        }
        private ImmutableHashSet<INamedTypeSymbol> GetResultTypes()
        {
            if (_resultTypesCache is not null)
                return _resultTypesCache;
            var builder = ImmutableHashSet.CreateBuilder<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            foreach (var metadataName in _engineOptions.ResultTypeMetadataNames)
            {
                if (_compilation.GetTypeByMetadataName(metadataName) is { } symbol)
                {
                    builder.Add(symbol);
                }
            }
            _resultTypesCache = builder.ToImmutable();
            return _resultTypesCache;
        }
        public bool IsResultType(ITypeSymbol? type)
        {
            if (type is null)
                return false;
            if (type is IErrorTypeSymbol)
                return false;
            if (type is not INamedTypeSymbol named)
                return false;
            var resultTypes = GetResultTypes();
            foreach (var result in resultTypes)
            {
                if (SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, result))
                    return true;
            }
            return false;
        }
        public bool IsResultLikeInstance(IOperation instance)
        {
            return IsResultType(instance.Type);
        }
    }
}
