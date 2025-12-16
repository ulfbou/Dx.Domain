// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Invariant.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Invariants
{
    /// <summary>
    /// Provides methods for enforcing invariants within the domain by validating conditions and throwing exceptions
    /// when violations occur.
    /// </summary>
    /// <remarks>Use this class to assert domain-specific conditions that must always hold true. If an
    /// invariant is violated, an exception containing diagnostic information is thrown to aid in debugging and error
    /// handling. This class is intended for internal use within the domain layer to ensure consistency and correctness
    /// of business rules.</remarks>
    internal static class Invariant
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
    }
}
