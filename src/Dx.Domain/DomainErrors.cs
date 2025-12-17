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

using Dx;

using static Dx.Dx;

namespace Dx.Domain
{
    /// <summary>
    /// Factory for creating strongly-typed, context-rich domain errors for all Result-related failures.
    /// </summary>
    internal sealed partial class DomainErrors
    {
        public static partial class Causation
        {
            public static DomainError MissingCorrelation
            => DomainError.Create(
            "Causation.MissingCorrelation",
            "Causation requires a non-empty CorrelationId.");

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
            public static DomainError MissingError
                => DomainError.Create(
                    "Transition.MissingError",
                    "Transition failure must provide a non-null DomainError.");

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
            /// Represents the error code used to indicate a missing value result.
            /// </summary>
            public const string MissingValueCode = "Result.MissingValue";

            /// <summary>
            /// Creates a message indicating that a value is missing from a failure result, including the specified
            /// error description if provided.
            /// </summary>
            /// <param name="error">An optional error description to include in the message. If null, a default message is used.</param>
            /// <returns>A string message indicating that the value is not present, including the specified error description or
            /// a default message if none is provided.</returns>
            public static string MissingValueMessage(string? error)
                => $"Value is not present in a failure result. Error: {error ?? "Not specified"}";

            public static string MissingErrorMessage(string? value)
                => $"Error is not present in a successful result. Value: {value ?? "Not specified"}";

            /// <summary>
            /// Creates a domain error indicating that an error value was accessed in a successful result where no error is present.
            /// </summary>
            /// <remarks>Use this method to generate a consistent error when an error value is accessed from a result that
            /// does not contain an error. This helps enforce correct usage patterns in result-handling code.</remarks>
            /// <typeparam name="TValue">The type of the value expected in the successful result.</typeparam>
            /// <typeparam name="TError">The type of the error associated with the result.</typeparam>
            /// <param name="error">The error value that was incorrectly accessed in a successful result.</param>
            /// <returns>A DomainError representing the invalid access of an error value in a successful result.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static DomainError MissingValue<TValue, TError>(TError error) where TValue : notnull where TError : notnull
                => DomainError.Create($"Result.MissingValue.{typeof(TValue)}", $"Error is not present in a successful result. Error: {error}");

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static DomainError MissingError<TValue, TError>(TValue value) where TValue : notnull where TError : notnull
                => DomainError.Create("Result.MissingError", $"Error is not present in a successful result. Value: {value}");
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
            public static DomainError MissingRequiredField(string fieldName)
            {
                Invariant.That(!string.IsNullOrEmpty(fieldName), DomainError.Create("Validation.InvalidFieldName", "Field name cannot be null or empty."));

                return DomainError.Create(
                    code: "Validation.MissingRequiredField",
                    message: $"Required field '{fieldName}' is missing.");
            }
        }
    }
}
