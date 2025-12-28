// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AggregateMetadata.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Metadata
{
    public sealed record AggregateMetadata(
        string Name,
        ImmutableArray<string> Entities,
        ImmutableArray<string> ValueObjects,
        ImmutableArray<string> Invariants);
    public sealed record EntityMetadata(
        string Name,
        ImmutableArray<string> Properties,
        string? IdentityProperty);
    public sealed record ValueObjectMetadata(
        string Name,
        ImmutableArray<string> Components);
    public sealed record DomainEventMetadata(
        string Name,
        ImmutableArray<string> PayloadProperties);
}
#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices
{
    // Required so "init" compiles under netstandard2.0
    public sealed class IsExternalInit { }
}
#endif
#if NETSTANDARD2_0
public abstract class Record
{
    public override bool Equals(object obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var fields = GetType().GetProperties();
        foreach (var f in fields)
        {
            var thisValue = f.GetValue(this);
            var otherValue = f.GetValue(obj);
            if (!Equals(thisValue, otherValue))
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (var f in GetType().GetProperties())
            {
                var value = f.GetValue(this);
                hash = hash * 23 + (value?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }

    public T With<T>(Action<T> mutator) where T : Record, new()
    {
        var clone = new T();
        foreach (var f in GetType().GetProperties())
            f.SetValue(clone, f.GetValue(this));

        mutator(clone);
        return clone;
    }
}
#endif
