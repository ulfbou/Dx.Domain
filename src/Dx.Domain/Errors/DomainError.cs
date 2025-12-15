namespace Dx.Domain
{
    using System.Diagnostics;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly record struct DomainError : IEquatable<DomainError>
    {
        public string Code { get; }
        public string Message { get; }

        private DomainError(string code, string message)
        {
            Code = code;
            Message = message;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DomainError Create(string code, string message)
            => new DomainError(code, message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DomainError? other)
            => other is not null && Code == other?.Code;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Code.GetHashCode(StringComparison.Ordinal);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => $"{Code}: {Message}";

        private string DebuggerDisplay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Code;
        }
    }
}
