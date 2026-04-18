// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageResult.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Base result type for generator stages as defined in DX-002.
    /// The system MUST NOT throw exceptions for domain logic failures.
    /// </summary>
    public abstract record StageResult;

    /// <summary>
    /// Represents a successful stage execution.
    /// </summary>
    public sealed record SuccessResult(
        IFactSet NewFacts,
        IReadOnlyList<GeneratedArtifact> Artifacts
    ) : StageResult;

    /// <summary>
    /// Represents a failed stage execution with classification and diagnostics.
    /// </summary>
    public sealed record FailureResult(
        Diagnostics.FailureClass Classification,
        Diagnostics.GeneratorDiagnostic Diagnostic,
        Core.ResolutionRequest? Resolution // Required for DX1003/Ambiguity
    ) : StageResult;
}
