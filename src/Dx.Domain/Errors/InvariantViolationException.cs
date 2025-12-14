namespace Dx.Domain.Errors
{
    using System.Diagnostics;

    /// <summary>
    /// The exception that is thrown when a domain invariant is violated during application execution.
    /// </summary>
    /// <remarks>This exception is typically used to indicate that a business rule or domain constraint has been
    /// broken. It encapsulates a <see cref="DomainError"/> that provides detailed information about the violation,
    /// including an error code, message, and the source location where the invariant was breached. Use this exception to
    /// signal unrecoverable domain errors that should be handled at the application boundary.</remarks>
    [Serializable]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class InvariantViolationException : Exception
    {
        /// <summary>Gets the error information associated with the domain operation.</summary>
        public DomainError Error { get; }

        /// <summary>Gets the error code associated with this instance.</summary>
        public string Code => Error.Code;

        /// <summary> Gets the name of the member associated with the error, if available. </summary>
        public string Member => Error.Member;

        /// <summary>Gets the name of the file in which the error occurred.</summary>
        public string File => Error.File;

        /// <summary>Gets the line number where the error occurred.</summary>
        public int Line => Error.Line;

        /// <summary>
        /// Gets the timestamp, in Coordinated Universal Time (UTC), when the associated error occurred.
        /// </summary>
        public DateTimeOffset UtcTimestamp => Error.UtcTimestamp;

        public int ProcessId => Error.ProcessId;
        public int ThreadId => Error.ThreadId;

        public InvariantViolationException(
            string message,
            DomainError error)
            : base(message)
        {
            Error = error;
        }

        public InvariantViolationException(
            string code,
            string message,
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
            : this(message, DomainError.Create(code, message, member, file, line))
        { }

        public InvariantViolationException(
            DomainError error,
            Exception? innerException)
            : base(error.ToString(), innerException)
        {
            Error = error;
        }

        private string DebuggerDisplay =>
            $"{Code}: {Message} @ {Member}:{Line} [{System.IO.Path.GetFileName(File)}] (UTC: {UtcTimestamp:O}, P:{ProcessId}, T:{ThreadId})";

        public override string ToString()
            => $"InvariantViolationException: {Error}{Environment.NewLine}{base.ToString()}";
    }
}
