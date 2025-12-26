// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StagePayload.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Generators.Core;
using Dx.Domain.Generators.Diagnostics;

using System.Collections.Generic;

namespace Dx.Domain.Generators.Abstractions
{
    /// <summary>
    /// Canonical payload for a successful generator stage execution.
    /// Intended to be wrapped in <see cref="Result{TValue, TError}"/> rather than
    /// introducing a separate result discriminated union for generators.
    /// </summary>
    public sealed record StageSuccessPayload(
        IFactTransaction Transaction,
        IReadOnlyList<GeneratedArtifact> Artifacts);

    /// <summary>
    /// Canonical payload for a failed generator stage execution.
    /// Intended to be wrapped in <see cref="Result{TValue, TError}"/> with a
    /// generator-specific error type, keeping the failure semantics aligned with the
    /// core <c>Dx.Domain</c> result model.
    /// </summary>
    public sealed record StageFailurePayload(
        FailureClass Classification,
        GeneratorDiagnostic Diagnostic,
        ResolutionRequest? Resolution);
}
