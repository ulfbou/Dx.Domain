// <summary>
//     <list type="bullet">
//         <item>
//             <term>File:</term>
//             <description>Fact{T}.cs</description>
//         </item>
//         <item>
//             <term>Project:</term>
//             <description>Dx.Domain</description>
//         </item>
//         <item>
//             <term>Description:</term>
//             <description>
//                 Defines the generic domain fact value object that carries a payload, causation metadata,
//                 and a timestamp.
//             </description>
//         </item>
//     </list>
// </summary>
// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Fact{T}.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Factors
{
    using Dx.Domain.Invariants;
    using System.Diagnostics;

    /// <summary>
    /// Represents a domain fact carrying a payload, causation metadata, and a timestamp.
    /// </summary>
    /// <typeparam name="T">The type of the payload associated with the fact.</typeparam>
    [DebuggerDisplay("Fact<{typeof(T).Name}> Id = {Id}, Type = {FactType}")]
    public readonly struct Fact<T> : IDomainFact where T : notnull
    {
        /// <inheritdoc />
        public FactId Id { get; }

        /// <inheritdoc />
        public string FactType { get; }

        /// <summary>Gets the payload associated with this fact.</summary>
        public T Payload { get; }

        /// <inheritdoc />
        public Causation Causation { get; }

        /// <inheritdoc />
        public DateTimeOffset UtcTimestamp { get; }

        private Fact(FactId id, string factType, T payload, Causation causation, DateTimeOffset utcTimestamp)
        {
            Id = id;
            FactType = factType;
            Payload = payload;
            Causation = causation;
            UtcTimestamp = utcTimestamp;
        }

        /// <summary>
        /// Creates a new domain fact with the specified type, payload, and causation metadata.
        /// </summary>
        /// <param name="factType">The logical type or category of the fact. Must not be null or whitespace.</param>
        /// <param name="payload">The fact payload.</param>
        /// <param name="causation">The causation metadata associated with this fact.</param>
        /// <returns>A new <see cref="Fact{T}"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
        public static Fact<T> Create(string factType, T payload, Causation causation)
        {
            Invariant.That(!string.IsNullOrWhiteSpace(factType), DomainErrors.Fact.MissingType);
            return new Fact<T>(FactId.New(), factType, payload, causation, DateTimeOffset.UtcNow);
        }
    }
}
