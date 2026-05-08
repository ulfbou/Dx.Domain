// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ResultFlowEngineOptions.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Immutable;

namespace Dx.Domain.Analyzers.ResultFlow
{
    /// <summary>
    /// Represents configuration options for the result-flow engine.
    /// </summary>
    /// <remarks>
    /// This type is used exclusively by analyzers. It carries analysis configuration only and imposes no runtime semantics outside compilation analysis.
    /// </remarks>
    public sealed class ResultFlowEngineOptions
    {
        /// <summary>
        /// Gets the default options.
        /// </summary>
        public static readonly ResultFlowEngineOptions Default = new();

        /// <summary>
        /// Gets the metadata names used to identify Result types.
        /// </summary>
        public ImmutableArray<string> ResultTypeMetadataNames { get; init; }

        /// <summary>
        /// Gets the member names used to inspect Result state.
        /// </summary>
        public ImmutableArray<string> InspectionMemberNames { get; init; }

        /// <summary>
        /// Gets the configuration key for handler resolution.
        /// </summary>
        public string HandlerConfigKey { get; init; } = "dx_handler";

        /// <summary>
        /// Gets the configuration key for terminalizer resolution.
        /// </summary>
        public string TerminalizerConfigKey { get; init; } = "dx_terminalizer";
    }
}
