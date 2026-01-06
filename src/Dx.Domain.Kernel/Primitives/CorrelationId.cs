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

using Dx.Domain.Kernel;

using System;
using System.Diagnostics;

namespace Dx.Domain.Primitives
{
    /// <summary>
    /// Represents a correlation identifier spanning multiple operations.
    /// </summary>
    [DebuggerDisplay("{Value,nq}")]
    public readonly struct CorrelationId : IEquatable<CorrelationId>
    {
        /// <summary>
        /// The empty correlation identifier.
        /// </summary>
        public static readonly CorrelationId Empty = new(Guid.Empty);

        /// <summary>
        /// Underlying GUID value.
        /// </summary>
        public Guid Value { get; }

        private CorrelationId(Guid value)
        {
            Invariant.That(value != Guid.Empty, Faults.Guard.GuidParameterCannotBeEmpty(nameof(value)));
            Value = value;
        }

        /// <summary>
        /// Creates a new correlation identifier.
        /// </summary>
        /// <returns>The correlation identifier.</returns>
        internal static CorrelationId New() => new(Guid.NewGuid());

        /// <summary>
        /// Creates a correlation identifier from the specified GUID value.
        /// </summary>
        /// <param name="value">The GUID value.</param>
        /// <returns>The correlation identifier.</returns>
        internal static CorrelationId From(Guid value) => new(value);

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

        /// <inheritdoc />
        public override string ToString() => Value.ToString("N");
    }
}
