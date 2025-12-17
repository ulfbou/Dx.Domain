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

namespace Dx
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides the primary gateway for the Dx Domain Kernel, offering a unified facade for result creation 
    /// while encapsulating internal mechanical enforcement and validation utilities.
    /// </summary>
    /// <remarks>
    /// This class provides the ONLY public entry points for creating Results,
    /// while hiding the mechanical enforcement tools (Invariant, Require) internally.
    /// </remarks>
    public static partial class Dx
    {
        // ---------------------------------------------------------
        // PUBLIC FACADE (The "Front Office")
        // ---------------------------------------------------------

        /// <summary>
        /// Provides static factory methods for creating and converting result objects that represent success or failure
        /// outcomes in domain operations.
        /// </summary>
        /// <remarks>The Result class is the only way to construct and transform result types, supporting both 
        /// value-carrying and error-carrying results. These methods simplify the creation of success and
        /// failure results, including those with custom error types or unit values. This class is typically used to
        /// encapsulate the outcome of operations, enabling clear handling of success and error cases without relying on
        /// exceptions.</remarks>
        public static partial class Result
        {
            /// <summary>
            /// Creates a successful result containing the specified value.
            /// </summary>
            /// <typeparam name="TValue">The type of the value to be stored in the result.</typeparam>
            /// <param name="value">The value to include in the successful result. Can be <see langword="null"/> for reference types.</param>
            /// <returns>A <see cref="Result{TValue}"/> representing a successful operation with the provided value.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<TValue> Ok<TValue>(TValue value) where TValue : notnull
                => Result<TValue>.InternalOk(value);

            /// <summary>
            /// Creates a failed result containing the specified domain error.
            /// </summary>
            /// <remarks>Use this method to represent an operation that did not succeed due to a
            /// domain-specific error. The returned result will not contain a value.</remarks>
            /// <typeparam name="TValue">The type of the value that would be held by a successful result.</typeparam>
            /// <param name="error">The domain error to associate with the failed result. Cannot be <see langword="null"/>.</param>
            /// <returns>A failed <see cref="Result{TValue}"/> instance containing the provided domain error.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<TValue> Failure<TValue>(DomainError error) where TValue : notnull
                => Result<TValue>.InternalFailure(error);

            /// <summary>
            /// Creates a failed result with the specified error code and message.
            /// </summary>
            /// <typeparam name="TValue">The type of the value that would have been returned on success.</typeparam>
            /// <param name="code">A string that uniquely identifies the error condition. Cannot be null or empty.</param>
            /// <param name="message">A descriptive message explaining the reason for the failure. Cannot be null.</param>
            /// <returns>A failed result containing the specified error information.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<TValue> Failure<TValue>(string code, string message) where TValue : notnull
                => Result<TValue>.InternalFailure(DomainError.Create(code, message));

            /// <summary>
            /// Creates a successful result containing the specified value.
            /// </summary>
            /// <typeparam name="TValue">The type of the value to be stored in the successful result.</typeparam>
            /// <typeparam name="TError">The type used to represent an error in the result.</typeparam>
            /// <param name="value">The value to be wrapped in a successful result. Can be <see langword="null"/> for reference types.</param>
            /// <returns>A Result object representing a successful outcome that contains the specified value.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<TValue, TError> Ok<TValue, TError>(TValue value) where TValue : notnull where TError : notnull
                => Result<TValue, TError>.InternalOk(value);

            /// <summary>
            /// Creates a failed result containing the specified error value.
            /// </summary>
            /// <remarks>Use this method to construct a result that represents a failure, typically in
            /// scenarios where an operation cannot complete successfully and an error must be reported.</remarks>
            /// <typeparam name="TValue">The type of the value that would be returned on success.</typeparam>
            /// <typeparam name="TError">The type of the error value to be returned on failure.</typeparam>
            /// <param name="error">The error value to associate with the failed result. Cannot be <see langword="null"/> if the error type does not allow null
            /// values.</param>
            /// <returns>A <see cref="Result{TValue, TError}"/> representing a failed operation with the provided error value.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<TValue, TError> Failure<TValue, TError>(TError error) where TValue : notnull where TError : notnull
                => Result<TValue, TError>.InternalFailure(error);

            /// <summary>
            /// Creates a successful result with no associated value.
            /// </summary>
            /// <returns>A <see cref="Result{Unit}"/> representing a successful operation with no value.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<Unit> Ok() => Result<Unit>.InternalOk(Unit.Value);

            /// <summary>
            /// Creates a failed result of type <see cref="Unit"/> with the specified domain error.
            /// </summary>
            /// <param name="error">The <see cref="DomainError"/> that describes the reason for the failure. Cannot be null.</param>
            /// <returns>A <see cref="Result{Unit}"/> representing a failed operation with the provided error.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<Unit> Failure(DomainError error) => Result<Unit>.InternalFailure(error);

            /// <summary>
            /// Creates a failed result containing the specified error value.
            /// </summary>
            /// <remarks>This method is typically used to represent an operation that has failed and to
            /// propagate error information. The returned result will not contain a value, only the error.</remarks>
            /// <typeparam name="TError">The type of the error value to be stored in the result.</typeparam>
            /// <param name="error">The error value to associate with the failed result. Cannot be null if the error type does not allow null
            /// values.</param>
            /// <returns>A result representing failure, containing the provided error value.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<Unit, TError> Failure<TError>(TError error) where TError : notnull
                => Result<Unit, TError>.InternalFailure(error);

            /// <summary>
            /// Creates a successful result with no value, indicating that the operation completed successfully.
            /// </summary>
            /// <typeparam name="TError">The type of the error value that may be returned if the result is unsuccessful.</typeparam>
            /// <returns>A successful <see cref="Result{Unit, TError}"/> instance containing no value.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<Unit, TError> Ok<TError>() where TError : notnull
                => Result<Unit, TError>.InternalOk(Unit.Value);

            /// <summary>
            /// Creates a new result instance from an existing <see cref="Result{TValue, TError}"/> object.
            /// </summary>
            /// <typeparam name="TValue">The type of the value contained in the result.</typeparam>
            /// <typeparam name="TError">The type of the error contained in the result.</typeparam>
            /// <param name="result">The result object to be returned.</param>
            /// <returns>The same <see cref="Result{TValue, TError}"/> instance provided in <paramref name="result"/>.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<TValue, TError> From<TValue, TError>(Result<TValue, TError> result) where TValue : notnull where TError : notnull
                => result;

            /// <summary>
            /// Returns the specified result instance without modification.
            /// </summary>
            /// <remarks>This method can be used to improve code readability or to enable method chaining in
            /// generic scenarios. It does not create a new instance or alter the input result.</remarks>
            /// <typeparam name="TValue">The type of the value contained in the result.</typeparam>
            /// <param name="result">The result instance to return. Cannot be <see langword="null"/>.</param>
            /// <returns>The same <see cref="Result{TValue}"/> instance provided in <paramref name="result"/>.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<TValue> From<TValue>(Result<TValue> result) where TValue : notnull
                => result;

            /// <summary>
            /// Returns the specified <see cref="Result{Unit}"/> instance without modification.
            /// </summary>
            /// <param name="result">The <see cref="Result{Unit}"/> instance to return.</param>
            /// <returns>The same <see cref="Result{Unit}"/> instance provided in <paramref name="result"/>.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<Unit> From(Result<Unit> result)
                => result;

            /// <summary>
            /// Returns the specified result without modification.
            /// </summary>
            /// <typeparam name="TError">The type of the error value contained in the result.</typeparam>
            /// <param name="result">The result to return. Represents either a successful outcome with no value, or an error of type
            /// <typeparamref name="TError"/>.</param>
            /// <returns>The same <see cref="Result{Unit, TError}"/> instance provided in <paramref name="result"/>.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<Unit, TError> From<TError>(Result<Unit, TError> result) where TError : notnull
                => result;

            /// <summary>
            /// Creates a new result object by converting a non-error result to a result type that includes an explicit
            /// error value.
            /// </summary>
            /// <remarks>
            /// Use this method to adapt a <see cref="Result{TValue}"/> to a <see cref="Result{TValue, TError}"/> when you need
            /// to work with a result type that explicitly represents both success and error values. The error value is cast
            /// to <typeparamref name="TError"/>; ensure that the runtime type is compatible to avoid invalid cast exceptions.
            /// </remarks>
            /// <typeparam name="TValue">The type of the value contained in the result.</typeparam>
            /// <typeparam name="TError">The type of the error value to associate with a failed result.</typeparam>
            /// <param name="result">The result to convert. Must not be null.</param>
            /// <returns>
            /// A result of type <see cref="Result{TValue, TError}"/> containing the value if successful,
            /// or the error converted to <typeparamref name="TError"/> if failed.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Result<TValue, TError> From<TValue, TError>(Result<TValue> result) where TValue : notnull where TError : notnull
                => result.IsSuccess
                    ? Result<TValue, TError>.InternalOk(result.Value)
                    : Result<TValue, TError>.InternalFailure((TError)(object)result.Error!);
        }

        // ---------------------------------------------------------
        // INTERNAL MECHANICS (The "Back Office")
        // ---------------------------------------------------------

        /// <summary>
        /// Internal mechanical enforcement for the domain kernel.
        /// </summary>
        internal static partial class Invariant { }

        /// <summary>
        /// Internal functional-style precondition checks.
        /// </summary>
        internal static partial class Require { }
    }
}
