// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ResultNode.cs" company="Dx.Domain Team">
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

using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Represents a single Result value discovered during flow analysis.
    /// </summary>
    /// <remarks>
    /// Nodes are identified by producer operation and track state transitions. Equality is based on identifier only.
    /// </remarks>
    [DebuggerDisplay("{Id} {Type.Name} State={State}")]
    public sealed class ResultNode : IEquatable<ResultNode>
    {
        /// <summary>Initializes a new instance of the <see cref="ResultNode"/> class.</summary>
        /// <param name="id">The unique identifier for the node.</param>
        /// <param name="producer">The operation that produces the Result.</param>
        /// <param name="type">The Result type.</param>
        public ResultNode(int id, IOperation producer, ITypeSymbol type)
        {
            Id = id;
            Producer = producer ?? throw new ArgumentNullException(nameof(producer));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>Gets the unique identifier.</summary>
        public int Id { get; }

        /// <summary>Gets the producer operation.</summary>
        public IOperation Producer { get; }

        /// <summary>Gets the Result type.</summary>
        public ITypeSymbol Type { get; }

        internal ResultState State { get; set; }

        /// <inheritdoc/>
        public bool Equals(ResultNode? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            return Id == other.Id;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as ResultNode);

        /// <inheritdoc/>
        public override int GetHashCode() => Id;

        /// <inheritdoc/>
        public override string ToString() => $"ResultNode#{Id} Type={Type.ToDisplayString()} State={State}";
    }
}
