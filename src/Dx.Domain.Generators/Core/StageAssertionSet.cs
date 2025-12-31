// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageAssertionSet.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Diagnostics;
using Dx.Domain.Generators.Model;
using Dx.Domain.Generators.Orchestration;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dx.Domain.Generators.Core
{
    /// <summary>
    /// Represents a set of assertions emitted by a pipeline stage.
    /// Acts as a contract for both pre-flight validation (Prerequisites) and 
    /// monotonic integrity checks (Compatibility).
    /// </summary>
    public sealed class StageAssertionSet
    {
        /// <summary>
        /// Gets the name of the stage that emitted these assertions.
        /// </summary>
        public string StageName { get; }

        /// <summary>
        /// Gets the assertions as key-value pairs, where the value is an executable assertion.
        /// </summary>
        public ImmutableDictionary<string, IStageAssertion> Assertions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StageAssertionSet"/> class.
        /// </summary>
        /// <param name="stageName">The name of the stage.</param>
        /// <param name="assertions">The assertions dictionary.</param>
        private StageAssertionSet(string stageName, IDictionary<string, IStageAssertion> assertions)
        {
            StageName = stageName ?? throw new ArgumentNullException(nameof(stageName));
            Assertions = assertions?.ToImmutableDictionary() ?? ImmutableDictionary<string, IStageAssertion>.Empty;
        }

        /// <summary>
        /// Validates that this assertion set is compatible with a previous assertion set.
        /// Returns true if compatible, false if there are contradictions.
        /// </summary>
        /// <param name="priorAssertion">The prior assertion set to check against.</param>
        /// <param name="contradictions">Output parameter containing any contradictions found.</param>
        /// <returns>True if compatible, false otherwise.</returns>
        public bool IsCompatibleWith(
            StageAssertionSet priorAssertion,
            out ImmutableList<string> contradictions)
        {
            var contradictionsList = new List<string>();

            foreach (var priorKvp in priorAssertion.Assertions)
            {
                if (Assertions.TryGetValue(priorKvp.Key, out var currentAssertion))
                {
                    // Check if the expected values of the assertions contradict each other
                    if (!AreValuesCompatible(priorKvp.Value.ExpectedValue, currentAssertion.ExpectedValue))
                    {
                        contradictionsList.Add(
                            $"Assertion '{priorKvp.Key}' contradicts: " +
                            $"prior stage '{priorAssertion.StageName}' expects '{priorKvp.Value.ExpectedValue}'" +
                            $", current stage '{StageName}' expects '{currentAssertion.ExpectedValue}'");
                    }
                }
            }

            contradictions = contradictionsList.ToImmutableList();
            return contradictions.Count == 0;
        }

        /// <summary>
        /// Validates compatibility with multiple prior assertion sets.
        /// </summary>
        /// <param name="priorAssertions">The collection of prior assertion sets.</param>
        /// <param name="allContradictions">Output parameter containing all contradictions found.</param>
        /// <returns>True if compatible with all prior assertions, false otherwise.</returns>
        public bool IsCompatibleWithAll(
            IEnumerable<StageAssertionSet> priorAssertions,
            out ImmutableList<string> allContradictions)
        {
            var contradictionsList = new List<string>();

            foreach (var priorAssertion in priorAssertions)
            {
                if (!IsCompatibleWith(priorAssertion, out var contradictions))
                {
                    contradictionsList.AddRange(contradictions);
                }
            }

            allContradictions = contradictionsList.ToImmutableList();
            return allContradictions.Count == 0;
        }

        private static bool AreValuesCompatible(object? priorValue, object? currentValue)
        {
            // If either side doesn't enforce a specific value (null), there is no conflict.
            if (priorValue == null || currentValue == null)
                return true;

            // For simple types, use equality
            if (priorValue.Equals(currentValue))
                return true;

            // For string values, compare case-insensitively
            if (priorValue is string priorStr && currentValue is string currentStr)
                return priorStr.Equals(currentStr, StringComparison.OrdinalIgnoreCase);

            // For numeric types, try comparison
            if (IsNumeric(priorValue) && IsNumeric(currentValue))
            {
                var priorNum = Convert.ToDouble(priorValue, System.Globalization.CultureInfo.InvariantCulture);
                var currentNum = Convert.ToDouble(currentValue, System.Globalization.CultureInfo.InvariantCulture);
                return Math.Abs(priorNum - currentNum) < double.Epsilon;
            }

            return false;
        }

        private static bool IsNumeric(object value)
        {
            return value is int or long or float or double or decimal;
        }

        public static Builder Create() => new Builder("UnnamedStage");

        public Result<StageSuccessPayload, StageFailurePayload> Validate(MonotonicFactStore store)
        {
            // Use the Snapshot to identify prior StageAssertionSets
            var snapshot = store.Snapshot();
            var priorAssertions = snapshot.Values
                .OfType<StageAssertionSet>()
                .ToList();

            if (!IsCompatibleWithAll(priorAssertions, out var contradictions))
            {
                var diagnostic = new GeneratorDiagnostic(
                    id: "DXG.AssertionViolation",
                    @class: FailureClass.IntentViolation,
                    title: "Stage Assertion Violation",
                    message: contradictions.FirstOrDefault() ?? "Unknown contradiction",
                    inputFingerprint: InputFingerprint.FromHash("unknown"), // Context required for real fingerprint
                    stageName: StageName,
                    location: null,
                    remediationOptions: Enumerable.Empty<Remediation>(),
                    fixPreview: null,
                    impact: ImpactLevel.Breaking);

                return DxDomain.Result.Failure<StageSuccessPayload, StageFailurePayload>(
                    new StageFailurePayload(
                        FailureClass.IntentViolation,
                        diagnostic,
                        null));
            }

            // In a validation context with no transaction, we provide empty artifacts/transaction.
            // Note: This relies on the caller not using the Payload for writing.
            return DxDomain.Result.Ok<StageSuccessPayload, StageFailurePayload>(
                new StageSuccessPayload(
                    new StageTransaction(store),
                    ImmutableList<GeneratedArtifact>.Empty));
        }

        public async Task<Result<StageSuccessPayload, StageFailurePayload>> ExecuteAsync(
            StageContext context,
            IFactTransaction transaction,
            CancellationToken ct)
        {
            // 1. Monotonicity Check: Validate Stage Assertions against Prior Facts
            // Iterate over .Values to access the IStageAssertion objects
            foreach (var assertion in Assertions.Values)
            {
                ct.ThrowIfCancellationRequested();

                // Evaluate the assertion against the read-only fact store
                var assertionResult = assertion.Evaluate(context.PriorFacts);

                if (assertionResult.IsFailure)
                {
                    var diagnostic = new GeneratorDiagnostic(
                        id: assertion.DiagnosticId,
                        @class: FailureClass.PolicyViolation,
                        title: "Domain Policy Violation",
                        message: $"Stage '{StageName}' failed assertion: {assertion.Description}. {assertionResult.Error.Message}",
                        inputFingerprint: context.Fingerprint,
                        stageName: StageName,
                        location: assertion.Location,
                        remediationOptions: assertion.Remediations,
                        fixPreview: null,
                        impact: ImpactLevel.Breaking);

                    return DxDomain.Result.Failure<StageSuccessPayload, StageFailurePayload>(
                        new StageFailurePayload(FailureClass.PolicyViolation, diagnostic, null));
                }
            }

            // 2. Fact Proposal Phase
            try
            {
                var completionKey = new FactKey<string>(StageName, "Status");
                var proposal = transaction.Propose(completionKey, "CompletedSuccessfully");

                if (proposal.IsFailure)
                {
                    var diagnostics = new GeneratorDiagnostic(
                        id: "DXG.FactProposalFailure",
                        @class: FailureClass.InfrastructureError,
                        title: "Fact Proposal Failure",
                        message: $"Stage '{StageName}' failed to propose fact: {proposal.Error.Message}",
                        inputFingerprint: context.Fingerprint,
                        stageName: StageName,
                        location: null,
                        remediationOptions: Enumerable.Empty<Remediation>(),
                        fixPreview: null,
                        impact: ImpactLevel.Breaking);

                    return DxDomain.Result.Failure<StageSuccessPayload, StageFailurePayload>(
                        new StageFailurePayload(
                            FailureClass.InfrastructureError,
                            diagnostics,
                            null));
                }
            }
            catch (Exception ex)
            {
                var diagnostics = new GeneratorDiagnostic(
                    id: "DXG.UnexpectedError",
                    @class: FailureClass.InternalError,
                    title: "Unexpected Stage Error",
                    message: $"Stage '{StageName}' encountered an unexpected error: {ex.Message}",
                    inputFingerprint: context.Fingerprint,
                    stageName: StageName,
                    location: null,
                    remediationOptions: Enumerable.Empty<Remediation>(),
                    fixPreview: null,
                    impact: ImpactLevel.Breaking);

                return DxDomain.Result.Failure<StageSuccessPayload, StageFailurePayload>(
                    new StageFailurePayload(
                        FailureClass.InternalError,
                        diagnostics,
                        null));
            }

            return DxDomain.Result.Ok<StageSuccessPayload, StageFailurePayload>(
                new StageSuccessPayload(transaction, ImmutableList<GeneratedArtifact>.Empty));
        }

        // ----------------------------------------------------------------------------------
        // Nested Types & Builder
        // ----------------------------------------------------------------------------------

        /// <summary>
        /// Defines the contract for an executable stage assertion.
        /// </summary>
        public interface IStageAssertion
        {
            string DiagnosticId { get; }
            string Description { get; }
            DiagnosticLocation? Location { get; }
            IEnumerable<Remediation> Remediations { get; }
            object? ExpectedValue { get; } // Used for compatibility checks

            Result<Unit, DomainError> Evaluate(IReadOnlyFactSet facts);
        }

        /// <summary>
        /// Asserts that a specific fact key exists in the context (Value agnostic).
        /// </summary>
        private sealed class RequiredFactAssertion : IStageAssertion
        {
            private readonly string _keyNamespace;
            private readonly string _keyName;
            private readonly string _fullKey;

            public string DiagnosticId => "DXG.MissingPrerequisite";
            public string Description => $"Required fact '{_fullKey}' is missing.";
            public DiagnosticLocation? Location => null;
            public IEnumerable<Remediation> Remediations => Enumerable.Empty<Remediation>();

            // For simple requirements, we assume existence implies valid state, 
            // so we don't enforce a specific value for compatibility checks.
            public object? ExpectedValue => null;

            public RequiredFactAssertion(string ns, string name)
            {
                _keyNamespace = ns;
                _keyName = name;
                _fullKey = $"{ns}:{name}"; // Align with FactKey.ToString() logic if needed
            }

            public Result<Unit, DomainError> Evaluate(IReadOnlyFactSet facts)
            {
                // We check variations of the key format to be robust
                if (facts.ContainsKey(_fullKey) || facts.ContainsKey($"{_keyNamespace}.{_keyName}"))
                {
                    return DxDomain.Result.Ok<Unit, DomainError>(Unit.Value);
                }

                return DxDomain.Result.Failure<Unit, DomainError>(
                    DxDomain.Faults.InvalidInput($"Prerequisite fact '{_fullKey}' was not found in the prior fact store."));
            }
        }

        /// <summary>
        /// Asserts that a fact exists AND matches a specific value.
        /// </summary>
        private sealed class ValueAssertion : IStageAssertion
        {
            private readonly string _key;
            private readonly object _expected;

            public string DiagnosticId => "DXG.ValueMismatch";
            public string Description => $"Fact '{_key}' must equal '{_expected}'.";
            public DiagnosticLocation? Location => null;
            public IEnumerable<Remediation> Remediations => Enumerable.Empty<Remediation>();
            public object? ExpectedValue => _expected;

            public ValueAssertion(string key, object expected)
            {
                _key = key;
                _expected = expected;
            }

            public Result<Unit, DomainError> Evaluate(IReadOnlyFactSet facts)
            {
                if (!facts.TryGetValue(_key, out var actual))
                {
                    return DxDomain.Result.Failure<Unit, DomainError>(
                        DxDomain.Faults.InvalidInput($"Fact '{_key}' is missing."));
                }

                if (!AreValuesCompatible(_expected, actual))
                {
                    return DxDomain.Result.Failure<Unit, DomainError>(
                         DxDomain.Faults.InvalidInput($"Fact '{_key}' has value '{actual}' but expected '{_expected}'."));
                }

                return DxDomain.Result.Ok<Unit, DomainError>(Unit.Value);
            }
        }

        public sealed class Builder
        {
            private readonly string _stageName;
            private readonly Dictionary<string, IStageAssertion> _assertions = new();

            public Builder(string stageName)
            {
                _stageName = stageName ?? throw new ArgumentNullException(nameof(stageName));
            }

            /// <summary>
            /// Adds a general value assertion.
            /// </summary>
            public Builder AddAssertion(string key, object value)
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("Assertion key cannot be null or whitespace.", nameof(key));

                // Wrap raw values in a ValueAssertion
                _assertions[key] = new ValueAssertion(key, value ?? throw new ArgumentNullException(nameof(value)));
                return this;
            }

            /// <summary>
            /// Adds a requirement that a specific fact must exist (prerequisite).
            /// </summary>
            public Builder Require(FactKey<DomainIntentModel> factKey)
            {
                // We store the requirement under a unique key derived from the fact
                var key = $"RequiresFact:{factKey.Namespace}:{factKey.Name}";
                _assertions[key] = new RequiredFactAssertion(factKey.Namespace, factKey.Name);
                return this;
            }

            public StageAssertionSet Build()
            {
                return new StageAssertionSet(_stageName, _assertions);
            }
        }
    }
}
