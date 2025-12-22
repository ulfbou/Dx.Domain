// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IFactSet.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Represents a mutable fact set for new facts added by a stage.
    /// </summary>
    public interface IFactSet
    {
        /// <summary>
        /// Adds a new fact to the set.
        /// </summary>
        /// <param name="key">The fact key.</param>
        /// <param name="value">The fact value.</param>
        void Add(string key, object value);

        /// <summary>
        /// Gets all facts as a read-only dictionary.
        /// </summary>
        IReadOnlyDictionary<string, object> AsReadOnly();
    }

    /// <summary>
    /// Represents a read-only fact set from prior stages (Monotonic Knowledge Store).
    /// </summary>
    public interface IReadOnlyFactSet
    {
        /// <summary>
        /// Tries to get a fact value by key.
        /// </summary>
        /// <param name="key">The fact key.</param>
        /// <param name="value">The fact value if found.</param>
        /// <returns>True if the fact exists, false otherwise.</returns>
        bool TryGetValue(string key, out object? value);

        /// <summary>
        /// Checks if a fact exists.
        /// </summary>
        /// <param name="key">The fact key.</param>
        /// <returns>True if the fact exists, false otherwise.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets all facts as a read-only dictionary.
        /// </summary>
        IReadOnlyDictionary<string, object> All { get; }
    }
}
