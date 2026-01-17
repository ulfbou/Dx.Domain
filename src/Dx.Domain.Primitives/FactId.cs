// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FactId.cs" company="Dx.Domain Team">
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
    /// Identifies an immutable domain fact.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Empty values are not permitted.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("{Value,nq}")]
    public readonly struct FactId :
        IEquatable<FactId>,
        IParsable<FactId>,
        ISpanFormattable
    {
        /// <summary>The underlying GUID value.</summary>
        public Guid Value { get; }

        private FactId(Guid value) => Value = value;

        /// <summary>Creates a new random <see cref="FactId"/>.</summary>
        public static FactId New() => new(Guid.NewGuid());

        /// <summary>Creates a <see cref="FactId"/> from a non-empty <see cref="Guid"/>.</summary>
        /// <param name="value">The GUID value.</param>
        /// <returns>A new <see cref="FactId"/> instance.</returns>
        public static FactId FromGuid(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("FactId cannot be empty.", nameof(value));

            return new FactId(value);
        }

        /// <inheritdoc />
        public static FactId Parse(string s, IFormatProvider? provider)
            => FromGuid(Guid.ParseExact(s ?? throw new ArgumentNullException(nameof(s)), "N"));

        /// <inheritdoc />
        public static bool TryParse(string? s, IFormatProvider? provider, out FactId result)
        {
            if (Guid.TryParseExact(s, "N", out var g) && g != Guid.Empty)
            {
                result = new FactId(g);
                return true;
            }

            result = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => ((ISpanFormattable)Value).TryFormat(destination, out charsWritten, format, provider);

        /// <inheritdoc />
        public override string ToString()
            => Value.ToString("N", CultureInfo.InvariantCulture);

        /// <inheritdoc />
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(format, formatProvider);

        /// <inheritdoc />
        public bool Equals(FactId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is FactId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Determines whether two <see cref="FactId"/> values are equal.
        /// </summary>
        /// <param name="left">The first identifier to compare.</param>
        /// <param name="right">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(FactId left, FactId right)
            => left.Equals(right);

        /// <inheritdoc />
        public static bool operator !=(FactId left, FactId right)
            => !(left == right);
    }
}
