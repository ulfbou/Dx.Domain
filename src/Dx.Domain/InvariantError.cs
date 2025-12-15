namespace Dx.Domain
{
    using System.Diagnostics;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class InvariantError
    {
        public DomainError DomainError { get; }
        public string? MessageOverride { get; }
        public string Member { get; }
        public string FileName { get; }
        public int Line { get; }
        public CorrelationId CorrelationId { get; }
        public TraceId TraceId { get; }
        public SpanId SpanId { get; }
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

        public string EffectiveMessage => MessageOverride ?? DomainError.Message;

        public override string ToString()
            => $"{DomainError.Code}: {EffectiveMessage} @ {Member}:{Line}";

        private string DebuggerDisplay => $"{DomainError.Code} @ {Member}:{Line}";
    }
}
