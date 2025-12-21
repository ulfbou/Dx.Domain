// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AnalyzerServices.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Flow;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;
using Dx.Domain.Analyzers.Infrastructure.Semantics;

using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    // Support for C# 9 record types when targeting netstandard2.0
    internal static class IsExternalInit { }
}

namespace Dx.Domain.Analyzers.Infrastructure
{
    /// <summary>
    /// Represents the intent behind an exception being thrown.
    /// </summary>
    public enum ExceptionIntent
    {
        /// <summary>
        /// Intent cannot be determined or is ambiguous.
        /// </summary>
        Unknown,

        /// <summary>
        /// Exception is thrown for argument validation (e.g., ArgumentNullException, ArgumentException).
        /// </summary>
        ArgumentValidation,

        /// <summary>
        /// Exception is thrown for invariant violation (e.g., InvariantViolationException).
        /// </summary>
        InvariantViolation,

        /// <summary>
        /// Exception is thrown to signal a control flow decision (e.g., OperationCanceledException).
        /// </summary>
        ControlFlow,

        /// <summary>
        /// Exception is thrown for domain control flow.
        /// </summary>
        DomainControl,

        /// <summary>
        /// Exception is thrown for infrastructure concerns.
        /// </summary>
        Infrastructure
    }

    /// <summary>
    /// Classifies exception throw operations by their intent.
    /// </summary>
    public interface IExceptionIntentClassifier
    {
        /// <summary>
        /// Classifies the intent of a throw operation.
        /// </summary>
        /// <param name="throwOperation">The throw operation to classify.</param>
        /// <returns>The classified intent, or <see cref="ExceptionIntent.Unknown"/> if classification fails.</returns>
        ExceptionIntent Classify(Microsoft.CodeAnalysis.Operations.IThrowOperation throwOperation);
    }

    public sealed record AnalyzerServices(
        IScopeResolver Scope,
        IDxFacadeResolver Dx,
        ISemanticClassifier Semantic,
        IExceptionIntentClassifier Exceptions,
        ResultFlowEngineWrapper Flow,
        IGeneratedCodeDetector Generated);
}
