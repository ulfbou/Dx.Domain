// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="RoleMatrix.cs" company="Dx.Domain Team">
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Dx.Domain;

namespace Dx.Domain.Analyzers.Roles
{
    internal static class RoleMatrix
    {
        private static readonly Dictionary<DxAssemblyRole, DxAssemblyRole[]> Allowed = new()
        {
            { DxAssemblyRole.Contracts, new[]{ DxAssemblyRole.Contracts } },
            { DxAssemblyRole.Domain, new[]{ DxAssemblyRole.Contracts, DxAssemblyRole.Domain, DxAssemblyRole.Application, DxAssemblyRole.Shared } },
            { DxAssemblyRole.Application, new[]{ DxAssemblyRole.Contracts, DxAssemblyRole.Domain, DxAssemblyRole.Application, DxAssemblyRole.Shared } },
            { DxAssemblyRole.Infrastructure, new[]{ DxAssemblyRole.Domain, DxAssemblyRole.Application, DxAssemblyRole.Infrastructure } },
            { DxAssemblyRole.Host, new[]{ DxAssemblyRole.Application, DxAssemblyRole.Infrastructure, DxAssemblyRole.Host } },
            { DxAssemblyRole.Shared, new[]{ DxAssemblyRole.Shared } }
        };

        public static bool IsAllowed(DxAssemblyRole from, DxAssemblyRole to)
            => Allowed.TryGetValue(from, out var list) && list.Contains(to);
    }
}
