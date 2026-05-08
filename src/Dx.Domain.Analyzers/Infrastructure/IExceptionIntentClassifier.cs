// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IExceptionIntentClassifier.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers.Infrastructure
{
    /// <summary>
    /// Defines a contract for classifying exception throw operations by their intent.
    /// </summary>
    /// <remarks>
    /// This interface is used exclusively by analyzers to distinguish guard failures from domain faults. It carries analysis data only and imposes no runtime semantics outside compilation analysis.
    /// </remarks>
    public interface IExceptionIntentClassifier
    {
        /// <summary>
        /// Classifies the intent of a throw operation.
        /// </summary>
        /// <param name="throwOperation">The throw operation to classify.</param>
        /// <returns>The classified intent, or <see cref="ExceptionIntent.Unknown"/> if classification fails.</returns>
        ExceptionIntent Classify(IThrowOperation throwOperation);
    }
}
