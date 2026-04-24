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
    [DebuggerDisplay("{Id} {Type.Name} State={State}")]
    public sealed class ResultNode : IEquatable<ResultNode>
    {
        public ResultNode(int id, IOperation producer, ITypeSymbol type)
        {
            Id = id;
            Producer = producer ?? throw new ArgumentNullException(nameof(producer));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
        public int Id { get; }
        public IOperation Producer { get; }
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
        public override bool Equals(object? obj) => Equals(obj as ResultNode);
        public override int GetHashCode() => Id;
        public override string ToString() => $"ResultNode#{Id} Type={Type.ToDisplayString()} State={State}";
    }
}
