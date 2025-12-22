// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IReadOnlyPolicy.cs" company="Dx.Domain Team">
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
    /// Represents a read-only policy configuration for generator stages.
    /// </summary>
    public interface IReadOnlyPolicy
    {
        /// <summary>
        /// Tries to get a policy value by key.
        /// </summary>
        /// <param name="key">The policy key.</param>
        /// <param name="value">The policy value if found.</param>
        /// <returns>True if the policy exists, false otherwise.</returns>
        bool TryGetValue(string key, out object? value);

        /// <summary>
        /// Gets a policy value by key, or returns a default value if not found.
        /// </summary>
        /// <typeparam name="T">The type of the policy value.</typeparam>
        /// <param name="key">The policy key.</param>
        /// <param name="defaultValue">The default value to return if the policy is not found.</param>
        /// <returns>The policy value or the default value.</returns>
        T GetValueOrDefault<T>(string key, T defaultValue);
    }
}
