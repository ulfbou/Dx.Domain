// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Dx.Require.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

// ============================================================================
// Dx.Domain — Canonical Domain Facade
//
// Validation surface for recoverable domain conditions.
//
// Responsibilities:
//   - validate domain preconditions
//   - express failure as Result values
//   - participate in Result-based control flow
//
// Guarantees:
//   - no method on this surface throws due to validation failure
//   - no kernel internals are exposed
//   - no policy, logging, or IO behavior is encoded
//
// ============================================================================

using System;

using Dx.Domain.Errors;

namespace Dx.Domain
{
    public static partial class Dx
    {
        /// <summary>
        /// Provides validation‑style invariant checks that return failed Results
        /// instead of throwing exceptions.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <c>Require</c> represents <em>recoverable domain validation</em>.
        /// A failed requirement indicates that an operation cannot proceed,
        /// but does not represent a corrupted program state or programming error.
        /// </para>
        /// <para>
        /// In contrast to <see cref="Invariant"/>, methods on this class
        /// <strong>never throw</strong> as a result of validation failure.
        /// All outcomes are expressed as <see cref="Result"/> values and are therefore
        /// composable, inspectable, and analyzable.
        /// </para>
        /// <para>
        /// This class is the primary validation surface intended for third‑party,
        /// application‑level, and flow‑oriented domain logic.
        /// </para>
        /// </remarks>
        /// <seealso cref="Invariant"/>
        /// <seealso cref="Result"/>
        public static class Require
        {
            // -----------------------------------------------------------------
            // InvariantError (structural, recoverable)
            // -----------------------------------------------------------------

            /// <summary>
            /// Validates that the specified condition holds using an eagerly supplied
            /// <see cref="InvariantError"/>.
            /// </summary>
            /// <param name="condition">
            /// The condition to evaluate.
            /// </param>
            /// <param name="error">
            /// The invariant error describing the validation failure.
            /// The error value is captured eagerly.
            /// </param>
            /// <returns>
            /// A successful <see cref="Result{Unit}"/> if the condition holds;
            /// otherwise a failed Result containing <paramref name="error"/>.
            /// </returns>
            /// <remarks>
            /// <para>
            /// This overload should be used when error construction is inexpensive
            /// and independent of runtime context.
            /// </para>
            /// </remarks>
            public static Result<Unit> That(
                bool condition,
                DomainError error)
            {
                if (condition)
                    return Result.Success(Unit.Value);

                return Result.Failure<Unit>(error);
            }

            /// <summary>
            /// Validates that the specified condition holds using lazy
            /// <see cref="DomainError"/> construction.
            /// </summary>
            /// <param name="condition">
            /// The condition to evaluate.
            /// </param>
            /// <param name="errorFactory">
            /// A factory that lazily produces the domain error.
            /// The factory is invoked only when the condition fails.
            /// </param>
            /// <returns>
            /// A successful <see cref="Result{Unit}"/> if the condition holds;
            /// otherwise a failed Result containing the produced error.
            /// </returns>
            /// <remarks>
            /// <para>
            /// This overload avoids unnecessary allocations on successful code paths
            /// and allows richer contextual information to be captured at the point
            /// of failure.
            /// </para>
            /// </remarks>
            public static Result<Unit> That(
                bool condition,
                Func<DomainError> errorFactory)
            {
                if (condition)
                    return Result.Success(Unit.Value);

                return Result.Failure<Unit>(errorFactory);
            }

            // -----------------------------------------------------------------
            // DomainError (business, recoverable)
            // -----------------------------------------------------------------

            /// <summary>
            /// Validates that the specified condition holds using an eagerly supplied
            /// <see cref="DomainError"/>.
            /// </summary>
            /// <param name="condition">
            /// The condition to evaluate.
            /// </param>
            /// <param name="value">
            /// The value to wrap in the result if the condition holds.
            /// </param>
            /// <param name="error">
            /// The domain error describing the validation failure.
            /// </param>
            /// <returns>
            /// A successful <see cref="Result{Unit}"/> if the condition holds;
            /// otherwise a failed Result containing <paramref name="error"/>.
            /// </returns>
            /// <remarks>
            /// <para>
            /// Domain errors typically represent business-rule violations
            /// or invalid user input rather than structural domain corruption.
            /// </para>
            /// </remarks>
            public static Result<TValue> That<TValue>(
                bool condition,
                TValue value,
                DomainError error)
                where TValue : notnull
                => condition
                    ? Result.Success<TValue>(value)
                    : Result.Failure<TValue>(error);

