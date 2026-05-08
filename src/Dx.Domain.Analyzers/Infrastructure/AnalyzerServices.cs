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
    /// Aggregates shared analyzer services used during compilation analysis.
    /// </summary>
    /// <param name="Scope">The scope resolver service.</param>
    /// <param name="Dx">The Dx facade resolver service.</param>
    /// <param name="Semantic">The semantic classifier service.</param>
    /// <param name="Exceptions">The exception intent classifier service.</param>
    /// <param name="Flow">The result flow engine wrapper service.</param>
    /// <param name="Generated">The generated code detector service.</param>
    /// <remarks>
    /// This record is pure analyzer metadata.
    /// It is immutable, contains no behavior, and imposes no runtime semantics.
    /// </remarks>
    public sealed record AnalyzerServices(
        IScopeResolver Scope,
        IDxFacadeResolver Dx,
        ISemanticClassifier Semantic,
        IExceptionIntentClassifier Exceptions,
        ResultFlowEngineWrapper Flow,
        IGeneratedCodeDetector Generated);
}
