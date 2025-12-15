namespace Dx.Domain
{
    using Dx.Domain.Errors;

    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents the common result of an operation that can succeed with a value of type <typeparamref name="TValue"/> or fail
    /// with a standard <see cref="DomainError"/>.
    /// </summary>
    /// <remarks>
    /// This is a thin wrapper around <see cref="Result{TValue, TError}"/> where the error type is fixed to <see cref="DomainError"/>.
    /// </remarks>
    /// <typeparam name="TValue">The type of the value returned when the operation succeeds.</typeparam>
    public readonly struct Result<TValue> where TValue : notnull
    {
        private readonly Result<TValue, DomainError> _inner;

        /// <summary>
        /// Initializes a new instance of the Result class with the specified inner result value.
        /// </summary>
        /// <param name="inner">The inner result value that encapsulates either a successful value or a domain error.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Result(Result<TValue, DomainError> inner) => _inner = inner;

        /// <summary>
        /// Gets a value indicating whether the operation completed successfully.
        /// </summary>
        public bool IsSuccess
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _inner.IsSuccess;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the result represents a failure state.
        /// </summary>
        public bool IsFailure
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _inner.IsFailure;
            }
        }

        /// <summary>
        /// Gets the value contained in the current instance, if one is present.
        /// </summary>
        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _inner.Value;
        }

        /// <summary>
        /// Gets the error value associated with the result, if any.
        /// </summary>
        public DomainError Error
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _inner.Error;
        }

        /// <summary>
        /// Creates a successful result containing the specified value.
        /// </summary>
        /// <param name="value">The value to be wrapped in a successful result.</param>
        /// <returns>A <see cref="Result{TValue}"/> representing a successful operation with the provided value.</returns>
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Static factories on the generic result type are an intentional part of the API.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TValue> Ok(TValue value) => new Result<TValue>(Result<TValue, DomainError>.Ok(value));

        /// <summary>
        /// Creates a failed result with the specified domain error.
        /// </summary>
        /// <param name="error">The domain error that describes the reason for the failure. Cannot be null.</param>
        /// <returns>A result representing a failure, containing the specified domain error.</returns>
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Static factories on the generic result type are an intentional part of the API.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TValue> Failure(DomainError error) => new Result<TValue>(Result<TValue, DomainError>.Failure(error));

        /// <summary>
        /// Converts a value of type <typeparamref name="TValue"/> to a successful <see cref="Result{TValue}"/> instance containing the specified value.
        /// </summary>
        /// <remarks>
        /// This implicit conversion allows a <typeparamref name="TValue"/> to be used wherever a <see cref="Result{TValue}"/> is
        /// expected, simplifying code that returns or assigns results.
        /// </remarks>
        /// <param name="value">The value to wrap in a successful result.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<TValue>(TValue value) => Ok(value);

        /// <summary>
        /// Converts a <see cref="DomainError"/> to a failed <see cref="Result{TValue}"/> instance representing an error
        /// result.
        /// </summary>
        /// <remarks>This implicit conversion allows a <see cref="DomainError"/> to be returned or
        /// assigned where a <see cref="Result{TValue}"/> is expected, simplifying error handling in method
        /// results.</remarks>
        /// <param name="error">The domain error to represent as a failed result. Cannot be null.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<TValue>(DomainError error) => Failure(error);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _inner.ToString();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Result<TValue> other) => _inner.Equals(other._inner);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _inner.GetHashCode();

        /// <summary>
        /// Deconstructs the result into its success status, value, and error information.
        /// </summary>
        /// <remarks>This method enables deconstruction of the result into separate variables for pattern
        /// matching or tuple assignment.</remarks>
        /// <param name="isSuccess">When this method returns, contains <see langword="true"/> if the result represents a success; otherwise,
        /// <see langword="false"/>.</param>
        /// <param name="value">When this method returns, contains the value of the result if it is successful; otherwise, the default value
        /// for the type.</param>
        /// <param name="error">When this method returns, contains the error information if the result represents a failure; otherwise, <see
        /// langword="null"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out bool isSuccess, out TValue? value, out DomainError? error)
        {
            isSuccess = IsSuccess;
            value = isSuccess ? Value : default;
            error = !isSuccess ? Error : null;
        }

        /// <summary>
        /// Deconstructs the result into its failure status, error, and value components.
        /// </summary>
        /// <remarks>This method enables deconstruction syntax, allowing the result to be unpacked into
        /// separate variables for failure status, error, and value.</remarks>
        /// <param name="isFailure">When this method returns, contains <see langword="true"/> if the result represents a failure; otherwise,
        /// <see langword="false"/>.</param>
        /// <param name="error">When this method returns, contains the associated <see cref="DomainError"/> if the result is a failure;
        /// otherwise, <see langword="null"/>.</param>
        /// <param name="value">When this method returns, contains the value if the result is successful; otherwise, <see langword="null"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out bool isFailure, out DomainError? error, out TValue? value)
        {
            isFailure = IsFailure;
            error = !isFailure ? Error : default;
            value = isFailure ? Value : default;
        }

        /// <summary>
        /// Deconstructs the result into its value component.
        /// </summary>
        /// <remarks>This method enables deconstruction syntax, allowing the result to be unpacked into its value
        /// component using tuple deconstruction.</remarks>
        /// <param name="value">When this method returns, contains the value if the operation was successful; otherwise, the default value for the
        /// type.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out TValue? value)
        {
            value = IsSuccess ? Value : default;
        }

        /// <summary>
        /// Deconstructs the result into its error component, if the result represents a failure.
        /// </summary>
        /// <param name="error">When this method returns, contains the associated error if the result is a failure; otherwise, <see
        /// langword="null"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out DomainError? error)
        {
            error = IsFailure ? Error : default;
        }
    }
}
