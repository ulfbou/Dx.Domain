// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ResultExtensions.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Errors;

using System;

namespace Dx.Domain.Kernel
{
    // DPI: this class is internal to avoid polluting the public API surface with static factory methods.
    internal static class Result
    {
        /// <summary>
        /// Creates a successful Result with no value.
        /// </summary>
        /// <returns>A successful Result.</returns>
        public static Result<Unit> Success()
            => Result<Unit>.Success();

        public static Result<Unit, TError> Success<TError>()
            where TError : notnull
            => Result<Unit, TError>.Success(Unit.Value);

        /// <summary>
        /// Creates a successful Result with the specified value.
        /// </summary>
        /// <typeparam name="TOut">The type of the value.</typeparam>
        /// <param name="value">The value to wrap in a successful Result.</param>
        /// <returns>A successful Result containing the specified value.</returns>
        public static Result<TOut> Success<TOut>(TOut value) where TOut : notnull
            => Result<TOut>.Success(value);

        /// <summary>
        /// Creates a successful Result with the specified value.
        /// </summary>
        /// <typeparam name="TOut">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="value">The value to wrap in a successful Result.</param>
        /// <returns>A successful Result containing the specified value.</returns>
        public static Result<TOut, TError> Success<TOut, TError>(TOut value)
            where TOut : notnull
            where TError : notnull
            => Result<TOut, TError>.Success(value);

        /// <summary>
        /// Creates a failed Result with the specified <see cref="DomainError"/>.
        /// </summary>
        /// <param name="error">The <see cref="DomainError"/> representing the failure.</param>
        /// <returns>A failed Result containing the specified <see cref="DomainError"/>.</returns>
        public static Result<Unit> Failure(DomainError error)
            => Result<Unit>.Failure(error);

        /// <summary>
        /// Creates a failed Result with the specified error.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="error">The error representing the failure.</param>
        /// <returns>A failed Result containing the specified error.</returns>
        public static Result<Unit, TError> Failure<TError>(TError error)
            where TError : notnull
            => Result<Unit, TError>.Failure(error);

        /// <summary>
        /// Creates a failed Result with the specified <see cref="DomainError"/>.
        /// </summary>
        /// <typeparam name="TOut">The type of the value.</typeparam>
        /// <param name="error">The <see cref="DomainError"/> representing the failure.</param>
        /// <returns>A failed Result containing the specified <see cref="DomainError"/>.</returns>
        public static Result<TOut> Failure<TOut>(DomainError error) where TOut : notnull
            => Result<TOut>.Failure(error);

        /// <summary>
        /// Creates a failed Result with the specified error.
        /// </summary>
        /// <typeparam name="TOut">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="error">The error representing the failure.</param>
        /// <returns>A failed Result containing the specified error.</returns>
        public static Result<TOut, TError> Failure<TOut, TError>(TError error)
            where TOut : notnull
            where TError : notnull
            => Result<TOut, TError>.Failure(error);
    }
}
