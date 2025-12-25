// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FileName.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain;
using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Artifacts;
using Dx.Domain.Generators.CodeGeneration;
using Dx.Domain.Generators.Common;
using Dx.Domain.Generators.Core;
using Dx.Domain.Generators.Diagnostics;
using Dx.Domain.Generators.Model;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dx.Domain.Generators.Stages
{
    /// <summary>
    /// Emits authoritative, strongly-typed Identity value objects.
    /// Implements the Two-Phase Emission Protocol to ensure provenance and determinism.
    /// </summary>
    /// <remarks>
    /// This stage identifies "Identity" candidates from the DIM via three rules:
    /// 1. Explicit metadata (Kind == "id").
    /// 2. Structural usage (referenced as an IdType in an Entity or Repository).
    /// 3. Domain convention (suffixed with "Id" and linked to an Aggregate).
    /// </remarks>
    public sealed class IdentityGeneratorStage : IGeneratorStage
    {
        /// <inheritdoc/>
        public string StageName => "Dx.Domain.Generator.Identity";

        /// <inheritdoc/>
        public string StageVersion => "v1.0.0";

        /// <inheritdoc/>
        public StageCapabilities Capabilities => StageCapabilities.None;

        /// <inheritdoc/>
        public StageAssertionSet Assertions => StageAssertionSet.Create()
            .Require(new FactKey<DomainIntentModel>("System", "DomainIntentModel"))
            .Build();

        /// <inheritdoc/>
        public async Task<Result<StageSuccessPayload, StageFailurePayload>> ExecuteAsync(
                    StageContext context,
                    IFactTransaction transaction,
                    CancellationToken ct)
        {
            // 1. Inference: Resolve the authoritative DIM from the prior fact set
            if (!context.PriorFacts.TryGetValue("System.DomainIntentModel", out var modelObj) ||
                modelObj is not DomainIntentModel dim)
            {
                // We can't return Failure(...) directly here because the method signature requires Result<StageSuccessPayload, StageFailurePayload>
                return Dx.Result.Failure<StageSuccessPayload, StageFailurePayload>(
                    Failure("DX4001",
                    FailureClass.InferenceFailure,
                    "DomainIntentModel is missing from prior facts. Pipeline sequence violation."));
            }

            try
            {
                // 2. Identity Detection (Heuristic-based, strictly deterministic)
                var identityCandidates = DetectIdentities(dim);
                var artifacts = new List<GeneratedArtifact>();

                // 3. Artifact Generation (Two-Phase Protocol)
                foreach (var identity in identityCandidates)
                {
                    ct.ThrowIfCancellationRequested();

                    // Phase A: Generate Payload
                    string rootNamespace = dim.Metadata.TryGetValue("Namespace", out var ns) ? ns : "Dx.Generated";
                    var body = GenerateIdentitySource(identity, rootNamespace);

                    // Phase B: Compute Provenance Signature
                    var contentHash = DeterministicHash.Compute(body);
                    var header = new Artifacts.GeneratedFileHeader(
                        DimVersion: dim.ModelVersion,
                        TemplateVersion: StageVersion,
                        InputFingerprint: context.Fingerprint.Value,
                        ContentHash: contentHash,
                        GeneratorName: StageName
                    );

                    // Consolidate into the final signed text
                    var signedText = $"// {GeneratedFileHeaderSerializer.Serialize(header)}\n{body}";
                    var path = $"Identity/{identity.Name}.g.cs";

                    artifacts.Add(new GeneratedArtifact(path, signedText, contentHash));
                }

                // 4. Success: Return artifacts and the (potentially expanded) fact transaction
                return Dx.Result.Ok<StageSuccessPayload, StageFailurePayload>(
                    new StageSuccessPayload(transaction, artifacts.ToImmutableList())
                );
            }
            catch (Exception ex)
            {
                return Dx.Result.Failure<StageSuccessPayload, StageFailurePayload>(
                    Failure("DX5001", FailureClass.InternalError,
                    $"Unexpected failure during Identity generation: {ex.Message}"));
            }
        }

        private static ImmutableArray<ValueObjectIntent> DetectIdentities(DomainIntentModel dim)
        {
            var detected = new HashSet<ValueObjectIntent>();

            // Heuristic 1: Explicitly tagged via "Kind" metadata
            var explicitIds = dim.ValueObjects
                .Where(v => string.Equals(v.Kind, "id", StringComparison.OrdinalIgnoreCase));

            foreach (var vo in explicitIds)
                detected.Add(vo);

            // Heuristic 2: Contextual Usage (Referenced as Primary Key in Entities or Repos)
            var pkNames = dim.Entities.Select(e => e.IdType)
                .Concat(dim.Repositories.Select(r => r.IdType))
                .ToHashSet(StringComparer.Ordinal);

            foreach (var vo in dim.ValueObjects.Where(v => pkNames.Contains(v.Name)))
            {
                detected.Add(vo);
            }

            // Heuristic 3: Naming Convention (Aggregate Root suffix)
            var aggregateIdNames = dim.Aggregates.Select(a => $"{a.Name}Id").ToHashSet();
            foreach (var vo in dim.ValueObjects.Where(v => aggregateIdNames.Contains(v.Name)))
            {
                detected.Add(vo);
            }

            return detected.OrderBy(v => v.Name, StringComparer.Ordinal).ToImmutableArray();
        }

        private static string GenerateIdentitySource(ValueObjectIntent identity, string rootNamespace)
        {
            // Note: In a full implementation, this uses a specialized T4 or Scriban template.
            // Here we use a deterministic string builder to satisfy Phase 3 requirements.
            var sb = new StringBuilder();

            // Fix CA1305: Use CultureInfo.InvariantCulture for all formatting
            sb.AppendFormat(CultureInfo.InvariantCulture, "namespace {0}.Domain.Identity;\n\n", rootNamespace);

            sb.AppendLine("[System.CodeDom.Compiler.GeneratedCode(\"Dx.Domain.Generator\", \"1.0.0\")]");

            sb.AppendFormat(CultureInfo.InvariantCulture,
                "public readonly partial struct {0} : System.IEquatable<{0}>\n", identity.Name);

            sb.AppendLine("{");

            sb.AppendFormat(CultureInfo.InvariantCulture,
                "    public {0}(System.Guid value) => Value = value;\n", identity.Name);

            sb.AppendLine("    public System.Guid Value { get; }");

            sb.AppendFormat(CultureInfo.InvariantCulture,
                "    public bool Equals({0} other) => Value.Equals(other.Value);\n", identity.Name);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private StageFailurePayload Failure(string code, FailureClass fClass, string message)
        {
            var diagnostic = new GeneratorDiagnostic(
                id: code,
                @class: fClass,
                title: "Identity Stage Fault",
                message: message,
                inputFingerprint: InputFingerprint.FromHash("unknown"),
                stageName: StageName,
                location: null,
                remediationOptions: ImmutableList<Remediation>.Empty,
                fixPreview: null,
                impact: ImpactLevel.Breaking
            );

            return new StageFailurePayload(fClass, diagnostic, null);
        }
    }
}
