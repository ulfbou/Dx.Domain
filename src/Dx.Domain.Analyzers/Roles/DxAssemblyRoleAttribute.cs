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

namespace Dx.Domain.Analyzers.Roles
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    internal sealed class DxAssemblyRoleAttribute : Attribute
    {
        public DxAssemblyRoleAttribute(DxAssemblyRole role)
        {
            Role = role;
        }

        public DxAssemblyRole Role { get; }
    }
}
