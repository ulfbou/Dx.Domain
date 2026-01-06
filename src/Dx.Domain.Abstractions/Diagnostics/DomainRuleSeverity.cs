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

namespace Dx.Domain.Diagnostics
{
    /// <summary>
    /// Specifies the severity level of a domain rule for tooling and analysis purposes.
    /// </summary>
    // DPI: Passive severity markers for tooling; no behavior.
    public enum DomainRuleSeverity
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }
}
