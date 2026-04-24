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
    [DebuggerDisplay("{Message}")]
    public sealed class FlowDiagnostic
    {
        public FlowDiagnostic(string message, IOperation? operation = null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Operation = operation;
        }
        public string Message { get; }
        public IOperation? Operation { get; }
    }
}
