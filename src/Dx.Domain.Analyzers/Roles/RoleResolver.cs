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

using System;
using System.Linq;

using Dx.Domain.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers.Roles
{
    internal static class RoleResolver
    {
        public static DxAssemblyRole? Resolve(Compilation compilation)
        {
            return ResolveFromAttributes(compilation);
        }

        public static DxAssemblyRole? Resolve(Compilation compilation, AnalyzerConfigOptionsProvider optionsProvider)
        {
            var role = ResolveFromOptions(optionsProvider);
            if (role.HasValue)
                return role;

            return ResolveFromAttributes(compilation);
        }

        private static DxAssemblyRole? ResolveFromOptions(AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (optionsProvider.GlobalOptions.TryGetValue("build_property.DxResolvedRole", out var raw) &&
                Enum.TryParse(raw, true, out DxAssemblyRole parsed))
            {
                return parsed;
            }

            return null;
        }

        private static DxAssemblyRole? ResolveFromAttributes(Compilation compilation)
        {
            var attrs = compilation.Assembly.GetAttributes()
                .Where(a =>
                {
                    var name = a.AttributeClass?.Name;
                    return name != null && name.StartsWith("DxAssemblyRole", StringComparison.Ordinal);
                })
                .ToArray();

            if (attrs.Length == 0)
                return null;

            var arg = attrs[0].ConstructorArguments.FirstOrDefault();
            if (arg.Value is int i)
                return (DxAssemblyRole)i;

            if (arg.Value is DxAssemblyRole role)
                return role;

            return null;
        }
    }
}
