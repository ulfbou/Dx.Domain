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

namespace Dx.Domain
{
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Represents a strongly typed identifier for an actor, backed by a GUID value.
    /// </summary>
    /// <remarks>Use the ActorId type to uniquely identify actors within a distributed system or application.
    /// ActorId provides value-based equality and can be compared, serialized, or used as a key in collections. An
    /// ActorId with a value equal to Guid.Empty is considered empty and can be accessed via the static Empty
    /// property.</remarks>
    [DebuggerDisplay("ActorId = {Value}")]
    public readonly struct ActorId : IEquatable<ActorId>
    {
        /// <summary>
        /// Gets an <see cref="ActorId"/> whose underlying value is <see cref="Guid.Empty"/>.
        /// </summary>
        public static readonly ActorId Empty = new(Guid.Empty);

        /// <summary>
        /// Gets the underlying <see cref="Guid"/> value for this actor.
        /// </summary>
        public Guid Value { get; }

        private ActorId(Guid value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new <see cref="ActorId"/> with a freshly generated <see cref="Guid"/> value.
        /// </summary>
        /// <returns>A non-empty <see cref="ActorId"/>.</returns>
        public static ActorId New() => new ActorId(Guid.NewGuid());

        /// <summary>
        /// Gets a value indicating whether this instance represents <see cref="Empty"/>.
        /// </summary>
        public bool IsEmpty => Value == Guid.Empty;

        /// <inheritdoc />
        public bool Equals(ActorId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is ActorId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Determines whether two <see cref="ActorId"/> values are equal.
        /// </summary>
        /// <param name="a">The first identifier to compare.</param>
        /// <param name="b">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(ActorId a, ActorId b) => a.Equals(b);

        /// <summary>
        /// Determines whether two <see cref="ActorId"/> values are not equal.
        /// </summary>
        /// <param name="a">The first identifier to compare.</param>
        /// <param name="b">The second identifier to compare.</param>
        /// <returns><see langword="true"/> if the identifiers are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(ActorId a, ActorId b) => !a.Equals(b);

        /// <inheritdoc />
        public override string ToString()
            => IsEmpty ? "ActorId.Empty" : $"ActorId({Value.ToString("N", CultureInfo.InvariantCulture)})";
    }
}
