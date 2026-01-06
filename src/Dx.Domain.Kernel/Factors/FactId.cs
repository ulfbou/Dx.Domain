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

using Dx.Domain.Kernel;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Dx.Domain.Factors
{
    /// <summary>
    /// Strongly-typed identifier for a domain fact.
    /// </summary>
    /// <remarks>
    /// Uniquely identifies a fact within the event stream or persistence store.
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct FactId : IEquatable<FactId>, IComparable<FactId>, IComparable, ISpanFormattable
    {
        /// <summary>
        /// Represents an uninitialized or default value of the <see cref="FactId"/> type.
        /// </summary>
        public static readonly FactId Empty = new(Guid.Empty);

        /// <summary>Gets the underlying <see cref="Guid"/> value.</summary>
        public Guid Value { get; }

        /// <summary>
        /// Gets a value indicating whether the current value is empty.
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value == Guid.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactId"/> struct with the specified <see cref="Guid"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value to assign to the <see cref="FactId"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FactId(Guid value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="FactId"/> with a freshly generated <see cref="Guid"/> value.
        /// </summary>
        /// <returns>A new <see cref="FactId"/> where <see cref="Value"/> is non-empty.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static FactId New() => new(Guid.NewGuid());

        /// <summary>
        /// Creates a new <see cref="FactId"/> instance from the specified <see cref="Guid"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value to use for the <see cref="FactId"/>. Must not be <see cref="Guid.Empty"/>.</param>
        /// <returns>A <see cref="FactId"/> that represents the specified <see cref="Guid"/> value.</returns>
        /// <exception cref="InvariantViolationException">Thrown if the provided <see cref="Guid"/> value is <see cref="Guid.Empty"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static FactId From(Guid value)
        {
            Invariant.That(value != Guid.Empty, "FactId.From", "The provided Guid value must not be Guid.Empty.");
            return new(value);
        }

        /// <summary>
        /// Attempts to format the value as a 32-digit hexadecimal string without hyphens into the provided character span.
        /// </summary>
        /// <param name="destination">The span to write the formatted string to.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters written to the span.</param>
        /// <returns><see langword="true"/> if the formatting was successful; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFormat(Span<char> destination, out int charsWritten)
            => Value.TryFormat(destination, out charsWritten, "N");

        /// <summary>
        /// General-purpose span formatting implementation used by composite formatting APIs.
        /// </summary>
        /// <remarks>
        /// <see cref="Guid"/>'s span-based formatting does not accept an <see cref="IFormatProvider"/>, so the provider parameter is ignored.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => Value.TryFormat(destination, out charsWritten, format.IsEmpty ? "N" : format);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => Value.ToString("N", CultureInfo.InvariantCulture);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(format ?? "N", formatProvider ?? CultureInfo.InvariantCulture);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FactId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is FactId other && Equals(other);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(FactId other) => Value.CompareTo(other.Value);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object? obj)
        {
            if (obj is null || obj is not FactId other)
            {
                return 1;
            }

            return CompareTo(other);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FactId left, FactId right) => left.Equals(right);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FactId left, FactId right) => !left.Equals(right);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FactId left, FactId right) => left.CompareTo(right) < 0;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FactId left, FactId right) => left.CompareTo(right) <= 0;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FactId left, FactId right) => left.CompareTo(right) > 0;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FactId left, FactId right) => left.CompareTo(right) >= 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => $"FactId={Value.ToString("N", CultureInfo.InvariantCulture)}";
        }
    }
}
