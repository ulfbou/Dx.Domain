// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ActorId.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Primitives
{
    /// <summary>
    /// Represents a strongly typed identifier for an actor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Canonical string format is <c>"N"</c> (32 hexadecimal characters, no separators).
    /// </para>
    /// <para>
    /// <see cref="ActorId"/> does not permit empty values.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("{Value,nq}")]
    public readonly struct ActorId :
        IEquatable<ActorId>,
        IParsable<ActorId>,
        ISpanFormattable
    {
        /// <summary>The underlying GUID value.</summary>
        public Guid Value { get; }

        private ActorId(Guid value) => Value = value;

        /// <summary>Creates a new random <see cref="ActorId"/>.</summary>
        public static ActorId New() => new(Guid.NewGuid());

        /// <summary>Creates an <see cref="ActorId"/> from a non-empty GUID.</summary>
        public static ActorId FromGuid(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("ActorId cannot be empty.", nameof(value));

            return new ActorId(value);
        }

        /// <inheritdoc />
        public static ActorId Parse(string s, IFormatProvider? provider)
            => FromGuid(Guid.ParseExact(s ?? throw new ArgumentNullException(nameof(s)), "N"));

        /// <inheritdoc />
        public static bool TryParse(string? s, IFormatProvider? provider, out ActorId result)
        {
            if (Guid.TryParseExact(s, "N", out var g) && g != Guid.Empty)
            {
                result = new ActorId(g);
                return true;
            }

            result = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => ((ISpanFormattable)Value).TryFormat(destination, out charsWritten, format, provider);

        /// <inheritdoc />
        public bool Equals(ActorId other)
            => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is ActorId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString()
            => Value.ToString("N", CultureInfo.InvariantCulture);

        /// <inheritdoc />
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(format, formatProvider);

        /// <inheritdoc />
        public static bool operator ==(ActorId left, ActorId right)
            => left.Equals(right);

        /// <inheritdoc />
        public static bool operator !=(ActorId left, ActorId right)
            => !(left == right);
    }
}
