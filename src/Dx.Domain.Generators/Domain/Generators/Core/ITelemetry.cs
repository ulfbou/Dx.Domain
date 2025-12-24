// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ITelemetry.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Generators.Core
{
    /// <summary>
    /// Minimal telemetry surface for generator execution.
    /// Implementations may forward to OpenTelemetry, Application Insights,
    /// console logging, or remain no-op.
    /// </summary>
    public interface ITelemetry
    {
        void TrackCommitAttempt(string stageName, int proposalCount);
        void TrackCommitSuccess(string stageName, int committedCount);
        void TrackCommitFailure(string stageName, int conflictCount, string firstConflictKey);
    }
    /// <summary>
    /// Default telemetry implementation.
    /// Used when no telemetry pipeline is configured.
    /// </summary>
    internal sealed class NoopTelemetry : ITelemetry
    {
        public void TrackCommitAttempt(string stageName, int proposalCount) { }
        public void TrackCommitSuccess(string stageName, int committedCount) { }
        public void TrackCommitFailure(string stageName, int conflictCount, string firstConflictKey) { }
    }
}
