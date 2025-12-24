// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageAssertionSet.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageAssertionSet.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Immutable;

using Dx.Domain.Generators.Core;
using Dx.Domain.Generators.Internal;

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Defines the structural prerequisites for a generator stage.
    /// Acts as a "Pre-Flight Checklist" enforced by the Orchestrator.
    /// </summary>
    public sealed class StageAssertionSet
    {
        public static readonly StageAssertionSet Empty = new(
            ImmutableHashSet<string>.Empty,
            ImmutableHashSet<string>.Empty);

        public ImmutableHashSet<string> RequiredKeys { get; }
        public ImmutableHashSet<string> ForbiddenKeys { get; }

        private StageAssertionSet(ImmutableHashSet<string> required, ImmutableHashSet<string> forbidden)
        {
            RequiredKeys = required;
            ForbiddenKeys = forbidden;
        }

        public static Builder Create() => new();

        /// <summary>
        /// Validates assertions against the current state of the fact store.
        /// </summary>
        /// <param name="store">The monotonic store to check against.</param>
        /// <returns>A Result indicating success or a detailed DomainError if assertions fail.</returns>
        public Result<Unit> Validate(MonotonicFactStore store)
        {
            var missing = new List<string>();
            var violations = new List<string>();

            // Check Requirements using the store's TryGet pattern
            foreach (var key in RequiredKeys)
            {
                if (!store.TryGet(key, out _))
                {
                    missing.Add(key);
                }
            }

            // Check Forbidden facts
            foreach (var key in ForbiddenKeys)
            {
                if (store.TryGet(key, out _))
                {
                    violations.Add(key);
                }
            }

            if (missing.Count == 0 && violations.Count == 0)
            {
                return Dx.Result.Ok();
            }

            var details = $"Missing: [{string.Join(", ", missing)}], Forbidden: [{string.Join(", ", violations)}]";

            // FIX: Correctly call the bridged factory with the single string parameter
            return Dx.Result.Failure<Unit>(GeneratorFaults.AssertionFailed(details));
        }

        public sealed class Builder
        {
            private readonly HashSet<string> _required = new();
            private readonly HashSet<string> _forbidden = new();

            public Builder Require<T>(FactKey<T> key) where T : notnull
            {
                _required.Add(key.FullyQualifiedName);
                return this;
            }

            public Builder Forbid<T>(FactKey<T> key) where T : notnull
            {
                _forbidden.Add(key.FullyQualifiedName);
                return this;
            }

            public StageAssertionSet Build() => new(
                _required.ToImmutableHashSet(),
                _forbidden.ToImmutableHashSet());
        }
    }
}
