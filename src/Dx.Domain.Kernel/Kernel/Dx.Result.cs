// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dx.Result.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;

using Dx.Domain.Errors;

namespace Dx.Domain
{
    public static partial class Dx
    {
        /// <summary>
        /// Provides canonical factory methods for constructing <see cref="Result"/> values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This class defines the <strong>single authoritative construction surface</strong>
        /// for all <see cref="Result"/> instances.
        /// </para>
        /// <para>
        /// Direct instantiation, alternative factories, or ad‑hoc construction
        /// mechanisms are intentionally prohibited and analyzer‑enforced.
        /// </para>
        /// <para>
        /// All methods on this class are pure, side‑effect free, and deterministic.
        /// They perform no logging, IO, or policy evaluation.
        /// </para>
        /// </remarks>
        public static class Result
        {
            /// <summary>
            /// Creates a successful Result with no value.
            /// </summary>
            /// <returns>
            /// A successful <see cref="Result{Unit}"/>.
            /// </returns>
            public static Result<Unit> Success()
                => Result<Unit>.Success();

            /// <summary>
            /// Creates a successful Result containing the specified value.
            /// </summary>
            /// <typeparam name="TValue">
            /// The type of the success value.
            /// </typeparam>
            /// <param name="value">
            /// The value to wrap in a successful Result.
            /// </param>
            /// <returns>
            /// A successful <see cref="Result{TValue}"/> containing <paramref name="value"/>.
            /// </returns>
            public static Result<TValue> Success<TValue>(TValue value) where TValue : notnull
                => Result<TValue>.Success(value);

            /// <summary>
            /// Creates a successful result containing the specified value.
            /// </summary>
            /// <typeparam name="TValue">The type of the success value.</typeparam>
            /// <typeparam name="TError">The type of the error value.</typeparam>
            /// <param name="value">The success value to wrap in the result.</param>
            /// <returns>A successful result containing the specified value.</returns>
            public static Result<TValue, TError> Success<TValue, TError>(TValue value)
                where TValue : notnull
                where TError : notnull
                => Result<TValue, TError>.Success(value);

            /// <summary>
            /// Creates a failed Result using an eagerly supplied domain error.
            /// </summary>
            /// <typeparam name="TValue">
            /// The type of the success value.
            /// </typeparam>
            /// <param name="error">
            /// The domain error describing the failure.
            /// </param>
            /// <returns>
            /// A failed <see cref="Result{TValue}"/> containing <paramref name="error"/>.
            /// </returns>
            public static Result<TValue> Failure<TValue>(DomainError error) where TValue : notnull
                => Result<TValue>.Failure(error);

            /// <summary>
            /// Creates a failed Result using lazy domain error construction.
            /// </summary>
            /// <typeparam name="TValue">
            /// The type of the success value.
            /// </typeparam>
            /// <param name="errorFactory">
            /// A factory that lazily produces the domain error.
            /// The factory is evaluated only on failure.
            /// </param>
            /// <returns>
            /// A failed <see cref="Result{TValue}"/> containing the produced error.
            /// </returns>
            public static Result<TValue> Failure<TValue>(Func<DomainError> errorFactory) where TValue : notnull
                => Result<TValue>.Failure(errorFactory());

            /// <summary>
            /// Creates a failed Result with a strongly typed error value.
            /// </summary>
            /// <typeparam name="TValue">
            /// The type of the success value.
            /// </typeparam>
            /// <typeparam name="TError">
            /// The type of the error value.
            /// </typeparam>
            /// <param name="error">
            /// The error value describing the failure.
            /// </param>
            /// <returns>
            /// A failed <see cref="Result{TValue, TError}"/> containing <paramref name="error"/>.
            /// </returns>
            public static Result<TValue, TError> Failure<TValue, TError>(TError error)
                where TValue : notnull
                where TError : notnull
                => Result<TValue, TError>.Failure(error);

            /// <summary>
            /// Creates a failed Result with a lazily produced error value.
            /// </summary>
            /// <typeparam name="TValue">
            /// The type of the success value.
            /// </typeparam>
            /// <typeparam name="TError">
            /// The type of the error value.
            /// </typeparam>
            /// <param name="errorFactory">
            /// A factory that lazily produces the error value.
            /// </param>
            /// <returns>
            /// A failed <see cref="Result{TValue, TError}"/> containing the produced error.
            /// </returns>
            public static Result<TValue, TError> Failure<TValue, TError>(
                Func<TError> errorFactory)
                where TValue : notnull
                where TError : notnull
                => Result<TValue, TError>.Failure(errorFactory());
        }
    }
}
