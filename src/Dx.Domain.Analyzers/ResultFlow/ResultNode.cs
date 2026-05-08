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

using Microsoft.CodeAnalysis;

using System;
using System.Diagnostics;

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Represents a node in the result-flow graph corresponding to a Result-producing operation.
    /// </summary>
    /// <remarks>
    /// This type is used exclusively by analyzers to track data-flow for Result values.
    /// Nodes are identified by <see cref="Id"/> and are immutable except for internal analysis state.
    /// It carries analysis data only and imposes no runtime semantics outside compilation analysis.
    /// </remarks>
    [DebuggerDisplay("{Id} {Type.Name} State={State}")]
    public sealed class ResultNode : IEquatable<ResultNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultNode"/> class with the specified identifier, producer, and type.
        /// </summary>
        /// <param name="id">The unique identifier for the node.</param>
        /// <param name="producer">The operation that produces the result. Must not be null.</param>
        /// <param name="type">The type symbol of the result. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="producer"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
        public ResultNode(int id, IOperation producer, ITypeSymbol type)
        {
            Id = id;
            Producer = producer ?? throw new ArgumentNullException(nameof(producer));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <summary>
        /// Gets the unique identifier of the node.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the operation that produces the result.
        /// </summary>
        public IOperation Producer { get; }

        /// <summary>
        /// Gets the type symbol of the result.
        /// </summary>
        public ITypeSymbol Type { get; }

        internal ResultState State { get; set; }

        /// <summary>
        /// Determines whether the specified node is equal to the current node.
        /// </summary>
        /// <param name="other">The node to compare with the current node.</param>
        /// <returns><see langword="true"/> if the nodes have the same <see cref="Id"/>;
        /// otherwise, <see langword="false"/>.</returns>
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
