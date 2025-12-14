namespace Dx.Domain.Errors
{
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Represents a domain-specific error with contextual information such as error code, message, caller details,
    /// and execution context.
    /// </summary>
    /// <remarks>Use this type to capture and convey errors that occur within a domain or business
    /// logic layer, along with relevant diagnostic details. The structure includes information about where and when
    /// the error was created, which can assist in debugging and error tracking. The static Create method
    /// automatically populates caller information and is recommended for typical usage.</remarks>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly record struct DomainError
    {
        public string Code { get; }
        public string Message { get; }
        public string Member { get; }
        public string File { get; }
        public int Line { get; }
        public DateTimeOffset UtcTimestamp { get; }
        public int ProcessId { get; }
        public int ThreadId { get; }

        private DomainError(
            string code,
            string message,
            string member = "",
            string file = "",
            int line = 0,
            DateTimeOffset? utcTimestamp = null,
            int? processId = null,
            int? threadId = null)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Member = member;
            File = file;
            Line = line;
            UtcTimestamp = utcTimestamp ?? DateTimeOffset.UtcNow;
            ProcessId = processId ?? Environment.ProcessId;
            ThreadId = threadId ?? Environment.CurrentManagedThreadId;
        }


        /// <summary>
        /// Creates a new instance of the DomainError class with the specified error code, message, and optional
        /// caller information.
        /// </summary>
        /// <remarks>The optional parameters 'member', 'file', and 'line' are typically populated
        /// by the compiler using Caller Info attributes. Supplying these values manually is not required in normal
        /// usage.</remarks>
        /// <param name="code">The unique code that identifies the type or category of the domain error. Cannot be null or empty.</param>
        /// <param name="message">A descriptive message that explains the error. Cannot be null or empty.</param>
        /// <param name="member">The name of the member where the error occurred. This value is automatically provided by the compiler
        /// and should not be set explicitly in most cases.</param>
        /// <param name="file">The full path of the source file that contains the member where the error occurred. This value is
        /// automatically provided by the compiler.</param>
        /// <param name="line">The line number in the source file at which the error was created. This value is automatically provided
        /// by the compiler.</param>
        /// <returns>A new DomainError instance containing the specified error code, message, and caller information.</returns>
        public static DomainError Create(
            string code,
            string message,
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
            => new(code, message, member, file, line);

        private string DebuggerDisplay =>
            $"{Code}: {Message} @ {Member}:{Line} [{System.IO.Path.GetFileName(File)}] (UTC: {UtcTimestamp:O}, P:{ProcessId}, T:{ThreadId})";

        public override string ToString() =>
            $"{Code}: {Message} (at {Member}:{Line} in {File}) [UTC: {UtcTimestamp:O}, P:{ProcessId}, T:{ThreadId}]";
    }
}
