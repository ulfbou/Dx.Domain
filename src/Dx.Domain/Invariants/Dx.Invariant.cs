// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dx.Invariant.cs" company="Dx.Domain Team">
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
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides the primary gateway for the Dx Domain Kernel, offering a unified facade for result creation 
    /// while encapsulating internal mechanical enforcement and validation utilities.
    /// </summary>
    /// <remarks>
    /// This class serves as the singular public entry point for domain primitives.
    /// <para><b>Public Surface:</b> Use <see cref="Result"/> for functional flow control.</para>
    /// <para><b>Internal Mechanics:</b> The <c>Invariant</c> and <c>Require</c> systems are reserved 
    /// for kernel-internal enforcement and are not visible to external consumers.</para>
    /// </remarks>
    public static partial class Dx
    {
        /// <summary>
        /// Provides methods for enforcing invariants within the domain by validating conditions and throwing exceptions
        /// when violations occur.
        /// </summary>
        /// <remarks>Use this class to assert domain-specific conditions that must always hold true. If an
        /// invariant is violated, an exception containing diagnostic information is thrown to aid in debugging and error
        /// handling. This class is intended for internal use within the domain layer to ensure consistency and correctness
        /// of business rules.</remarks>
        internal static partial class Invariant
        {
            /// <summary>
            /// Asserts that a specified condition is true and throws an exception with diagnostic information if the
            /// condition is false.
            /// </summary>
            /// <remarks>Use this method to enforce invariants or preconditions in domain logic. The exception
            /// includes detailed diagnostic information to aid in troubleshooting and tracing failures.</remarks>
            /// <param name="condition">The condition to evaluate. If <see langword="false"/>, an <see cref="InvariantViolationException"/> is
            /// thrown.</param>
            /// <param name="error">The domain error to associate with the exception if the assertion fails. Cannot be null.</param>
            /// <param name="message">An optional custom message to include in the exception. If null, the message from <paramref name="error"/>
            /// is used.</param>
            /// <param name="correlationId">An optional correlation identifier to include in the diagnostic information. Used for distributed tracing or
            /// logging.</param>
            /// <param name="traceId">An optional trace identifier to include in the diagnostic information. Used for distributed tracing or
            /// logging.</param>
            /// <param name="spanId">An optional span identifier to include in the diagnostic information. Used for distributed tracing or
            /// logging.</param>
            /// <param name="member">The name of the calling member. This value is automatically provided by the compiler.</param>
            /// <param name="file">The full path of the source file that contains the caller. This value is automatically provided by the
            /// compiler.</param>
            /// <param name="line">The line number in the source file at which the method is called. This value is automatically provided by
            /// the compiler.</param>
            /// <exception cref="InvariantViolationException">Thrown if <paramref name="condition"/> is <see langword="false"/>.</exception>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void That(
                bool condition,
                DomainError error,
                string? message = null,
                CorrelationId? correlationId = null,
                TraceId? traceId = null,
                SpanId? spanId = null,
                [CallerMemberName] string member = "",
                [CallerFilePath] string file = "",
                [CallerLineNumber] int line = 0)
            {
                if (condition)
                    return;

                var diagnostic = InvariantError.Create(
                    error,
                    message ?? error.Message,
                    correlationId ?? CorrelationId.Empty,
                    traceId ?? TraceId.Empty,
                    spanId ?? SpanId.Empty,
                    member,
                    file,
                    line);

                throw new InvariantViolationException(diagnostic);
            }

            /// <summary>
            /// Checks the specified condition and throws an exception if the condition is not met.
            /// </summary>
            /// <remarks>Use this method to avoid infinite loops and unnecessary creation of <see cref="DomainError"/> instances when the 
            /// invariant condition is not met. The error factory is only invoked if the condition fails.</remarks>
            /// <param name="condition">A value indicating whether the invariant condition has succeeded. If <see langword="true"/>, no
            /// exception is thrown.</param>
            /// <param name="errorFactory">A function that returns a <see cref="DomainError"/> describing the error to be used if the 
            /// invariant is violated. Cannot be <see langword="null"/>.</param>
            /// <param name="message">An optional custom message to include in the exception. If null, the message from the generated
            /// <see cref="DomainError"/> is used.</param>
            /// <param name="correlationId">An optional correlation identifier to include in the diagnostic information. Used for distributed tracing or
            /// logging.</param>
            /// <param name="traceId">An optional trace identifier to include in the diagnostic information. Used for distributed tracing or
            /// logging.</param>
            /// <param name="spanId">An optional span identifier to include in the diagnostic information. Used for distributed tracing or
            /// logging.</param>
            /// <param name="member">The name of the calling member. This value is automatically provided by the compiler.</param>
            /// <param name="file">The full path of the source file that contains the caller. This value is automatically provided by the
            /// compiler.</param>
            /// <param name="line">The line number in the source file at which the method is called. This value is automatically provided by
            /// the compiler.</param>
            /// <exception cref="InvariantViolationException">Thrown when <paramref name="condition"/> is <see langword="false"/>. The 
            /// exception contains diagnostic information about the violated invariant.</exception>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void That(
                bool condition,
                Func<DomainError> errorFactory,
                string? message = null,
                CorrelationId? correlationId = null,
                TraceId? traceId = null,
                SpanId? spanId = null,
                [CallerMemberName] string member = "",
                [CallerFilePath] string file = "",
                [CallerLineNumber] int line = 0)
            {
                // Internal check to ensure the factory itself isn't null. 
                // Uses Raw error to avoid circular dependency in DomainError.Create validation.
                Invariant.That(errorFactory != null, Faults.FactoryBypass("The invariant error factory function cannot be null."));

                if (condition)
                    return;

                var error = errorFactory!();
                var diagnostic = InvariantError.Create(
                    error,
                    message ?? error.Message,
                    correlationId ?? CorrelationId.Empty,
                    traceId ?? TraceId.Empty,
                    spanId ?? SpanId.Empty,
                    member,
                    file,
                    line);

                throw new InvariantViolationException(diagnostic);
            }
        }
    }
}
