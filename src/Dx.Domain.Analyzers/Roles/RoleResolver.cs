// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="RoleResolver.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Linq;

using Dx.Domain;

using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Roles
{
    internal static class RoleResolver
    {
        public static DxAssemblyRole? Resolve(Compilation compilation)
        {
            var attrs = compilation.Assembly.GetAttributes()
                .Where(a => a.AttributeClass?.Name == "DxAssemblyRoleAttribute")
                .ToArray();

            if (attrs.Length != 1)
                return null;

            var arg = attrs[0].ConstructorArguments.FirstOrDefault();
            return arg.Value is int i ? (DxAssemblyRole)i : null;
        }
    }
}
