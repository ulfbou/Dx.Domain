// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Scope.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Analyzers.Infrastructure.Scopes
{
    public enum Scope
    {
        S0, // Kernel
        S1, // Shared
        S2, // Domain
        S3  // Application
    }
}
