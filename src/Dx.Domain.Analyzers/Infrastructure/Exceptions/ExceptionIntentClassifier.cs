// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ExceptionIntentClassifier.cs" company="Dx.Domain Team">
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
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers.Infrastructure.Exceptions
{
    /// <summary>
    /// Classifies <see cref="IThrowOperation"/> instances into high level intent buckets
    /// that other analyzers can use when reasoning about exception usage.
    /// </summary>
    /// <remarks>
    /// The implementation is deliberately conservative and fail-open: when it cannot
    /// confidently determine an intent, it returns <see cref="ExceptionIntent.Unknown"/>.
    /// </remarks>
    public sealed class ExceptionIntentClassifier : IExceptionIntentClassifier
    {
        private static readonly char[] SplitSemicolon = { ';' };

        private readonly INamedTypeSymbol? _argumentException;
        private readonly INamedTypeSymbol? _argumentNullException;
        private readonly INamedTypeSymbol? _argumentOutOfRangeException;
        private readonly INamedTypeSymbol? _invalidOperationException;
        private readonly INamedTypeSymbol? _operationCanceledException;
        private readonly INamedTypeSymbol? _taskCanceledException;
        private readonly INamedTypeSymbol? _invariantViolationException;

        private readonly ISet<INamedTypeSymbol> _validationExceptions;
        private readonly ISet<INamedTypeSymbol> _controlFlowExceptions;
        private readonly ISet<INamedTypeSymbol> _domainControlExceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionIntentClassifier"/> class.
        /// </summary>
        /// <param name="compilation">The compilation used for symbol resolution.</param>
        /// <param name="config">Analyzer configuration options for optional overrides.</param>
        public ExceptionIntentClassifier(Compilation compilation, AnalyzerConfigOptionsProvider config)
        {
            _argumentException = compilation.GetTypeByMetadataName("System.ArgumentException");
            _argumentNullException = compilation.GetTypeByMetadataName("System.ArgumentNullException");
            _argumentOutOfRangeException = compilation.GetTypeByMetadataName("System.ArgumentOutOfRangeException");
            _invalidOperationException = compilation.GetTypeByMetadataName("System.InvalidOperationException");
            _operationCanceledException = compilation.GetTypeByMetadataName("System.OperationCanceledException");
            _taskCanceledException = compilation.GetTypeByMetadataName("System.Threading.Tasks.TaskCanceledException");

            _invariantViolationException = compilation.GetTypeByMetadataName("Dx.Domain.InvariantViolationException");

            _validationExceptions = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            _controlFlowExceptions = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            _domainControlExceptions = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

            AddIfNotNull(_validationExceptions, _argumentException);
            AddIfNotNull(_validationExceptions, _argumentNullException);
            AddIfNotNull(_validationExceptions, _argumentOutOfRangeException);
            AddIfNotNull(_validationExceptions, _invalidOperationException);

            AddIfNotNull(_controlFlowExceptions, _operationCanceledException);
            AddIfNotNull(_controlFlowExceptions, _taskCanceledException);

            var global = config.GlobalOptions;

            LoadOverrides(global, "dx_exception_intent.validation", _validationExceptions, compilation);
            LoadOverrides(global, "dx_exception_intent.controlFlow", _controlFlowExceptions, compilation);
            LoadOverrides(global, "dx_exception_domain_types", _domainControlExceptions, compilation);
        }

        /// <inheritdoc />
        public ExceptionIntent Classify(IThrowOperation throwOperation)
        {
            if (throwOperation == null)
                throw new ArgumentNullException(nameof(throwOperation));

            var exception = throwOperation.Exception;
            var type = exception?.Type as INamedTypeSymbol;
            if (type == null)
                return ExceptionIntent.Unknown;

            if (_invariantViolationException != null &&
                SymbolEqualityComparer.Default.Equals(type, _invariantViolationException))
            {
                return ExceptionIntent.InvariantViolation;
            }

            if (IsOrInheritsFrom(type, _validationExceptions))
            {
                return ExceptionIntent.ArgumentValidation;
            }

            if (IsOrInheritsFrom(type, _controlFlowExceptions))
            {
                return ExceptionIntent.ControlFlow;
            }

            if (IsOrInheritsFrom(type, _domainControlExceptions))
            {
                return ExceptionIntent.DomainControl;
            }

            return ExceptionIntent.Unknown;
        }

        private static void AddIfNotNull(ISet<INamedTypeSymbol> set, INamedTypeSymbol? symbol)
        {
            if (symbol != null)
            {
                set.Add(symbol);
            }
        }

        private static void LoadOverrides(
            AnalyzerConfigOptions options,
            string key,
            ISet<INamedTypeSymbol> destination,
            Compilation compilation)
        {
            if (!options.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
                return;

            var parts = raw.Split(SplitSemicolon, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var name = part.Trim();
                if (name.Length == 0)
                    continue;

                var symbol = compilation.GetTypeByMetadataName(name);
                if (symbol != null)
                {
                    destination.Add(symbol);
                }
            }
        }

        private static bool IsOrInheritsFrom(INamedTypeSymbol type, ISet<INamedTypeSymbol> candidates)
        {
            for (var current = type; current != null; current = current.BaseType)
            {
                foreach (var candidate in candidates)
                {
                    if (SymbolEqualityComparer.Default.Equals(current, candidate))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
