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

using System.Diagnostics;

using static Dx.Dx;

namespace Dx.Domain
{
    /// <summary>
    /// Represents a strongly typed identifier for an actor, backed by a <see cref="Guid"/> value.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="ActorId"/> type to uniquely identify actors within a distributed system or application.
    /// <see cref="ActorId"/> provides value-based equality and can be compared, serialized, or used as a key in collections.
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct ActorId : IEquatable<ActorId>
    {
        /// <summary>
        /// Represents an uninitialized or default value of the ActorId type.
        /// </summary>
        /// <remarks>Use this field to represent a scenario where no valid actor identifier is available
        /// or assigned. The value of Empty is equivalent to an ActorId constructed with Guid.Empty.</remarks>
        public static readonly ActorId Empty = new ActorId(Guid.Empty);

        /// <summary>
        /// Gets the underlying <see cref="Guid"/> value for this actor.
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Initializes a new instance of the ActorId struct using the specified <see cref="Guid"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value that uniquely identifies the actor.</param>
        private ActorId(Guid value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="ActorId"/> with a freshly generated <see cref="Guid"/> value.
        /// </summary>
        /// <returns>A new unique <see cref="ActorId"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ActorId New() => new ActorId(Guid.NewGuid());

        /// <summary>
        /// Creates a new ActorId instance from the specified GUID value.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value to use for the ActorId. Must not be Guid.Empty.</param>
        /// <returns>An <see cref="ActorId"/> that represents the specified <see cref="Guid"/> value.</returns>
        /// <exception cref="InvariantViolationException">Thrown if the provided <see cref="Guid"/> value is <see cref="Guid.Empty"/>.</exception>
        /// <remarks>The method enforces the invariant that the provided <see cref="Guid"/> value is not
        /// <see cref="Guid.Empty"/>. If the value is <see cref="Guid.Empty"/>, an invariant violation is raised.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ActorId Create(Guid value)
        {
            Invariant.That(value != Guid.Empty, Dx.Faults.FactoryBypass("ActorId cannot be default or empty. Use ActorId.New()"));

            return new ActorId(value);
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
        public bool Equals(ActorId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is ActorId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ActorId left, ActorId right) => left.Equals(right);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ActorId left, ActorId right) => !left.Equals(right);

        /// <inheritdoc />
        public override string ToString() => Value.ToString("N");

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"ActorId={Value.ToString("N")}";
    }
}

