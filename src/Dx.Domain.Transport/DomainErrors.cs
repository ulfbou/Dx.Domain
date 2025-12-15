namespace Dx.Domain
{
    

    /// <summary>
    /// Factory for creating strongly-typed, context-rich domain errors for all Result-related failures.
    /// </summary>
    public sealed partial class DomainErrors
    {
        public static partial class Result
        {
            /// <summary>
            /// Error for when a null DomainError object was supplied where a domain error was required.
            /// Intended for internal invariant/protection use.
            /// </summary>
            public static DomainError NullErrorNotAllowed(
                [CallerMemberName] string member = "",
                [CallerFilePath] string file = "",
                [CallerLineNumber] int line = 0
            ) =>
                DomainError.Create(
                    code: "Result.NullErrorNotAllowed",
                    message: "A null DomainError is not allowed. All Result failures must provide an explicit error object.",
                    member, file, line);

            /// <summary>
            /// Error for when a null value was supplied where a non-null was required.
            /// </summary>
            public static DomainError NullValueNotAllowed(
                [CallerMemberName] string member = "",
                [CallerFilePath] string file = "",
                [CallerLineNumber] int line = 0
            ) =>
                DomainError.Create(
                    code: "Result.NullValueNotAllowed",
                    message: "A null value is not allowed for a successful Result. All successes must provide a concrete value.",
                    member, file, line);
        }

        public static partial class Aggregate
        {

        }

        public static partial class Validation
        {
            public static DomainError MissingRequiredField(
                string fieldName,
                [CallerMemberName] string member = "",
                [CallerFilePath] string file = "",
                [CallerLineNumber] int line = 0)
                => DomainError.Create(
                    code: "Validation.MissingRequiredField",
                    message: $"Required field '{fieldName}' is missing.",
                    member, file, line);
        }
    }
}
