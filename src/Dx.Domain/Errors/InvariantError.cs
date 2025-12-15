namespace Dx.Domain.Errors
{
    using System.Diagnostics;

    /// <summary>
    /// Diagnostic carrier for invariant violations in the Dx platform.
    /// <para>
    /// <b>InvariantError</b> is <b>NOT</b> a domain value and MUST NOT:
    /// <list type="bullet">
    /// <item>appear in <c>Result</c></item>
    /// <item>cross process boundaries</item>
    /// <item>be persisted</item>
    /// <item>be returned from APIs</item>
    /// </list>
    /// It exists solely to explain <i>why the program is wrong</i> at an invariant failure site.
    /// </para>
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly record struct InvariantError
    {
        /// <summary>The canonical domain meaning of the violation.</summary>
        public DomainError DomainError { get; }

        /// <summary>Optional override message for this specific violation.</summary>
        public string? MessageOverride { get; }

        /// <summary>Member name at the throw site.</summary>
        public string Member { get; }

        /// <summary>Source file at the throw site.</summary>
        public string File { get; }

        /// <summary>Source line at the throw site.</summary>
        public int Line { get; }

        /// <summary>Correlation identifier (semantic).</summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>Trace identifier (opaque).</summary>
        public TraceId TraceId { get; }

        /// <summary>Span identifier (opaque).</summary>
        public SpanId SpanId { get; }

        /// <summary>UTC timestamp captured at creation.</summary>
        public DateTime UtcTimestamp { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvariantError"/> class.
        /// </summary>
        /// <param name="domainError">The canonical domain meaning of the violation.</param>
        /// <param name="messageOverride">Optional override message for this specific violation.</param>
        /// <param name="member">Member name at the throw site.</param>
        /// <param name="file">Source file at the throw site.</param>
        /// <param name="line">Source line at the throw site.</param>
        /// <param name="correlationId">Correlation identifier (semantic).</param>
        /// <param name="traceId">Trace identifier (opaque).</param>
        /// <param name="spanId">Span identifier (opaque).</param>
        private InvariantError(
            DomainError domainError,
            string? messageOverride,
            string member,
            string file,
            int line,
            CorrelationId correlationId,
            TraceId traceId,
            SpanId spanId)
        {
            DomainError = domainError;
            MessageOverride = messageOverride;
            Member = member;
            File = file;
            Line = line;
            CorrelationId = correlationId;
            TraceId = traceId;
            SpanId = spanId;
            UtcTimestamp = DateTime.UtcNow;
        }

        // ------------------------------------
        // Factory (used only by invariants)
        // ------------------------------------

        /// <summary>
        /// Creates an <see cref="InvariantError"/> for diagnostic purposes at invariant failure sites.
        /// <para>
        /// Intended to be called only at invariant failure sites. This type must never be used as a domain value, nor cross boundaries, nor be persisted.
        /// </para>
        /// </summary>
        /// <param name="domainError">The canonical domain meaning of the violation.</param>
        /// <param name="messageOverride">Optional override message for this specific violation.</param>
        /// <param name="correlationId">Correlation identifier (semantic).</param>
        /// <param name="traceId">Trace identifier (opaque).</param>
        /// <param name="spanId">Span identifier (opaque).</param>
        /// <param name="member">Member name at the throw site.</param>
        /// <param name="file">Source file at the throw site.</param>
        /// <param name="line">Source line at the throw site.</param>
        /// <returns>A new <see cref="InvariantError"/> instance for diagnostics.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InvariantError Create(
            DomainError domainError,
            string? messageOverride = null,
            CorrelationId correlationId = default,
            TraceId traceId = default,
            SpanId spanId = default,
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            return new InvariantError(
                domainError,
                messageOverride,
                member,
                file,
                line,
                correlationId,
                traceId,
                spanId);
        }

        /// <summary>Effective message for diagnostics and exceptions.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string EffectiveMessage
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MessageOverride ?? DomainError.Message;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => $"{DomainError.Code}: {EffectiveMessage} @ {Member}:{Line}";

        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => $"{DomainError.Code} @ {Member}:{Line}";
        }
    }
}
