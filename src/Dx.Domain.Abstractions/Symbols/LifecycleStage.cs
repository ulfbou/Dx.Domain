// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="LifecycleStage.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Symbols
{
    /// <summary>
    /// Specifies the stages in the lifecycle of an object, such as construction, mutation, validation, and persistence.
    /// </summary>
    /// <remarks>Use this enumeration to indicate or track the current phase of an object's lifecycle in
    /// workflows or state management scenarios. The stages are ordered to reflect typical progression: Construction,
    /// Mutation, Validation, and Persistence.</remarks>
    public enum LifecycleStage
    {
        /// <summary>Indicates the construction phase of an object.</summary>
        Construction = 0,

        /// <summary>Indicates the mutation phase of an object.</summary>
        Mutation = 1,

        /// <summary>Indicates the validation phase of an object.</summary>
        Validation = 2,

        /// <summary>Indicates the persistence phase of an object.</summary>
        Persistence = 3
    }
}
