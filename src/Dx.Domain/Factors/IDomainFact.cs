// <summary>
//     <list type="bullet">
//         <item>
//             <term>File:</term>
//             <description>IDomainFact.cs</description>
//         </item>
//         <item>
//             <term>Project:</term>
//             <description>Dx.Domain</description>
//         </item>
//         <item>
//             <term>Description:</term>
//             <description>
//                 Declares the contract for domain facts, including identity, type, causation, and timestamp metadata.
//             </description>
//         </item>
//     </list>
// </summary>
// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IDomainFact.cs" company="Dx.Domain Team">
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
    /// <summary>
    /// Represents a domain fact with identity, type, causation, and timestamp metadata.
    /// </summary>
    public interface IDomainFact
    {
        /// <summary>Gets the unique identifier of the fact.</summary>
        FactId Id { get; }

        /// <summary>Gets the logical type or category of the fact.</summary>
        string FactType { get; }

        /// <summary>Gets the causation metadata associated with the fact.</summary>
        Causation Causation { get; }

        /// <summary>Gets the UTC timestamp when the fact occurred.</summary>
        DateTimeOffset UtcTimestamp { get; }
    }
}
