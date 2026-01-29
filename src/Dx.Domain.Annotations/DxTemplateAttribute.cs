// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxTemplateAttribute.cs" company="Dx.Domain Team">
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

namespace Dx.Domain
{
    /// <summary>
    /// Declares the template intent for a template-generated assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class DxTemplateAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DxTemplateAttribute"/> class.
        /// </summary>
        /// <param name="template">The template identifier (for example, <c>dx-service</c>).</param>
        public DxTemplateAttribute(string template)
        {
            Template = template;
        }

        /// <summary>Gets the template identifier.</summary>
        public string Template { get; }
    }
}
