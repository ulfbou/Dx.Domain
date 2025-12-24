// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IDomainFactExtensions.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Factors;

using System.Collections.Concurrent;
using System.Reflection;

namespace Dx.Domain.Generators.Core;

public static class IDomainFactExtensions
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> _cache = new();

    public static object GetPayload(this IDomainFact fact)
    {
        ArgumentNullException.ThrowIfNull(fact);

        var type = fact.GetType();
        var prop = _cache.GetOrAdd(type, t => t.GetProperty("Payload"));
        return prop?.GetValue(fact) ?? new object();
    }
}
