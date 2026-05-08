// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ResultState.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Defines the analysis states for a Result node in the result-flow graph.
    /// </summary>
    /// <remarks>
    /// This enumeration is used exclusively by analyzers to track how Result values flow through a method.
    /// It carries analysis data only and imposes no runtime semantics outside compilation analysis.
    /// </remarks>
    public enum ResultState
    {
        /// <summary>
        /// The Result value has been created but not yet examined.
        /// </summary>
        Created = 0,

        /// <summary>
        /// The Result value has been checked for success or failure.
        /// </summary>
        Checked = 1,

        /// <summary>
        /// The Result value has been propagated to a caller or another operation.
        /// </summary>
        Propagated = 2,

        /// <summary>
        /// The Result value has reached a terminal operation such as return or throw.
        /// </summary>
        Terminated = 3,

        /// <summary>
        /// The Result value has been ignored without checking its state.
        /// </summary>
        Ignored = 4
    }
}
