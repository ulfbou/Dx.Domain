// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FactKey.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Represents a strongly typed key for identifying a fact by name.
    /// </summary>
    /// <remarks>Use this type to associate a specific name with a value of type T in scenarios where facts
    /// are stored or retrieved by key. FactKey{T} provides type safety when working with collections or registries of
    /// named facts.</remarks>
    /// <typeparam name="T">The type of the value associated with the fact. Must be a non-nullable type.</typeparam>
    /// <param name="Namespace">The namespace that groups related facts.</param>
    /// <param name="Name">The name that uniquely identifies the fact.</param>
    /// <summary>
    /// A strongly-typed, namespace-qualified identifier for a fact.
    /// </summary>
    public readonly record struct FactKey<T>(string Namespace, string Name)
        where T : notnull
    {
        public string FullyQualifiedName => $"{Namespace}.{Name}";
        public override string ToString() => FullyQualifiedName;

        public static implicit operator string(FactKey<T> key) => key.FullyQualifiedName;
    }
}
