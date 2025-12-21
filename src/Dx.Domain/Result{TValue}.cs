// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Result.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Diagnostics;

using static Dx.Dx;

namespace Dx.Domain
{
    /// <summary>
    /// Represents the common result of an operation that can succeed with a value of type <typeparamref name="TValue"/> or fail
    /// with a standard <see cref="DomainError"/>.
    /// </summary>
    /// <remarks>
    /// This is a thin wrapper around <see cref="Result{TValue, TError}"/> where the error type is fixed to <see cref="DomainError"/>.
    /// It is the canonical result type for domain operations that use the shared <see cref="DomainError"/> model.
    /// </remarks>
    /// <typeparam name="TValue">The type of the value returned when the operation succeeds.</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct Result<TValue> where TValue : notnull
    {
        private readonly Result<TValue, DomainError> _inner;

        /// <summary>
        /// Gets a value indicating whether the operation completed successfully.
        /// </summary>
        public bool IsSuccess => _inner.IsSuccess;

        /// <summary>
        /// Gets a value indicating whether the result represents a failure state.
        /// </summary>
        public bool IsFailure => _inner.IsFailure;

        /// <summary>
        /// Gets the value contained in the current instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the result represents a failure.</exception>
        public TValue Value => _inner.Value;

        /// <summary>
        /// Gets the error value associated with the result, if any.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the result represents a success.</exception>
        internal DomainError Error => _inner.Error;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TValue}"/> struct that wraps an existing
        /// <see cref="Result{TValue, TError}"/> with <see cref="DomainError"/> as the error type.
        /// </summary>
        /// <param name="inner">The inner result value that encapsulates either a successful value or a domain error.</param>
        private Result(Result<TValue, DomainError> inner) => _inner = inner;

        // ------------------------------------------------------------
        // PUBLIC FACTORIES REMOVED
        // Consumers must use Dx.Result.Ok(...)
        // ------------------------------------------------------------

        /// <summary>
        /// Creates a successful result containing the specified value.
        /// </summary>
        /// <param name="value">The value to be wrapped in a successful result.</param>
        /// <returns>A <see cref="Result{TValue}"/> representing a successful operation with the provided value.</returns>
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Static factories on the generic result type are an intentional part of the API.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Result<TValue> InternalOk(TValue value) => new(Result<TValue, DomainError>.InternalOk(value));

        /// <summary>
        /// Creates a failed result with the specified domain error.
        /// </summary>
        /// <param name="error">The domain error that describes the reason for the failure. Cannot be null.</param>
        /// <returns>A result representing a failure, containing the specified domain error.</returns>
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Static factories on the generic result type are an intentional part of the API.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Result<TValue> InternalFailure(DomainError error) => new(Result<TValue, DomainError>.InternalFailure(error));

        /// <summary>
        /// Converts a successful <see cref="Result{TValue}"/> to its underlying value of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <remarks>This operator throws an exception if the result represents a failure. Use only when it is guaranteed
        /// that the result is successful.</remarks>
        /// <param name="result">The <see cref="Result{TValue}"/> instance to convert. Must represent a successful result.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator TValue(Result<TValue> result)
        {

            Invariant.That(result.IsSuccess, () => Dx.Faults.Result.MissingValueOnFailure<TValue, DomainError>(result.Error));

            return result.Value;
        }

        /// <inheritdoc/>
        public override string ToString() => _inner.ToString();

        /// <inheritdoc/>
        public bool Equals(Result<TValue> other) => _inner.Equals(other._inner);

        /// <inheritdoc/>
        public override int GetHashCode() => _inner.GetHashCode();

        /// <summary>
        /// Deconstructs the result into its success status, value, and error information.
        /// </summary>
        /// <remarks>
        /// This method enables deconstruction of the result into separate variables for pattern matching or tuple assignment.
        /// </remarks>
        /// <param name="isSuccess">When this method returns, contains <see langword="true"/> if the result represents a success; otherwise,
        /// <see langword="false"/>.</param>
        /// <param name="value">When this method returns, contains the value of the result if it is successful; otherwise, the default value
        /// for the type.</param>
        /// <param name="error">When this method returns, contains the error information if the result represents a failure; otherwise, <see
        /// langword="null"/>.</param>
        public void Deconstruct(out bool isSuccess, out TValue? value, out DomainError? error)
        {
            isSuccess = IsSuccess;
            value = isSuccess ? Value : default;
            error = !isSuccess ? Error : null;
        }

        /// <summary>
        /// Deconstructs the result into its failure status, error, and value components.
        /// </summary>
        /// <remarks>
        /// This method enables deconstruction syntax, allowing the result to be unpacked into separate variables for failure
        /// status, error, and value.
        /// </remarks>
        /// <param name="isFailure">When this method returns, contains <see langword="true"/> if the result represents a failure; otherwise,
        /// <see langword="false"/>.</param>
        /// <param name="error">When this method returns, contains the associated <see cref="DomainError"/> if the result is a failure;
        /// otherwise, <see langword="null"/>.</param>
        /// <param name="value">When this method returns, contains the value if the result is successful; otherwise, <see langword="null"/>.</param>
        public void Deconstruct(out bool isFailure, out DomainError? error, out TValue? value)
        {
            isFailure = IsFailure;
            error = !isFailure ? Error : default;
            value = isFailure ? Value : default;
        }

        /// <summary>
        /// Deconstructs the result into its value component.
        /// </summary>
        /// <remarks>This method enables deconstruction syntax, allowing the result to be unpacked into
        /// its value component using tuple deconstruction in assignment statements.</remarks>
        /// <param name="value">When this method returns, contains the value if the result represents success; otherwise, the default value
        /// for the type.</param>
        public void Deconstruct(out TValue? value)
        {
            value = IsSuccess ? Value : default;
        }

        /// <summary>
        /// Deconstructs the result into its error component, if the result represents a failure.
        /// </summary>
        /// <param name="error">When this method returns, contains the associated error if the result is a failure; otherwise, <see
        /// langword="null"/>.</param>
        public void Deconstruct(out DomainError? error)
        {
            error = IsFailure ? Error : default;
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Result<{typeof(TValue).Name}> IsSuccess = {IsSuccess}, HasError = {IsFailure}";
    }
}
