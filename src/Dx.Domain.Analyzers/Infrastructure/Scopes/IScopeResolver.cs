// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IScopeResolver.cs" company="Dx.Domain Team">
// Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
// This software is licensed under the MIT License.
// See the project's root <c>LICENSE</c> file for details.
// Contributions are welcome, subject to the terms of the project's license.
// See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

using System;
using System.Linq;

namespace Dx.Domain.Analyzers.Infrastructure.Scopes
{
    public interface IScopeResolver
    {
        Scope ResolveAssembly(IAssemblySymbol assembly);
        Scope ResolveSymbol(ISymbol symbol);
        bool IsKernelInternal(IAssemblySymbol assembly);
    }
}
