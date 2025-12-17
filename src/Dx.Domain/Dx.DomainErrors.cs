// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dx.DomainErrors.cs" company="Dx.Domain Team">
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

namespace Dx
{
    /// <summary>
    /// Provides domain validation and error creation utilities for enforcing invariants, performing functional-style
    /// precondition checks, and generating strongly-typed domain errors within the domain layer.
    /// </summary>
    /// <remarks>The Dx class contains static helpers for asserting domain invariants, validating
    /// preconditions without exceptions, and constructing context-rich domain errors. These utilities are intended for
    /// use within the domain layer to ensure business rule consistency, model failures explicitly, and provide detailed
    /// diagnostic information for error handling and debugging. Use the Invariant class to enforce conditions that must
    /// always hold true, the Require class for functional-style validations that return Result types, and the
    /// <see cref="DomainErrors"/> class to create or retrieve standardized domain errors for various failure scenarios.</remarks>
    public static partial class Dx
    {
        /// <summary>
        /// Factory for creating strongly-typed, context-rich domain errors for all Result-related failures.
        /// </summary>
        internal sealed partial class DomainErrors
        {
            /// <summary>
            /// Creates a <see cref="DomainError"/> instance representing a factory bypass error with the specified detail message.
            /// </summary>
            /// <param name="detail">The detail message that describes the specific reason for the factory bypass error. Cannot be <see langword="null"/>.</param>
            /// <returns>A DomainError instance with the error code "DomainErrors.FactoryBypass" and the provided detail message.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static DomainError FactoryBypass(string detail)
                => DomainError.Create("DomainErrors.FactoryBypass", detail);

            /// <summary>
            /// Creates a domain error indicating that the input provided is invalid.
            /// </summary>
            /// <param name="detail">A message that describes the details of the invalid input. This value should provide context to help
            /// identify the cause of the error.</param>
            /// <returns>A <see cref="DomainError"/> instance representing an invalid input error with the specified detail
            /// message.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static DomainError InvalidInput(string detail)
                => DomainError.Create("DomainErrors.InvalidInput", detail);

            /// <summary>
            /// Provides predefined domain errors related to causation validation.
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
                /// Creates a domain error indicating that a value is missing from a failure result.
                /// </summary>
                /// <param name="error">An optional error message providing additional details about the missing value. Can be null.</param>
                /// <returns>A <see cref="DomainError"/> representing the missing value error, including the specified error message
                /// if provided.</returns>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static DomainError MissingValue(string? error)
                    => DomainError.Create(
                        "Result.MissingValue",
                        $"Value is not present in a failure result. Error: {error ?? "Not specified"}");

                /// <summary>
                /// Generates an error message indicating that an error is not present in a successful result.
                /// </summary>
                /// <param name="value">The value associated with the successful result. Can be null.</param>
                /// <returns>A <see cref="DomainError"/> representing the missing error in a successful result, including the specified value.</returns>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static DomainError MissingError(string? value) => DomainError.Create("Result.MissingError", $"Error is not present in a successful result. Value: {value ?? "Not specified"}");
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
}
