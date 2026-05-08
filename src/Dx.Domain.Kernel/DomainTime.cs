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

using static Dx.Domain.Dx;

namespace Dx.Domain
{
    /// <summary>
    /// Represents a UTC timestamp used by the kernel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type is immutable and thread-safe. It carries no mutable state.
    /// </para>
    /// <para>
    /// Kernel policy: only monotonic checks are performed; no clock correction is applied.
    /// </para>
    /// <para>
    /// The <see cref="Utc"/> value is always normalized to UTC with an offset of zero.
    /// </para>
    /// </remarks>
    public readonly record struct DomainTime
    {
        /// <summary>
        /// Gets the UTC timestamp.
        /// </summary>
        public DateTimeOffset Utc { get; }

        private DomainTime(DateTimeOffset utc) => Utc = utc;

        /// <summary>
        /// Gets the current timestamp.
        /// </summary>
        /// <returns>A new <see cref="DomainTime"/> representing the current UTC time.</returns>
        public static DomainTime Now()
            => new(DateTimeOffset.UtcNow);

        /// <summary>
        /// Creates a <see cref="DomainTime"/> from the specified UTC value.
        /// </summary>
        /// <param name="utc">The UTC value. Must have an offset of zero.</param>
        /// <returns>A new <see cref="DomainTime"/> instance.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="utc"/> does not have an offset of zero.</exception>
        internal static DomainTime From(DateTimeOffset utc)
        {
            Invariant.That(
                utc.Offset == TimeSpan.Zero,
                "DomainTime.Invariant.Utc",
                "DomainTime must be UTC.");

            return new DomainTime(utc);
        }
    }
}
