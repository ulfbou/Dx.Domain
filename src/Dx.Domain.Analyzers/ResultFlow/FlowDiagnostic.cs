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

using System.Diagnostics;

using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Represents a diagnostic produced during flow analysis.
    /// </summary>
    [DebuggerDisplay("{Message}")]
    public sealed class FlowDiagnostic
    {
        /// <summary>Initializes a new instance of the <see cref="FlowDiagnostic"/> class.</summary>
        /// <param name="message">The diagnostic message.</param>
        /// <param name="operation">The associated operation, if any.</param>
        public FlowDiagnostic(string message, IOperation? operation = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            Message = message;
            Operation = operation;
        }

        /// <summary>Gets the diagnostic message.</summary>
        public string Message { get; }

        /// <summary>Gets the associated operation.</summary>
        public IOperation? Operation { get; }
    }
}
