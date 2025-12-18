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

using System.Diagnostics;
using System.Runtime.CompilerServices;

using static global::Dx.Dx;

namespace Dx.Domain
{
    /// <summary>
    /// Represents a strongly-typed identifier used to correlate related operations or requests across system
    /// boundaries.
    /// </summary>
    /// <remarks>
    /// The <see cref="CorrelationId"/> is essential for tracking the flow of a request through distributed components.
    /// It wraps a <see cref="Guid"/> to ensure uniqueness and type safety.
    /// A <see cref="CorrelationId"/> with a value of <see cref="Guid.Empty"/> is considered to be a context with no correlation.
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct CorrelationId : IEquatable<CorrelationId>
    {
        /// <summary>
        /// Gets an empty correlation identifier whose underlying value is all zeros.
        /// </summary>
        /// <remarks>Use this property to represent an uninitialized or default correlation identifier.
        /// The empty identifier is equivalent to a correlation ID constructed with a Guid value of all zeros.</remarks>
        public static readonly CorrelationId Empty = new(Guid.Empty);

        /// <summary>
        /// Gets a value indicating whether this identifier is empty.
        /// </summary>
        public bool IsEmpty => Value == Guid.Empty;

        /// <summary>Gets the underlying <see cref="Guid"/> value.</summary>
        public Guid Value { get; }

        /// <summary>
        /// Initializes a new instance of the CorrelationId struct with the specified <see cref="Guid"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value to assign to the correlation identifier.</param>
        private CorrelationId(Guid value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="CorrelationId"/> with a freshly generated <see cref="Guid"/> value.
        /// </summary>
        /// <returns>A new unique <see cref="CorrelationId"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static CorrelationId InternalNew() => new CorrelationId(Guid.NewGuid());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static CorrelationId InternalFrom(Guid value)
        {
            Invariant.That(value != Guid.Empty, Dx.Faults.FactoryBypass("CorrelationId cannot be default or empty. Use CorrelationId.New()"));
            return new CorrelationId(value);
        }

        /// <summary>
        /// Attempts to format the value as a 32-digit hexadecimal string without hyphens into the provided character
        /// span.
        /// </summary>
        /// <remarks>The formatted string uses the "N" format, which consists of 32 digits without
        /// hyphens. Ensure that the destination span is large enough to contain the entire formatted value; otherwise,
        /// the method returns false and no data is written.</remarks>
        /// <param name="destination">The span of characters in which to write the formatted value.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters written to the destination span.</param>
        /// <returns><see langword="true"/> if the formatting was successful and the value was written to the destination span; otherwise,
        /// <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFormat(Span<char> destination, out int charsWritten)
            => Value.TryFormat(destination, out charsWritten, "N");

        /// <inheritdoc />
        public override string ToString() => Value.ToString("N");

        /// <inheritdoc />
        public bool Equals(CorrelationId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is CorrelationId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public static bool operator ==(CorrelationId left, CorrelationId right) => left.Equals(right);

        /// <inheritdoc />
        public static bool operator !=(CorrelationId left, CorrelationId right) => !left.Equals(right);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"CorrelationId={ToString()}";
    }
}
