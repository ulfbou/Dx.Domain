// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="InvariantDiagnostic.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Diagnostics
{
    public readonly record struct InvariantDiagnostic
    {
        /// <summary>
        /// Stable identifier of the violated invariant rule.
        /// </summary>
        public string RuleId { get; init; }

        /// <summary>
        /// Human-friendly explanation of the invariant violation.
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// Canonical error code string associated with this diagnostic.
        /// </summary>
        public string ErrorCode { get; init; }

        /// <summary>
        /// Structured hints providing additional context.
        /// Allowed value types: primitives, strings, or small DTOs.
        /// </summary>
        public IReadOnlyDictionary<string, object> Hints { get; init; }

        /// <summary>
        /// Optional timestamp for the diagnostic.
        /// Must be provided explicitly or created via KernelClock.UtcNow().
        /// </summary>
        public DateTimeOffset? Timestamp { get; init; }

        /// <summary>
        /// Diagnostic category marker for tooling and adapters.
        /// </summary>
        public DiagnosticCategory Category { get; init; }
    }
}
