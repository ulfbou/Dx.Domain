// ----------------------------------------------------------------------------------
// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AggregateMetadata.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Immutable;

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
namespace System.Runtime.CompilerServices
{
    // This class is needed to enable "init" properties and "records" 
    // when targeting .NET Standard 2.0 or .NET Framework.
    internal static class IsExternalInit { }
}
