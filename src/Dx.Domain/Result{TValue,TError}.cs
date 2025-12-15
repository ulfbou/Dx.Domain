namespace Dx.Domain.Results
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents the immutable result of an operation that can succeed with a value of type <typeparamref name="TValue"/> or fail
    /// with an error of type <typeparamref name="TError"/>.
    /// </summary>
    /// <remarks>
    /// This is the canonical functional result type, a discriminated union that explicitly models success and failure paths.
    /// Use <see cref="Result{TValue}"/> for the common case where <typeparamref name="TError"/> is fixed to <see cref="DomainError"/>.
    /// </remarks>
    /// <typeparam name="TValue">The type of the value returned when the operation succeeds.</typeparam>
    /// <typeparam name="TError">The type of the error returned when the operation fails.</typeparam>
    public readonly struct Result<TValue, TError> where TValue : notnull where TError : notnull
    {
        private readonly TValue? _value;
        private readonly TError? _error;

        /// <summary>
        /// Gets a value indicating whether the operation completed successfully.
        /// </summary>
        public bool IsSuccess => _error is null;

        /// <summary>
        /// Gets a value indicating whether the result represents a failure state.
        /// </summary>
        public bool IsFailure => _error is not null;

        /// <summary>
        /// Gets the value contained in the current instance, if one is present.
        /// </summary>
        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsSuccess ? _value! : throw new InvalidOperationException(DomainErrors.Result.MissingValue(_error?.ToString()));
        }

        /// <summary>
        /// Gets the error value associated with the result, if any.
        /// </summary>
        public TError Error
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsFailure ? _error! : throw new InvalidOperationException(DomainErrors.Result.MissingError(_value?.ToString()));
        }

        /// <summary>
        /// Initializes a new instance of the Result class with the specified value.
        /// </summary>
        /// <param name="value">The value to be encapsulated by the Result instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Result(TValue value) => _value = value;

        /// <summary>
        /// Initializes a new instance of the Result class that represents a failed result with the specified error.
        /// </summary>
        /// <param name="error">The error value associated with the failed result. Cannot be null if TError is a reference type.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Result(TError error) => _error = error;

        /// <summary>
        /// Creates a successful result containing the specified value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
        public static Result<TValue, TError> Ok(TValue value) => new Result<TValue, TError>(value);

        /// <summary>
        /// Creates a failed result containing the specified error value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
        public static Result<TValue, TError> Failure(TError error) => new Result<TValue, TError>(error);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => IsSuccess ? $"Ok({_value})" : $"Failure({_error})";

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
        public void Deconstruct(out bool isSuccess, out TValue? value, out TError? error)
        {
            isSuccess = IsSuccess;
            value = _value;
            error = _error;
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
        public void Deconstruct(out bool isFailure, out TError? error, out TValue? value)
        {
            isFailure = IsFailure;
            error = _error;
            value = _value;
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
            value = _value;
        }

        /// <summary>
        /// Deconstructs the result into its error component, if the result represents a failure.
        /// </summary>
        /// <param name="error">When this method returns, contains the associated error if the result is a failure; otherwise, <see
        /// langword="null"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out TError? error)
        {
            error = _error;
        }
    }
}
