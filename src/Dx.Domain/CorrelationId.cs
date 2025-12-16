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

namespace Dx.Domain
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a strongly-typed identifier used to correlate related operations or requests across system
    /// boundaries.
    /// </summary>
    /// <remarks>A CorrelationId encapsulates a GUID value to provide a consistent way to track and associate
    /// activities, such as logging or distributed tracing, throughout an application's workflow. Use
    /// <see cref="CorrelationId.Empty"/> to represent an uninitialized or absent correlation identifier. This 
    /// struct is immutable and can be compared for equality.</remarks>
    [DebuggerDisplay("CorrelationId = {Value:N}")]
    public readonly struct CorrelationId : IEquatable<CorrelationId>
    {
        /// <summary>
        /// Gets an empty <see cref="CorrelationId"/> whose underlying value is <see cref="Guid.Empty"/>.
        /// </summary>
        public static readonly CorrelationId Empty = new(Guid.Empty);

        /// <summary>
        /// Gets the underlying <see cref="Guid"/> value.
        /// </summary>
        public Guid Value { get; }

        public CorrelationId(Guid value) => Value = value;

        /// <summary>
        /// Gets a value indicating whether this identifier is empty.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsEmpty => Value == Guid.Empty;

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
        public static bool operator ==(CorrelationId a, CorrelationId b) => a.Equals(b);

        /// <inheritdoc />
        public static bool operator !=(CorrelationId a, CorrelationId b) => !a.Equals(b);
    }
}
