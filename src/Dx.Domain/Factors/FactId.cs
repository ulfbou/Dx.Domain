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

using System.Diagnostics;

namespace Dx.Domain.Factors
{
    /// <summary>
    /// Strongly-typed identifier for a domain fact.
    /// </summary>
    [DebuggerDisplay("FactId = {Value:N}")]
    public readonly struct FactId : IEquatable<FactId>
    {
        /// <summary>Gets the underlying <see cref="Guid"/> value.</summary>
        public Guid Value { get; }

        private FactId(Guid value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="FactId"/> with a freshly generated <see cref="Guid"/> value.
        /// </summary>
        /// <returns>A new <see cref="FactId"/> where <see cref="Value"/> is non-empty.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FactId New() => new(Guid.NewGuid());

        /// <summary>
        /// Creates a <see cref="FactId"/> from an existing <see cref="Guid"/> value.
        /// </summary>
        /// <param name="value">The GUID to wrap.</param>
        /// <returns>A <see cref="FactId"/> whose <see cref="Value"/> is <paramref name="value"/>.</returns>
        public static FactId From(Guid value) => new(value);

        /// <inheritdoc />
        public bool Equals(FactId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is FactId other && Equals(other);

        /// <summary>
        /// Determines whether two <see cref="FactId"/> values are equal.
        /// </summary>
        /// <param name="left">The first identifier to compare.</param>
        /// <param name="right">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(FactId left, FactId right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="FactId"/> values are not equal.
        /// </summary>
        /// <param name="left">The first identifier to compare.</param>
        /// <param name="right">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(FactId left, FactId right) => !left.Equals(right);

        /// <summary>
        /// Implicitly converts a <see cref="Guid"/> to a <see cref="FactId"/>.
        /// </summary>
        /// <param name="value">The GUID to wrap.</param>
        /// <returns>A new <see cref="FactId"/> with <see cref="Value"/> set to <paramref name="value"/>.</returns>
        /// <remarks>This conversion is lossless and never throws.</remarks>
        public static implicit operator FactId(Guid value) => From(value);

        /// <summary>
        /// Explicitly converts a <see cref="FactId"/> to its underlying <see cref="Guid"/> value.
        /// </summary>
        /// <param name="factId">The identifier to unwrap.</param>
        /// <returns>The underlying <see cref="Guid"/> value.</returns>
        /// <remarks>This conversion is lossless and never throws.</remarks>
        public static explicit operator Guid(FactId factId) => factId.Value;

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();
    }
}
