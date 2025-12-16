// <summary>
//     <list type="bullet">
//         <item>
//             <term>File:</term>
//             <description>FactId.cs</description>
//         </item>
//         <item>
//             <term>Project:</term>
//             <description>Dx.Domain</description>
//         </item>
//         <item>
//             <term>Description:</term>
//             <description>
//                 Defines the strongly-typed identifier used to uniquely reference domain facts.
//             </description>
//         </item>
//     </list>
// </summary>
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
    /// Represents the unique identifier of a domain fact.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct FactId : IEquatable<FactId>
    {
        /// <summary>Gets the underlying <see cref="Guid"/> value.</summary>
        public Guid Value { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FactId(Guid value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="FactId"/> with a freshly generated <see cref="Guid"/> value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FactId New() => new(Guid.NewGuid());

        /// <summary>
        /// Creates a <see cref="FactId"/> from an existing <see cref="Guid"/> value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FactId From(Guid value) => new(value);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FactId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is FactId other && Equals(other);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FactId left, FactId right) => left.Equals(right);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FactId left, FactId right) => !left.Equals(right);

        /// <summary>
        /// Implicitly converts a <see cref="Guid"/> to a <see cref="FactId"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FactId(Guid value) => From(value);

        /// <summary>
        /// Explicitly converts a <see cref="FactId"/> to its underlying <see cref="Guid"/> value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Guid(FactId factId) => factId.Value;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Value.GetHashCode();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => $"FactId {{ Value = {Value:N} }}";
        }
    }
}
