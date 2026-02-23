// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxAssemblyRoleAttribute.cs" company="Dx.Domain Team">
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
    /// Declares the architectural role of an assembly within the Dx.Domain framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute must be applied at the assembly level to indicate the assembly's
    /// role in the application architecture. Analyzers use this to enforce dependency
    /// rules and architectural boundaries.
    /// </para>
    /// <para>
    /// <b>Usage:</b> Apply once per assembly in AssemblyInfo.cs or any source file using:
    /// <c>[assembly: DxAssemblyRole(DxAssemblyRole.Domain)]</c>
    /// </para>
    /// <para>
    /// <b>Enforcement:</b> The DXK-001 analyzer will report an error if this attribute
    /// is missing or applied multiple times.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class DxAssemblyRoleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DxAssemblyRoleAttribute"/> class.
        /// </summary>
        /// <param name="role">The architectural role of the assembly.</param>
        public DxAssemblyRoleAttribute(DxAssemblyRole role)
        {
            Role = role;
        }

        /// <summary>
        /// Gets the architectural role of the assembly.
        /// </summary>
        public DxAssemblyRole Role { get; }
    }
}
