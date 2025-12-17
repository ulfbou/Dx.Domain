// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Result{TValue,TError}.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain
{
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using static global::Dx.Dx;

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
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
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
            [DebuggerStepThrough]
            get
            {
                Invariant.That(IsSuccess, Dx.Faults.Result.MissingValueOnFailure<TValue, TError>(_error!));
                return _value!;
            }
        }

        /// <summary>
        /// Gets the error value associated with the result, if any.
        /// </summary>
        public TError Error
        {
            [DebuggerStepThrough]
            get
            {
                Invariant.That(IsFailure, Dx.Faults.Result.MissingErrorOnSuccess<TValue, TError>(_value!));
                return _error!;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Result class with the specified value.
        /// </summary>
        /// <param name="value">The value to be encapsulated by the Result instance.</param>
        private Result(TValue value) => _value = value;

        /// <summary>
        /// Initializes a new instance of the Result class that represents a failed result with the specified error.
        /// </summary>
        /// <param name="error">The error value associated with the failed result. Cannot be null if TError is a reference type.</param>
        private Result(TError error) => _error = error;

        /// <summary>
        /// Creates a successful result containing the specified value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
        internal static Result<TValue, TError> InternalOk(TValue value) => new Result<TValue, TError>(value);

        /// <summary>
        /// Creates a failed result containing the specified error value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
        internal static Result<TValue, TError> InternalFailure(TError error) => new Result<TValue, TError>(error);

        /// <inheritdoc />
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
        public void Deconstruct(out TValue? value)
        {
            value = _value;
        }

        /// <summary>
        /// Deconstructs the result into its error component, if the result represents a failure.
        /// </summary>
        /// <param name="error">When this method returns, contains the associated error if the result is a failure; otherwise, <see
        /// langword="null"/>.</param>
        public void Deconstruct(out TError? error)
        {
            error = _error;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => IsSuccess ? $"Ok({_value})" : $"Failure({_error})";
    }
}
