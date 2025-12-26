// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageContext.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Generators.Core;

using System;
using System.Collections.Generic;

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Context provided to generator stages during execution.
    /// Contains all inputs required for deterministic execution as defined in DX-002.
    /// </summary>
    public sealed class StageContext
    {
        /// <summary>
        /// Gets the input fingerprint for this generation run.
        /// </summary>
        public Core.InputFingerprint Fingerprint { get; }

        /// <summary>
        /// Gets the manifest (read-only configuration).
        /// </summary>
        public IReadOnlyDictionary<string, object> Manifest { get; }

        /// <summary>
        /// Gets the policy configuration.
        /// </summary>
        public IReadOnlyPolicy Policy { get; }

        /// <summary>
        /// Gets the fact set from prior stages (Monotonic Knowledge Store).
        /// </summary>
        public IReadOnlyFactSet PriorFacts { get; }

        /// <summary>
        /// Gets the clock service for deterministic time access.
        /// Replaces DateTime.Now to ensure referential transparency.
        /// </summary>
        public IClock Clock { get; }

        /// <summary>
        /// Gets the identity service for deterministic identity generation.
        /// Replaces Guid.NewGuid() to ensure referential transparency.
        /// </summary>
        public IDeterministicIdentity Identity { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StageContext"/> class.
        /// </summary>
        public StageContext(
            InputFingerprint fingerprint,
            IReadOnlyDictionary<string, object> manifest,
            IReadOnlyPolicy policy,
            IReadOnlyFactSet priorFacts,
            IClock clock,
            IDeterministicIdentity identity)
        {
            Fingerprint = fingerprint ?? throw new ArgumentNullException(nameof(fingerprint));
            Manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            PriorFacts = priorFacts ?? throw new ArgumentNullException(nameof(priorFacts));
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        }
    }
}
