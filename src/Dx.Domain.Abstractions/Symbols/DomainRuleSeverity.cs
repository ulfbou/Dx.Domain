// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DomainRuleSeverity.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Symbols
{
    /// <summary>
    /// Specifies the severity level of a domain rule evaluation result.
    /// </summary>
    /// <remarks>Use this enumeration to indicate whether a rule result is informational, a warning, or an
    /// error. The severity level can be used to determine how the result should be handled or displayed to
    /// users.</remarks>
    public enum DomainRuleSeverity
    {
        /// <summary>Indicates an informational rule result.</summary>
        Info = 0,

        /// <summary>Indicates a warning rule result.</summary>
        Warning = 1,

        /// <summary>Indicates an error rule result.</summary>
        Error = 2
    }
}
