namespace Dx.Domain.Errors
{
    using System.Diagnostics;

    /// <summary>
    /// Canonical domain error value for the Dx platform.
    /// <para>
    /// Represents a semantic failure that may safely cross boundaries (Results, APIs, persistence, tests).
    /// <br/>
    /// <b>DomainError</b> is intentionally lean:
    /// <list type="bullet">
    /// <item>No caller info</item>
    /// <item>No stack context</item>
    /// <item>No transport concerns</item>
    /// <item>No diagnostics</item>
    /// </list>
    /// Diagnostics and stack context belong to invariant exceptions, not to this type.
    /// </para>
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly record struct DomainError
    {
        /// <summary>
        /// The unique code that identifies the type or category of the domain error.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// A descriptive message that explains the error.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The name of the member where the error occurred. Typically set by the compiler.
        /// </summary>
        public string Member { get; }

        /// <summary>
        /// The full path of the source file that contains the member where the error occurred. Typically set by the compiler.
        /// </summary>
        public string File { get; }

        /// <summary>
        /// The line number in the source file at which the error was created. Typically set by the compiler.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// The UTC timestamp when the error was created.
        /// </summary>
        public DateTimeOffset UtcTimestamp { get; }

        /// <summary>
        /// The process ID at the time the error was created.
        /// </summary>
        public int ProcessId { get; }

        /// <summary>
        /// The thread ID at the time the error was created.
        /// </summary>
        public int ThreadId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainError"/> struct.
        /// </summary>
        /// <param name="code">The unique code that identifies the type or category of the domain error.</param>
        /// <param name="message">A descriptive message that explains the error.</param>
        /// <param name="member">The name of the member where the error occurred.</param>
        /// <param name="file">The full path of the source file that contains the member where the error occurred.</param>
        /// <param name="line">The line number in the source file at which the error was created.</param>
        /// <param name="utcTimestamp">The UTC timestamp when the error was created.</param>
        /// <param name="processId">The process ID at the time the error was created.</param>
        /// <param name="threadId">The thread ID at the time the error was created.</param>
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
        /// Creates a new <see cref="DomainError"/> instance with the specified error code, message, and optional caller information.
        /// <para>
        /// Caller info parameters are typically populated by the compiler. Supplying these values manually is not required in normal usage.
        /// </para>
        /// </summary>
        /// <param name="code">The unique code that identifies the type or category of the domain error. Cannot be null or empty.</param>
        /// <param name="message">A descriptive message that explains the error. Cannot be null or empty.</param>
        /// <param name="member">The name of the member where the error occurred. Typically set by the compiler.</param>
        /// <param name="file">The full path of the source file that contains the member where the error occurred. Typically set by the compiler.</param>
        /// <param name="line">The line number in the source file at which the error was created. Typically set by the compiler.</param>
        /// <returns>A new <see cref="DomainError"/> instance containing the specified error code, message, and caller information.</returns>
        public static DomainError Create(
            string code,
            string message,
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
            => new(code, message, member, file, line);

        private string DebuggerDisplay =>
            $"{Code}: {Message} @ {Member}:{Line} [{System.IO.Path.GetFileName(File)}] (UTC: {UtcTimestamp:O}, P:{ProcessId}, T:{ThreadId})";

        /// <summary>
        /// Returns a string representation of the domain error, including code, message, location, and context.
        /// </summary>
        /// <returns>A string describing the error.</returns>
        public override string ToString() =>
            $"{Code}: {Message} (at {Member}:{Line} in {File}) [UTC: {UtcTimestamp:O}, P:{ProcessId}, T:{ThreadId}]";
    }
}
