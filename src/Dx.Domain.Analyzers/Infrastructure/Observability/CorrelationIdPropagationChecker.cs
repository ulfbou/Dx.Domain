// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="CorrelationIdPropagationChecker.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers.Infrastructure.Observability
{
    internal sealed class CorrelationIdPropagationChecker
    {
        private readonly INamedTypeSymbol _correlationIdType;

        public CorrelationIdPropagationChecker(INamedTypeSymbol correlationIdType)
        {
            _correlationIdType = correlationIdType;
        }

        public bool ShouldReport(IInvocationOperation operation, IMethodSymbol? containingMethod)
        {
            if (containingMethod == null)
                return false;

            var targetParameters = operation.TargetMethod.Parameters
                .Where(p => IsCorrelationId(p.Type))
                .ToArray();

            if (targetParameters.Length == 0)
                return false;

            var available = containingMethod.Parameters
                .Where(p => IsCorrelationId(p.Type))
                .ToArray();

            if (available.Length == 0)
                return true;

            foreach (var parameter in targetParameters)
            {
                var argument = operation.Arguments.FirstOrDefault(a =>
                    SymbolEqualityComparer.Default.Equals(a.Parameter, parameter));

                if (argument == null)
                    return true;

                if (argument.Value is IParameterReferenceOperation parameterReference &&
                    available.Any(p => SymbolEqualityComparer.Default.Equals(p, parameterReference.Parameter)))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private bool IsCorrelationId(ITypeSymbol type)
        {
            if (SymbolEqualityComparer.Default.Equals(type, _correlationIdType))
                return true;

            return type is INamedTypeSymbol named &&
                   named.IsGenericType &&
                   named.TypeArguments.Any(t => IsCorrelationId(t));
        }
    }
}
