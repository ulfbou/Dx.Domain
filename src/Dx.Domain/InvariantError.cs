// <summary>
//     <list type="bullet">
//         <item>
//             <term>File:</term>
//             <description>InvariantError.cs</description>
//         </item>
//         <item>
//             <term>Project:</term>
//             <description>Dx.Domain</description>
//         </item>
//         <item>
//             <term>Description:</term>
//             <description>
//                 Defines the diagnostic payload captured when an invariant is violated, including
//                 domain error, source location, correlation, and timing information.
//             </description>
//         </item>
//     </list>
// </summary>
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

namespace Dx.Domain
{
    using System.Diagnostics;

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
        /// Creates a new <see cref="InvariantError"/> instance with the specified diagnostic information.
        /// </summary>
        internal static InvariantError Create(
            DomainError domainError,
            string? messageOverride = null,
            CorrelationId correlationId = default,
            TraceId traceId = default,
            SpanId spanId = default,
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
                correlationId,
                traceId,
                spanId);
        }

        /// <summary>
        /// Gets the effective message for the invariant violation, using <see cref="MessageOverride"/> when present
        /// and falling back to <see cref="DomainError.Message"/> otherwise.
        /// </summary>
        public string EffectiveMessage => MessageOverride ?? DomainError.Message;

        /// <inheritdoc />
        public override string ToString()
            => $"{DomainError.Code}: {EffectiveMessage} @ {Member}:{Line}";
    }
}
