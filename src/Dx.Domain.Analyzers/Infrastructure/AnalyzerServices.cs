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

using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    // Support for C# 9 record types when targeting netstandard2.0
    internal static class IsExternalInit { }
}

namespace Dx.Domain.Analyzers.Infrastructure
{
    /// <summary>
    /// Defines the classified intent of a throw operation for use by analyzers.
    /// </summary>
    /// <remarks>
    /// Used to distinguish argument validation, invariant violations, and domain control flow from infrastructure exceptions. Classification is conservative and fail-open.
    /// </remarks>
    public enum ExceptionIntent
    {
        /// <summary>Intent cannot be determined or is ambiguous.</summary>
        Unknown,

        /// <summary>Exception is thrown to validate arguments.</summary>
        ArgumentValidation,

        /// <summary>Exception is thrown to signal a Kernel invariant violation.</summary>
        InvariantViolation,

        /// <summary>Exception is thrown to alter control flow, such as cancellation.</summary>
        ControlFlow,

        /// <summary>Exception is thrown to signal domain-level control flow that should use Result.</summary>
        DomainControl,

        /// <summary>Exception is thrown for infrastructure or I/O concerns.</summary>
        Infrastructure
    }

    /// <summary>
    /// Classifies throw operations by intent for analyzer consumption.
    /// </summary>
    /// <remarks>
    /// Provides the single source of truth for exception intent. Implementations must be conservative and return <see cref="ExceptionIntent.Unknown"/> when classification is ambiguous.
    /// </remarks>
    public interface IExceptionIntentClassifier
    {
        /// <summary>Classifies the intent of a throw operation.</summary>
        /// <param name="throwOperation">The throw operation to classify.</param>
        /// <returns>The classified intent, or <see cref="ExceptionIntent.Unknown"/> if classification fails.</returns>
        ExceptionIntent Classify(Microsoft.CodeAnalysis.Operations.IThrowOperation throwOperation);
    }

    /// <summary>
    /// Aggregates the infrastructure services required by analyzers.
    /// </summary>
    /// <remarks>
    /// Serves as the composition root for scope resolution, Facade discovery, semantic classification, exception intent classification, Result flow analysis, and generated code detection. Instances are created per compilation start and are immutable.
    /// </remarks>
    public sealed record AnalyzerServices(
        IScopeResolver Scope,
        IDxFacadeResolver Dx,
        ISemanticClassifier Semantic,
        IExceptionIntentClassifier Exceptions,
        ResultFlowEngineWrapper Flow,
        IGeneratedCodeDetector Generated);
}
