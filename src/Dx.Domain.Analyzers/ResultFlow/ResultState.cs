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
    public enum ResultState
    {
        Created = 0,
        Checked = 1,
        Propagated = 2,
        Terminated = 3,
        Ignored = 4
    }
}
