// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FlowDiagnostic.cs" company="Dx.Domain Team">
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
using System.Diagnostics;

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Represents a diagnostic message produced during result-flow analysis.
    /// </summary>
    /// <remarks>
    /// This type is immutable and is used exclusively by analyzers to report flow analysis findings.
    /// It carries analysis data only and imposes no runtime semantics outside compilation analysis.
    /// </remarks>
    [DebuggerDisplay("{Message}")]
    public sealed class FlowDiagnostic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlowDiagnostic"/> class with the specified message and optional operation.
        /// </summary>
        /// <param name="message">The diagnostic message. Must not be null.</param>
        /// <param name="operation">The operation associated with the diagnostic, or null if not applicable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
        public FlowDiagnostic(string message, IOperation? operation = null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Operation = operation;
        }

        /// <summary>
        /// Gets the diagnostic message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the operation associated with the diagnostic, or null if not applicable.
        /// </summary>
        public IOperation? Operation { get; }
    }
}
