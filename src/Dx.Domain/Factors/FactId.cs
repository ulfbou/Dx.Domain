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

using static Dx.Dx;

using System.Diagnostics;

namespace Dx.Domain.Factors
{
    /// <summary>
    /// Strongly-typed identifier for a domain fact.
    /// </summary>
    /// <remarks>
    /// Uniquely identifies a fact within the event stream or persistence store.
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct FactId : IEquatable<FactId>
    {
        /// <summary>Gets the underlying <see cref="Guid"/> value.</summary>
        public Guid Value { get; }

        /// <summary>
        /// Initializes a new instance of the FactId struct with the specified <see cref="Guid"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value to assign to the FactId.</param>
        private FactId(Guid value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="FactId"/> with a freshly generated <see cref="Guid"/> value.
        /// </summary>
        /// <returns>A new <see cref="FactId"/> where <see cref="Value"/> is non-empty.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FactId New() => new(Guid.NewGuid());

        /// <summary>
        /// Creates a new FactId instance from the specified Guid value.
        /// </summary>
        /// <param name="value">The Guid value to use for the FactId. Must not be <see cref="Guid.Empty"/>.</param>
        /// <returns>A FactId that represents the specified <see cref="Guid"/> value.</returns>
        /// <exception cref="InvariantViolationException">Thrown if the provided <see cref="Guid"/> value is <see cref="Guid.Empty"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FactId FromGuid(Guid value)
        {
            Invariant.That(value != Guid.Empty, Dx.DomainErrors.FactoryBypass("FactId cannot be default or empty. Use FactId.New()"));
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

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FactId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is FactId other && Equals(other);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FactId left, FactId right) => left.Equals(right);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FactId left, FactId right) => !left.Equals(right);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Value.ToString("N");

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => Value.ToString("N");
    }
}
