// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="TemplateRoleMap.cs" company="Dx.Domain Team">
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

using Dx.Domain.Analyzers.Roles;

namespace Dx.Domain.Analyzers.Templates
{
    internal sealed record TemplateDefinition(
        DxAssemblyRole Role,
        ImmutableArray<string> RequiredReferences,
        ImmutableArray<string> ForbiddenReferences);

    internal static class TemplateRoleMap
    {
        private static readonly ImmutableDictionary<string, TemplateDefinition> Map =
            new Dictionary<string, TemplateDefinition>(StringComparer.OrdinalIgnoreCase)
            {
                ["dx-service"] = new TemplateDefinition(
                    DxAssemblyRole.Host,
                    ImmutableArray.Create("Dx.Domain.Primitives", "Dx.Domain.Kernel", "Dx.Domain.Facts"),
                    ImmutableArray<string>.Empty)
            }.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);

        public static bool TryGet(string templateId, out TemplateDefinition definition)
            => Map.TryGetValue(templateId, out definition);
    }
}
