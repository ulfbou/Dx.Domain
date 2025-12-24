// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageCapabilities.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Defines capabilities that a generator stage may require.
    /// Used for DX7001 enforcement - undeclared I/O detection.
    /// </summary>
    [Flags]
    public enum StageCapabilities
    {
        /// <summary>
        /// No special capabilities required - pure computation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Stage requires file system access.
        /// </summary>
        FileSystemRead = 1 << 0,

        /// <summary>
        /// Stage requires file system write access.
        /// </summary>
        FileSystemWrite = 1 << 1,

        /// <summary>
        /// Stage requires network access.
        /// </summary>
        NetworkAccess = 1 << 2,

        /// <summary>
        /// Stage requires database access.
        /// </summary>
        DatabaseAccess = 1 << 3,

        /// <summary>
        /// Stage requires environment variable access.
        /// </summary>
        EnvironmentAccess = 1 << 4,

        /// <summary>
        /// All capabilities (use with caution).
        /// </summary>
        All = FileSystemRead | FileSystemWrite | NetworkAccess | DatabaseAccess | EnvironmentAccess
    }
}
