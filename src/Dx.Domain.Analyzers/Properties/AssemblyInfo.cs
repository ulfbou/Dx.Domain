// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AssemblyInfo.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
// </license>
// ----------------------------------------------------------------------------------

using System.Runtime.CompilerServices;

using Dx.Domain.Analyzers.Roles;

[assembly: InternalsVisibleTo("Dx.Domain.Analyzers")]
[assembly: InternalsVisibleTo("Dx.Domain.Tests")]
[assembly: InternalsVisibleTo("Dx.Domain.Analyzers.Tests")]
[assembly: DxAssemblyRole(DxAssemblyRole.Infrastructure)]
