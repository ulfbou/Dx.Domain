// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="TemplateIntentResolver.cs" company="Dx.Domain Team">
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

using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Templates
{
    internal static class TemplateIntentResolver
    {
        private const string TemplateAttributeName = "Dx.Domain.DxTemplateAttribute";

        public static string? Resolve(Compilation compilation)
        {
            var attribute = compilation.Assembly.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == TemplateAttributeName);

            if (attribute == null)
                return null;

            if (attribute.ConstructorArguments.Length == 0)
                return null;

            return attribute.ConstructorArguments[0].Value?.ToString();
        }
    }
}
