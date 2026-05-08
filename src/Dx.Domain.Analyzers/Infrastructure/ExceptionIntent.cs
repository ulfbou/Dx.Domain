// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ExceptionIntent.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Analyzers.Infrastructure
{
    /// <summary>
    /// Represents the intent behind an exception being thrown.
    /// </summary>
    /// <remarks>
    /// This enumeration imposes no runtime semantics. It is pure analyzer vocabulary used for static classification.
    /// Values are interpreted conservatively by analyzers.
    /// </remarks>
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
}
