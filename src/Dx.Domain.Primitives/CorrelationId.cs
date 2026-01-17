// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="CorrelationId.cs" company="Dx.Domain Team">
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
using System.Diagnostics;
using System.Globalization;

namespace Dx.Domain
{
    /// <summary>
    /// Correlates related operations across system boundaries.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Empty values are permitted to represent an uncorrelated context.
    /// </para>
    /// <para>
    /// Canonical string format is <c>"N"</c>.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("{Value,nq}")]
    public readonly struct CorrelationId :
        IEquatable<CorrelationId>,
        IParsable<CorrelationId>,
        ISpanFormattable
    {
        /// <summary>An empty correlation identifier.</summary>
        public static readonly CorrelationId Empty = new(Guid.Empty);

        /// <summary>The underlying GUID value.</summary>
        public Guid Value { get; }

        private CorrelationId(Guid value) => Value = value;

        /// <summary>Creates a new random <see cref="CorrelationId"/>.</summary>
        public static CorrelationId New() => new(Guid.NewGuid());

        /// <summary>Creates a <see cref="CorrelationId"/> from a GUID.</summary>
        public static CorrelationId FromGuid(Guid value) => new(value);

        /// <inheritdoc />
        public static CorrelationId Parse(string s, IFormatProvider? provider)
            => new(Guid.ParseExact(s ?? throw new ArgumentNullException(nameof(s)), "N"));

        /// <inheritdoc />
        public static bool TryParse(string? s, IFormatProvider? provider, out CorrelationId result)
        {
            if (Guid.TryParseExact(s, "N", out var g))
            {
                result = new CorrelationId(g);
                return true;
            }

            result = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => ((ISpanFormattable)Value).TryFormat(destination, out charsWritten, format, provider);

        /// <inheritdoc />
        public bool Equals(CorrelationId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is CorrelationId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString()
            => Value.ToString("N", CultureInfo.InvariantCulture);

        /// <inheritdoc />
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(format, formatProvider);

        /// <summary>
        /// Determines whether two specified CorrelationId instances are equal.
        /// </summary>
        /// <param name="left">The first identifier to compare.</param>
        /// <param name="right">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(CorrelationId left, CorrelationId right)
            => left.Equals(right);

        /// <summary>
        /// Determines whether two specified CorrelationId instances are not equal.
        /// </summary>
        /// <param name="left">The first identifier to compare.</param>
        /// <param name="right">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(CorrelationId left, CorrelationId right)
            => !(left == right);
    }
}