            /// <summary>
            /// Validates that the specified condition holds using lazy
            /// <see cref="DomainError"/> construction.
            /// </summary>
            /// <param name="condition">
            /// The condition to evaluate.
            /// </param>
            /// <param name="value">
            /// The value to wrap in the result if the condition holds.
            /// </param>
            /// <param name="errorFactory">
            /// A factory that lazily produces the domain error.
            /// The factory is invoked only when the condition fails.
            /// </param>
            /// <returns>
            /// A successful <see cref="Result{TValue}"/> if the condition holds;
            /// otherwise a failed Result containing the produced error.
            /// </returns>
            /// <remarks>
            /// <para>
            /// This overload should be preferred when error construction depends
            /// on runtime state or diagnostic context.
            /// </para>
            /// </remarks>
            public static Result<TValue> That<TValue>(
                bool condition,
                TValue value,
                Func<DomainError> errorFactory)
                where TValue : notnull
                => condition
                    ? Result.Success<TValue>(value)
                    : Result.Failure<TValue>(errorFactory);

            // -----------------------------------------------------------------
            // Typed error (TError)
            // -----------------------------------------------------------------

            /// <summary>
            /// Validates that the specified condition holds using an eagerly supplied
            /// strongly typed error value.
            /// </summary>
            /// <typeparam name="TError">
            /// The type of the error value.
            /// </typeparam>
            /// <param name="condition">
            /// The condition to evaluate.
            /// </param>
            /// <param name="error">
            /// The error value describing the validation failure.
            /// </param>
            /// <returns>
            /// A successful <see cref="Result{Unit, TError}"/> if the condition holds;
            /// otherwise a failed Result containing <paramref name="error"/>.
            /// </returns>
            /// <remarks>
            /// <para>
            /// This overload enables domain‑specific error models while preserving
            /// Result‑based control flow.
            /// </para>
            /// </remarks>

            public static Result<Unit, TError> That<TError>(
                bool condition,
                TError error)
                where TError : notnull
                => condition
                    ? Dx.Result.Success<Unit, TError>(Unit.Value)
                    : Dx.Result.Failure<Unit, TError>(error);

            /// <summary>
            /// Validates that the specified condition holds using lazy construction
            /// of a strongly typed error value.
            /// </summary>
            /// <typeparam name="TError">
            /// The type of the error value.
            /// </typeparam>
            /// <param name="condition">
            /// The condition to evaluate.
            /// </param>
            /// <param name="errorFactory">
            /// A factory that lazily produces the error value.
            /// </param>
            /// <returns>
            /// A successful <see cref="Result{Unit, TError}"/> if the condition holds;
            /// otherwise a failed Result containing the produced error.
            /// </returns>
            /// <remarks>
            /// <para>
            /// This overload maximizes composability while avoiding premature
            /// allocation or capture of error data.
            /// </para>
            /// </remarks>
            public static Result<Unit, TError> That<TError>(
                bool condition,
                Func<TError> errorFactory)
                where TError : notnull
                => condition
                    ? Dx.Result.Success<Unit, TError>(Unit.Value)
                    : Dx.Result.Failure<Unit, TError>(errorFactory);


            public static Result<TValue, TError> That<TValue, TError>(
                bool condition,
                TValue value,
                TError error)
                where TValue : notnull
                where TError : notnull
                => condition
                    ? Dx.Result.Success<TValue, TError>(value)
                    : Dx.Result.Failure<TValue, TError>(error);


            public static Result<TValue, TError> That<TValue, TError>(
                bool condition,
                TValue value,
                Func<TError> errorFactory)
                where TValue : notnull
                where TError : notnull
                => condition
                    ? Dx.Result.Success<TValue, TError>(value)
                    : Dx.Result.Failure<TValue, TError>(errorFactory);
        }
    }
}
