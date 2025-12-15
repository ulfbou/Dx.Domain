namespace Dx.Domain.Invariants
{
    public static class Invariant
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void That(
            bool condition,
            DomainError error,
            string? message = null,
            CorrelationId correlationId = default,
            TraceId traceId = default,
            SpanId spanId = default,
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            if (condition)
                return;

            var diagnostic = InvariantError.Create(
                error,
                message,
                correlationId,
                traceId,
                spanId,
                member,
                file,
                line);

            throw new InvariantViolationException(diagnostic);
        }
    }
}
