namespace Dx.Domain
{
    /// <summary>
    /// Factory for creating strongly-typed, context-rich domain errors for all Result-related failures.
    /// </summary>
    internal sealed partial class DomainErrors
    {
        public static partial class Causation
        {
            public static DomainError MissingCorrelation
                => DomainError.Create(
                    "Causation.MissingCorrelation",
                    "Causation requires a non-empty CorrelationId.");

            public static DomainError MissingTrace
                => DomainError.Create(
                    "Causation.MissingTrace",
                    "Causation requires a non-empty TraceId.");
        }

        public static partial class Fact
        {
            public static DomainError MissingType
                => DomainError.Create(
                    "Fact.MissingType",
                    "Domain facts must declare a fact type.");
        }

        public static partial class Code
        {
            public const string NullOrWhitespace = "Code cannot be null or whitespace.";
        }

        public static partial class Message
        {
            public const string NullOrWhitespace = "Message cannot be null or whitespace.";
        }

        public static partial class Transition
        {
            public const string MissingError = "Transition failure must provide a non-null DomainError.";
            public static DomainError MissingFacts
                => DomainError.Create(
                    "Transition.MissingFacts",
                    "Successful transitions must produce at least one fact.");
        }

        public static partial class Result
        {
            /// <summary>
            /// Error for when a null DomainError object was supplied where a domain error was required.
            /// Intended for internal invariant/protection use.
            /// </summary>
            public static DomainError NullErrorNotAllowed
                => DomainError.Create(
                    "Result.NullErrorNotAllowed",
                    "A null DomainError is not allowed. All Result failures must provide an explicit error object.");

            /// <summary>
            /// Error for when a null value was supplied where a non-null was required.
            /// </summary>
            public static DomainError NullValueNotAllowed
                => DomainError.Create(
                    "Result.NullValueNotAllowed",
                    "A null value is not allowed for a successful Result. All successes must provide a concrete value.");

            public static string MissingValue(string? error) => $"Value is not present in a failure result. Error: {error}";
            public static string MissingError(string? value) => $"Error is not present in a successful result. Value: {value}";
        }

        public static partial class Validation
        {
            public static DomainError MissingRequiredField(string fieldName)
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    throw new ArgumentException("Field name must be provided.", nameof(fieldName));
                }

                return DomainError.Create(
                    code: "Validation.MissingRequiredField",
                    message: $"Required field '{fieldName}' is missing.");
            }
        }
    }
}
