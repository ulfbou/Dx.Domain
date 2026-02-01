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

namespace Dx.Domain.Annotations
{
    /// <summary>
    /// Declares the architectural role of an assembly (pure metadata marker).
    /// </summary>
    /// <remarks>
    /// This attribute imposes no runtime semantics. Analyzers classify assembly role and
    /// enforce boundaries accordingly. Apply once per assembly:
    /// <c>[assembly: DxAssemblyRole(DxAssemblyRole.Domain)]</c>.
    /// SEE: Rule Charter → Role/Boundary Enforcement.
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
