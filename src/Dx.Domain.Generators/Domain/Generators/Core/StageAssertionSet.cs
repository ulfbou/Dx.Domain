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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Dx.Domain.Generators.Core
{
    /// <summary>
    /// Represents a set of assertions emitted by a pipeline stage.
    /// Subsequent stages must validate compatibility with all prior assertion sets (Monotonic Knowledge invariant).
    /// </summary>
    public sealed class StageAssertionSet
    {
        /// <summary>
        /// Gets the name of the stage that emitted these assertions.
        /// </summary>
        public string StageName { get; }

        /// <summary>
        /// Gets the assertions as key-value pairs.
        /// </summary>
        public ImmutableDictionary<string, object> Assertions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StageAssertionSet"/> class.
        /// </summary>
        /// <param name="stageName">The name of the stage.</param>
        /// <param name="assertions">The assertions dictionary.</param>
        public StageAssertionSet(string stageName, IDictionary<string, object> assertions)
        {
            StageName = stageName ?? throw new ArgumentNullException(nameof(stageName));
            Assertions = assertions?.ToImmutableDictionary() ?? ImmutableDictionary<string, object>.Empty;
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
                if (Assertions.TryGetValue(priorKvp.Key, out var currentValue))
                {
                    // Check if values contradict
                    if (!AreValuesCompatible(priorKvp.Value, currentValue))
                    {
                        contradictionsList.Add(
                            $"Assertion '{priorKvp.Key}' contradicts: " +
                            $"prior stage '{priorAssertion.StageName}' = '{priorKvp.Value}'" +
                            $", current stage '{StageName}' = '{currentValue}'");
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

        private static bool AreValuesCompatible(object priorValue, object currentValue)
        {
            if (priorValue == null && currentValue == null)
                return true;

            if (priorValue == null || currentValue == null)
                return false;

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
    }
}
