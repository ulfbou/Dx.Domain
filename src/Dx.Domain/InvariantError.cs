// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="InvariantError.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Diagnostics;
using System.IO;

namespace Dx.Domain
{
    /// <summary>
    /// Represents detailed diagnostic information for a violated invariant.
    /// </summary>
    [DebuggerDisplay("InvariantError {DomainError.Code} @ {Member}:{Line}")]
    public sealed class InvariantError
    {
        /// <summary>Gets the domain error associated with the invariant violation.</summary>
        public DomainError DomainError { get; }

        /// <summary>Gets an optional message override to use instead of <see cref="DomainError.Message"/>.</summary>
        public string? MessageOverride { get; }

        /// <summary>Gets the member in which the invariant was evaluated.</summary>
        public string Member { get; }

        /// <summary>Gets the source file name where the invariant was evaluated.</summary>
        public string FileName { get; }

        /// <summary>Gets the line number in the source file where the invariant was evaluated.</summary>
        public int Line { get; }

        /// <summary>Gets the correlation identifier associated with the operation.</summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>Gets the trace identifier associated with the operation.</summary>
        public TraceId TraceId { get; }

        /// <summary>Gets the span identifier associated with the operation.</summary>
        public SpanId SpanId { get; }

        /// <summary>Gets the UTC timestamp when the invariant violation was recorded.</summary>
        public DateTime UtcTimestamp { get; }

        private InvariantError(
            DomainError domainError,
            string? messageOverride,
            string member,
            string fileName,
            int line,
            CorrelationId correlationId,
            TraceId traceId,
            SpanId spanId)
        {
            DomainError = domainError;
            MessageOverride = messageOverride;
            Member = member;
            FileName = fileName;
            Line = line;
            CorrelationId = correlationId;
            TraceId = traceId;
            SpanId = spanId;
            UtcTimestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new instance of the InvariantError class with the specified domain error and optional contextual
        /// information.
        /// </summary>
        /// <remarks>This method is intended for internal use to capture detailed context about where and
        /// why an invariant violation occurred. Caller information is automatically populated by the compiler and
        /// should not be set manually.</remarks>
        /// <param name="domainError">The domain error that describes the invariant violation. Cannot be null.</param>
        /// <param name="messageOverride">An optional message to override the default error message. If null, the default message from the domain
        /// error is used.</param>
        /// <param name="correlationId">An optional correlation identifier used to trace the error across systems. Defaults to an empty value if not
        /// specified.</param>
        /// <param name="traceId">An optional trace identifier for distributed tracing. Defaults to an empty value if not specified.</param>
        /// <param name="spanId">An optional span identifier for distributed tracing. Defaults to an empty value if not specified.</param>
        /// <param name="member">The name of the member that invoked this method. This value is automatically provided by the compiler.</param>
        /// <param name="file">The full path of the source file that contains the caller. This value is automatically provided by the
        /// compiler.</param>
        /// <param name="line">The line number in the source file at which this method is called. This value is automatically provided by
        /// the compiler.</param>
        /// <returns>An InvariantError instance containing the specified domain error and contextual information.</returns>
        internal static InvariantError Create(
            DomainError domainError,
            string? messageOverride = null,
            CorrelationId? correlationId = default,
            TraceId? traceId = default,
            SpanId? spanId = default,
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            var fileName = string.IsNullOrEmpty(file) ? string.Empty : Path.GetFileName(file);

            return new InvariantError(
                domainError,
                messageOverride,
                member,
                fileName,
                line,
                correlationId ?? CorrelationId.Empty,
                traceId ?? TraceId.Empty,
                spanId ?? SpanId.Empty);
        }

        /// <summary>
        /// Gets the effective message for the invariant violation, using <see cref="MessageOverride"/> when present
        /// and falling back to <see cref="DomainError.Message"/> otherwise.
        /// </summary>
        public string EffectiveMessage => MessageOverride ?? DomainError.Message;

        /// <inheritdoc />
        public override string ToString()
            => $"InvariantError={DomainError.Code}: {EffectiveMessage} @ {Member}:{Line}";
    }
}
