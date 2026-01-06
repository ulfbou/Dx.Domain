// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DomainTime.cs" company="Dx.Domain Team">
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

namespace Dx.Domain
{
    /// <summary>
    /// Value object representing a UTC timestamp used by the kernel.
    /// Kernel policy: only monotonic checks; no clock correction.
    /// </summary>
    public readonly record struct DomainTime(DateTimeOffset Utc)
    {
        public static DomainTime Now() => new DomainTime(DateTimeOffset.UtcNow);
    }
}
