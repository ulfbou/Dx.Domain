// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Faults.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Root facade into the Dx Domain Kernel, exposing public factories for results, identities,
    /// causation, and related primitives while keeping enforcement mechanics internal.
    /// </summary>
    /// <remarks>
    /// All partial implementations of <see cref="Dx"/> contribute focused entry points (for example
    /// results, identities, causation, invariants, and preconditions) but present a single, cohesive
    /// surface to consumers of the domain kernel.
    /// </remarks>
    public static partial class Dx
    {
        // ---------------------------------------------------------
        // INTERNAL MECHANICS (The "Back Office")
        // ---------------------------------------------------------

        /// <summary>
        /// Internal catalog of standardized kernel refusals and diagnostic errors used to maintain structural integrity.
        /// </summary>
        internal static partial class Faults
        {
            /// <summary>
            /// Predefined errors for primitive code and message validation, used for bootstrapping the kernel.
            /// </summary>
            public static class Guard
            {
                /// <summary> Gets a domain error that indicates a provided error code is null or whitespace. </summary>
                public static DomainError NullOrWhitespaceCode => DomainError.InternalCreate("Faults.Code.Null", "Code cannot be null or whitespace.", false);

                /// <summary> Gets a domain error that indicates a provided error message is null or whitespace. </summary>
                public static DomainError NullOrWhitespaceMessage => DomainError.InternalCreate("Faults.Message.Null", "Message cannot be null or whitespace.", false);
            }

            /// <summary>
            /// General infrastructure and factory-related refusals.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static DomainError FactoryBypass(string detail)
                => DomainError.InternalCreate("Faults.FactoryBypass", detail, false);

            /// <summary>
            /// Errors related to invalid internal input processing.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static DomainError InvalidInput(string detail)
                => DomainError.InternalCreate("Faults.InvalidInput", detail, false);

            /// <summary>
            /// Provides predefined domain errors related to causation validation.
            /// </summary>
            public static partial class Causation
            {
                /// <summary>
                /// Gets a domain error that indicates a required correlation identifier is missing or empty.
                /// </summary>
                public static DomainError MissingCorrelation
                    => DomainError.InternalCreate(
                        "Causation.MissingCorrelation",
                        "Causation requires a non-empty CorrelationId.",
                        false);

                /// <summary>
                /// Gets a domain error that indicates a required TraceId is missing for causation operations.
                /// </summary>
                public static DomainError MissingTrace
                    => DomainError.InternalCreate(
                        "Causation.MissingTrace",
                        "Causation requires a non-empty TraceId.",
                        false);
            }

            /// <summary>
            /// Provides predefined domain errors related to fact validation.
            /// </summary>
            public static partial class Fact
            {
                /// <summary>
                /// Gets a domain error that indicates a required fact type is missing from a domain fact declaration.
                /// </summary>
                public static DomainError MissingFactType
                    => DomainError.InternalCreate(
                        "Fact.MissingFactType",
                        "Domain facts must declare a fact type.",
                        false);

                /// <summary>
                /// Gets the domain error that indicates a required TraceId is missing from the causation of a domain fact.
                /// </summary>
                public static DomainError MissingTrace
                    => DomainError.InternalCreate(
                        "Fact.MissingTrace",
                        "Domain facts require a non-empty TraceId in their Causation.",
                        false);

                /// <summary>
                /// Gets a domain error that indicates a required payload is missing for a domain fact.
                /// </summary>
                public static DomainError MissingPayload
                    => DomainError.InternalCreate(
                        "Fact.MissingPayload",
                        "Domain facts require a non-null payload.",
                        false);
            }

            /// <summary>
            /// Diagnostic errors used for illegal state access within Result types.
            /// </summary>
            public static partial class Result
            {
                /// <summary>
                /// Creates a domain error indicating that a value is missing from a failure result.
                /// </summary>
                /// <typeparam name="TValue">The type of the expected value that is missing.</typeparam>
                /// <typeparam name="TError">The type of the error information.</typeparam>
                /// <param name="error">The error information associated with the missing value.</param>
                /// <returns>A <see cref="DomainError"/> representing the invalid state access.</returns>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static DomainError MissingValueOnFailure<TValue, TError>(TError error) where TValue : notnull where TError : notnull
                    => DomainError.InternalCreate(
                        "Result.InvalidState.MissingValue",
                        $"Cannot access Value of a failure result. Error context: {error}",
                        false);

                /// <summary>
                /// Creates a domain error indicating that an error value was requested from a successful result.
                /// </summary>
                /// <typeparam name="TValue">The type of the value contained in the successful result.</typeparam>
                /// <typeparam name="TError">The type of the error associated with the result.</typeparam>
                /// <param name="value">The value of the successful result for which an error was incorrectly requested.</param>
                /// <returns>A domain error representing the invalid attempt to access an error from a successful result.</returns>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static DomainError MissingErrorOnSuccess<TValue, TError>(TValue value) where TValue : notnull where TError : notnull
                    => DomainError.InternalCreate(
                        "Result.InvalidState.MissingError",
                        $"Cannot access Error of a successful result. Value context: {value}",
                        false);

                /// <summary>
                /// Gets a domain error that indicates a required payload is missing for a domain result.
                /// </summary>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static DomainError MissingPayload()
                    => DomainError.InternalCreate(
                        "Result.InvalidState.MissingPayload",
                        "Result is missing expected payload data.",
                        false);
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
                    => DomainError.InternalCreate(
                        "Transition.MissingError",
                        "Transition failure must provide a non-null DomainError.",
                        false);

                /// <summary>
                /// Gets a domain error that indicates a transition did not produce any facts as required.
                /// </summary>
                public static DomainError MissingFacts
                    => DomainError.InternalCreate(
                        "Transition.MissingFacts",
                        "Successful transitions must produce at least one fact.",
                        false);
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
                    // Internal check to ensure the fault generator itself is used correctly.
                    Invariant.That(!string.IsNullOrEmpty(fieldName), DomainError.InternalCreate("Validation.InvalidFieldName", "Field name cannot be null or empty.", false));

                    return DomainError.InternalCreate(
                        code: "Validation.MissingRequiredField",
                        message: $"Required field '{fieldName}' is missing.",
                        false);
                }
            }
        }
    }
}
