// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Metadata.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Annotations
{
    public sealed record AggregateMetadata(string Name, string Identity);

    /// <summary>
    /// Metadata describing identity shape for analyzers/generators (pure data).
    /// </summary>
    /// <remarks>
    /// Records must be immutable and contain no runtime logic. SEE: Annotations Spec → Metadata Records.
    /// </remarks>
    public sealed record IdentityMetadata(string Name, string? Example);
    public sealed record InvariantMetadata(string Code, string Description);

    public sealed record FactoryMetadata(string Name, string ResultType);
}
