// <summary>
//     <list type="bullet">
//         <item>
//             <term>File:</term>
//             <description>ActorId.cs</description>
//         </item>
//         <item>
//             <term>Project:</term>
//             <description>Dx.Domain</description>
//         </item>
//         <item>
//             <term>Description:</term>
//             <description>
//                 Defines a strongly-typed identifier for actors (users, services, or systems) participating
//                 in domain operations.
//             </description>
//         </item>
//     </list>
// </summary>
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

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct ActorId : IEquatable<ActorId>
    {
        /// <summary>
        ///     An <see cref="ActorId"/> with an empty <see cref="Guid"/> value.
        /// </summary>
        public static readonly ActorId Empty = new(null);

        /// <summary>
        ///     The underlying <see cref="Guid"/> value of this <see cref="ActorId"/>.
        /// </summary>
        public Guid Value { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ActorId(Guid? guid)
        {
            Value = guid ?? Guid.Empty;
        }

        /// <summary>
        /// Creates a new <see cref="ActorId"/> with a freshly generated <see cref="Guid"/> value.
        /// </summary>
        /// <returns>A non-empty <see cref="ActorId"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ActorId New() => new ActorId(Guid.NewGuid());

        /// <summary>
        /// Gets a value indicating whether this instance represents <see cref="Empty"/>.
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value == Guid.Empty;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ActorId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is ActorId other && Equals(other);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ActorId a, ActorId b) => a.Equals(b);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ActorId a, ActorId b) => !a.Equals(b);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => IsEmpty ? "ActorId.Empty" : $"ActorId({Value})";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ToString();
        }
    }
}
