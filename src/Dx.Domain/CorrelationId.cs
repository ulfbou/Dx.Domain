// <summary>
//     <list type="bullet">
//         <item>
//             <term>File:</term>
//             <description>CorrelationId.cs</description>
//         </item>
//         <item>
//             <term>Project:</term>
//             <description>Dx.Domain</description>
//         </item>
//         <item>
//             <term>Description:</term>
//             <description>
//                 Defines a strongly-typed correlation identifier for grouping related operations or requests
//                 across components and services.
//             </description>
//         </item>
//     </list>
// </summary>
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
    /// Represents a correlation identifier used to group related operations or requests.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CorrelationId(Guid value) => Value = value;

        /// <summary>
        /// Gets a value indicating whether this identifier is empty.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value == Guid.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFormat(Span<char> destination, out int charsWritten)
            => Value.TryFormat(destination, out charsWritten, "N");

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => Value.ToString("N");

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(CorrelationId other) => Value.Equals(other.Value);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is CorrelationId other && Equals(other);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(CorrelationId a, CorrelationId b) => a.Equals(b);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(CorrelationId a, CorrelationId b) => !a.Equals(b);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsEmpty ? "CorrelationId.Empty" : ToString();
        }
    }
}
