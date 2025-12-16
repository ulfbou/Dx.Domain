// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DomainErrors.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain
{
    /// <summary>
    /// Factory for creating strongly-typed, context-rich domain errors for all Result-related failures.
    /// </summary>
    internal sealed partial class DomainErrors
    {
        /// <summary>
        /// Provides predefined domain errors related to causation metadata requirements.
        /// </summary>
        public static partial class Causation
        {
            /// <summary>
            /// Gets a domain error that indicates a required correlation identifier is missing or empty.
            /// </summary>
            public static DomainError MissingCorrelation
                => DomainError.Create(
                    "Causation.MissingCorrelation",
                    "Causation requires a non-empty CorrelationId.");

            /// <summary>
            /// Gets a domain error that indicates a required TraceId is missing for causation operations.
            /// </summary>
            public static DomainError MissingTrace
                => DomainError.Create(
                    "Causation.MissingTrace",
                    "Causation requires a non-empty TraceId.");
        }

        /// <summary>
        /// Provides predefined domain errors related to fact validation.
        /// </summary>
        public static partial class Fact
        {
            /// <summary>
            /// Gets a domain error that indicates a required fact type is missing from a domain fact declaration.
            /// </summary>
            public static DomainError MissingType
                => DomainError.Create(
                    "Fact.MissingType",
                    "Domain facts must declare a fact type.");
        }

        /// <summary>
        /// Provides predefined domain errors related to general code and message validation.
        /// </summary>
        public static partial class Code
        {
            /// <summary>
            /// Represents an error message indicating that a value cannot be null or consist only of white-space
            /// characters.
            /// </summary>
            public const string NullOrWhitespace = "Code cannot be null or whitespace.";
        }

        /// <summary>
        /// Provides predefined domain errors related to general code and message validation.
        /// </summary>
        public static partial class Message
        {
            /// <summary>
            /// Represents the error message used when a value is null or consists only of white-space characters.
            /// </summary>
            public const string NullOrWhitespace = "Message cannot be null or whitespace.";
        }

        /// <summary>
        /// Provides predefined domain errors related to transition operations.
        /// </summary>
        public static partial class Transition
        {
            /// <summary>
            /// Represents the error message used when a transition failure does not provide a non-null DomainError.
            /// </summary>
            public const string MissingError = "Transition failure must provide a non-null DomainError.";

            /// <summary>
            /// Gets a domain error that indicates a transition did not produce any facts as required.
            /// </summary>
            public static DomainError MissingFacts
                => DomainError.Create(
                    "Transition.MissingFacts",
                    "Successful transitions must produce at least one fact.");
        }

        /// <summary>
        /// Provides predefined domain errors related to <see langword="Result"/> operations.
        /// </summary>
        public static partial class Result
        {
            /// <summary>
            /// Error for when a null DomainError object was supplied where a domain error was required.
            /// Intended for internal invariant/protection use.
            /// </summary>
            public static DomainError NullErrorNotAllowed
                => DomainError.Create(
                    "Result.NullErrorNotAllowed",
                    "A null DomainError is not allowed. All Result failures must provide an explicit error object.");

            /// <summary>
            /// Error for when a null value was supplied where a non-null was required.
            /// </summary>
            public static DomainError NullValueNotAllowed
                => DomainError.Create(
                    "Result.NullValueNotAllowed",
                    "A null value is not allowed for a successful Result. All successes must provide a concrete value.");

            /// <summary>
            /// Creates an error message indicating that a value is missing from a failure result.
            /// </summary>
            /// <param name="error">The error message to include in the returned string. Can be null.</param>
            /// <returns>A string describing that the value is not present, including the specified error message.</returns>
            public static string MissingValue(string? error) => $"Value is not present in a failure result. Error: {error}";

            /// <summary>
            /// Generates an error message indicating that an error is not present in a successful result.
            /// </summary>
            /// <param name="value">The value associated with the successful result. Can be null.</param>
            /// <returns>A string containing the error message, including the specified value.</returns>
            public static string MissingError(string? value) => $"Error is not present in a successful result. Value: {value}";
        }

        /// <summary>
        /// Provides static members for representing and handling domain errors related to validation scenarios.
        /// </summary>
        public static partial class Validation
        {
            /// <summary>
            /// Creates a domain error indicating that a required field is missing.
            /// </summary>
            /// <param name="fieldName">The name of the required field that is missing. Cannot be null or empty.</param>
            /// <returns>A <see cref="DomainError"/> representing the missing required field error.</returns>
            /// <exception cref="ArgumentException">Thrown if <paramref name="fieldName"/> is null or empty.</exception>
            public static DomainError MissingRequiredField(string fieldName)
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    throw new ArgumentException("Field name must be provided.", nameof(fieldName));
                }

                return DomainError.Create(
                    code: "Validation.MissingRequiredField",
                    message: $"Required field '{fieldName}' is missing.");
            }
        }
    }
}
