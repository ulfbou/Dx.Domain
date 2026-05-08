// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="ResultExtensionsGeneric.cs" company="Dx.Domain Team">
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
using System.Collections.Generic;
using System.Threading.Tasks;

using static Dx.Domain.Dx;

namespace Dx.Domain
{
    /// <summary>
    /// Provides extension methods for composing, transforming, and handling <see cref="Result{TValue, TError}"/> instances
    /// with a generic error type.
    /// </summary>
    public static class ResultExtensionsGeneric
    {
        #region Map

        /// <summary>
        /// Transforms the success value of a <see cref="Result{TIn, TError}"/> using the specified mapping function.
        /// </summary>
        /// <typeparam name="TIn">The type of the source value. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the mapped value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="map">The mapping function to apply to the success value. Must not be <see langword="null"/>.</param>
        /// <returns>A new <see cref="Result{TOut, TError}"/> containing the mapped value if <paramref name="result"/> is successful; otherwise, a failure result containing the original error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="map"/> is <see langword="null"/>.</exception>
        public static Result<TOut, TError> Map<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TIn, TOut> map)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(map is not null, "Result.Map.ParameterCannotBeNull", "The mapping function cannot be null.");

            if (result.IsFailure)
                return Result.Failure<TOut, TError>(result.Error);

            return Result.Success<TOut, TError>(map!(result.Value));
        }

        /// <summary>
        /// Transforms the success value of a <see cref="Result{TIn, TError}"/> asynchronously using the specified mapping function.
        /// </summary>
        /// <typeparam name="TIn">The type of the source value. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the mapped value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="map">The asynchronous mapping function to apply to the success value. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a new <see cref="Result{TOut, TError}"/> with the mapped value if successful; otherwise, the original error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="map"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="map"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async Task<Result<TOut, TError>> MapAsync<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TIn, Task<TOut>> map)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(map is not null, "Result.MapAsync.ParameterCannotBeNull", "The mapping function cannot be null.");

            if (result.IsFailure)
                return Result.Failure<TOut, TError>(result.Error);

            try
            {
                var mappedValue = await map!(result.Value).ConfigureAwait(false);
                return Result.Success<TOut, TError>(mappedValue);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.MapAsync.MappingFunctionThrewException",
                    "An error occurred while executing the mapping function asynchronously.",
                    ex);
            }
        }

        #endregion

        #region Bind

        /// <summary>
        /// Binds the success value of a <see cref="Result{TIn, TError}"/> to a new result using the specified binding function.
        /// </summary>
        /// <typeparam name="TIn">The type of the source value. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the bound value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="bind">The binding function to apply to the success value. Must not be <see langword="null"/>.</param>
        /// <returns>The result produced by <paramref name="bind"/> if <paramref name="result"/> is successful; otherwise, a failure result containing the original error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="bind"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="bind"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TOut, TError> Bind<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TIn, Result<TOut, TError>> bind)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(bind is not null, "Result.Bind.ParameterCannotBeNull", "The binding function cannot be null.");

            if (result.IsFailure)
                return Result.Failure<TOut, TError>(result.Error);

            try
            {
                return bind!(result.Value);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Bind.BindingFunctionThrewException",
                    "An error occurred while executing the binding function.",
                    ex);
            }
        }

        /// <summary>
        /// Binds the success value of a <see cref="Result{TIn, TError}"/> to a new result asynchronously using the specified binding function.
        /// </summary>
        /// <typeparam name="TIn">The type of the source value. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the bound value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="bind">The asynchronous binding function to apply to the success value. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the bound result if successful; otherwise, the original error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="bind"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="bind"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async Task<Result<TOut, TError>> BindAsync<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TIn, Task<Result<TOut, TError>>> bind)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(bind is not null, "Result.BindAsync.ParameterCannotBeNull", "The binding function cannot be null.");

            if (result.IsFailure)
                return Result.Failure<TOut, TError>(result.Error);

            try
            {
                return await bind!(result.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.BindAsync.BindingFunctionThrewException",
                    "An error occurred while executing the binding function asynchronously.",
                    ex);
            }
        }

        #endregion

        #region Tap

        /// <summary>
        /// Executes the specified action if the result is successful, returning the original result unchanged.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="action">The action to execute on the success value. Must not be <see langword="null"/>.</param>
        /// <returns>The original <paramref name="result"/>.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue, TError> Tap<TValue, TError>(this Result<TValue, TError> result, Action<TValue> action)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(action is not null, "Result.Tap.ParameterCannotBeNull", "The tap action cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                action!(result.Value);
                return result;
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Tap.TapActionThrewException",
                    "An error occurred while executing the tap action.",
                    ex);
            }
        }

        /// <summary>
        /// Executes the specified asynchronous action if the result is successful, returning the original result unchanged.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="action">The asynchronous action to execute on the success value. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the original <paramref name="result"/>.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async Task<Result<TValue, TError>> TapAsync<TValue, TError>(this Result<TValue, TError> result, Func<TValue, Task> action)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(action is not null, "Result.TapAsync.ParameterCannotBeNull", "The tap action cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                await action!(result.Value).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.TapAsync.TapActionThrewException",
                    "An error occurred while executing the tap action asynchronously.",
                    ex);
            }
        }

        #endregion

        #region Ensure / Validate

        /// <summary>
        /// Ensures the success value satisfies the specified predicate, otherwise returns a failure with the provided error.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="predicate">The predicate to evaluate against the success value. Must not be <see langword="null"/>.</param>
        /// <param name="error">The error to return if <paramref name="predicate"/> returns <see langword="false"/>.</param>
        /// <returns>The original result if the predicate is satisfied; otherwise, a failure result containing <paramref name="error"/>.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue, TError> Ensure<TValue, TError>(this Result<TValue, TError> result, Func<TValue, bool> predicate, TError error)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return predicate!(result.Value) ? result : Result.Failure<TValue, TError>(error);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Ensure.PredicateThrewException",
                    "An error occurred while evaluating the predicate in Ensure.",
                    ex);
            }
        }

        /// <summary>
        /// Ensures the success value satisfies the specified predicate, otherwise returns a failure with an error produced by the factory.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="predicate">The predicate to evaluate against the success value. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The factory function that produces the error when the predicate fails. Must not be <see langword="null"/>.</param>
        /// <returns>The original result if the predicate is satisfied; otherwise, a failure result containing the produced error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue, TError> Ensure<TValue, TError>(this Result<TValue, TError> result, Func<TValue, bool> predicate, Func<TError> errorFactory)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");
            Invariant.That(errorFactory is not null, "Result.Ensure.ParameterCannotBeNull", "The error factory cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return predicate!(result.Value) ? result : Result.Failure<TValue, TError>(errorFactory!());
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Ensure.PredicateThrewException",
                    "An error occurred while evaluating the predicate in Ensure.",
                    ex);
            }
        }

        /// <summary>
        /// Ensures the success value satisfies the specified asynchronous predicate, otherwise returns a failure with the provided error.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="predicate">The asynchronous predicate to evaluate against the success value. Must not be <see langword="null"/>.</param>
        /// <param name="error">The error to return if <paramref name="predicate"/> returns <see langword="false"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the original result if satisfied; otherwise, a failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async Task<Result<TValue, TError>> EnsureAsync<TValue, TError>(this Result<TValue, TError> result, Func<TValue, Task<bool>> predicate, TError error)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return await predicate!(result.Value).ConfigureAwait(false)
                    ? result
                    : Result.Failure<TValue, TError>(error);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Ensure.PredicateThrewException",
                    "An error occurred while evaluating the predicate in Ensure.",
                    ex);
            }
        }

        /// <summary>
        /// Ensures the success value satisfies the specified asynchronous predicate, otherwise returns a failure with an error produced asynchronously.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="predicate">The asynchronous predicate to evaluate against the success value. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The asynchronous factory that produces the error when the predicate fails. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the original result if satisfied; otherwise, a failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async Task<Result<TValue, TError>> EnsureAsync<TValue, TError>(this Result<TValue, TError> result, Func<TValue, Task<bool>> predicate, Func<Task<TError>> errorFactory)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");
            Invariant.That(errorFactory is not null, "Result.Ensure.ParameterCannotBeNull", "The error factory cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return await predicate!(result.Value).ConfigureAwait(false)
                    ? result
                    : Result.Failure<TValue, TError>(await errorFactory!().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Ensure.PredicateThrewException",
                    "An error occurred while evaluating the predicate in Ensure.",
                    ex);
            }
        }

        #endregion

        #region Recover / Fallback

        /// <summary>
        /// Recovers from a failure by transforming the error into a success value using the specified recovery function.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="recovery">The recovery function that produces a value from the error. Must not be <see langword="null"/>.</param>
        /// <returns>A success result containing the recovered value if <paramref name="result"/> is a failure; otherwise, the original result.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recovery"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recovery"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue, TError> Recover<TValue, TError>(this Result<TValue, TError> result, Func<TError, TValue> recovery)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(recovery is not null, "Result.Recover.ParameterCannotBeNull", "The recovery function cannot be null.");

            try
            {
                return result.IsFailure
                    ? Result.Success<TValue, TError>(recovery!(result.Error))
                    : result;
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Recover.RecoveryFunctionThrewException",
                    "An error occurred while executing the recovery function.",
                    ex);
            }
        }

        /// <summary>
        /// Recovers from a failure by producing a new result using the specified recovery function.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="recovery">The recovery function that produces a result from the error. Must not be <see langword="null"/>.</param>
        /// <returns>The result produced by <paramref name="recovery"/> if <paramref name="result"/> is a failure; otherwise, the original result.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recovery"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recovery"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue, TError> Recover<TValue, TError>(this Result<TValue, TError> result, Func<TError, Result<TValue, TError>> recovery)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(recovery is not null, "Result.Recover.ParameterCannotBeNull", "The recovery function cannot be null.");

            try
            {
                return result.IsFailure ? recovery!(result.Error) : result;
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Recover.RecoveryFunctionThrewException",
                    "An error occurred while executing the recovery function.",
                    ex);
            }
        }

        /// <summary>
        /// Recovers from a failure asynchronously by transforming the error into a success value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="recovery">The asynchronous recovery function that produces a value from the error. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the recovered success if applicable.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recovery"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recovery"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async Task<Result<TValue, TError>> RecoverAsync<TValue, TError>(this Result<TValue, TError> result, Func<TError, Task<TValue>> recovery)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(recovery is not null, "Result.Recover.ParameterCannotBeNull", "The recovery function cannot be null.");

            try
            {
                return result.IsFailure
                    ? Result.Success<TValue, TError>(await recovery!(result.Error).ConfigureAwait(false))
                    : result;
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Recover.RecoveryFunctionThrewException",
                    "An error occurred while executing the recovery function.",
                    ex);
            }
        }

        /// <summary>
        /// Recovers from a failure asynchronously by producing a new result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="recovery">The asynchronous recovery function that produces a result from the error. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the recovered result if applicable.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recovery"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recovery"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async Task<Result<TValue, TError>> RecoverAsync<TValue, TError>(this Result<TValue, TError> result, Func<TError, Task<Result<TValue, TError>>> recovery)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(recovery is not null, "Result.Recover.ParameterCannotBeNull", "The recovery function cannot be null.");

            try
            {
                return result.IsFailure
                    ? await recovery!(result.Error).ConfigureAwait(false)
                    : result;
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Recover.RecoveryFunctionThrewException",
                    "An error occurred while executing the recovery function.",
                    ex);
            }
        }

        #endregion

        #region Match / Observers

        /// <summary>
        /// Projects a <see cref="Result{TValue, TError}"/> into a value by invoking the appropriate handler.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the projected value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccess">The handler invoked when <paramref name="result"/> is successful. Must not be <see langword="null"/>.</param>
        /// <param name="onFailure">The handler invoked when <paramref name="result"/> is a failure. Must not be <see langword="null"/>.</param>
        /// <returns>The value produced by the invoked handler.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onSuccess"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onFailure"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when either handler throws an exception. The original exception is included as the inner exception.</exception>
        public static TOut Match<TValue, TError, TOut>(this Result<TValue, TError> result, Func<TValue, TOut> onSuccess, Func<TError, TOut> onFailure)
            where TValue : notnull
            where TError : notnull
            where TOut : notnull
        {
            Invariant.That(onSuccess is not null, "Result.Match.ParameterCannotBeNull", "The onSuccess function cannot be null.");
            Invariant.That(onFailure is not null, "Result.Match.ParameterCannotBeNull", "The onFailure function cannot be null.");

            try
            {
                return result.IsSuccess
                    ? onSuccess!(result.Value)
                    : onFailure!(result.Error);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Match.HandlerFunctionThrewException",
                    "An error occurred while executing one of the match handler functions.",
                    ex);
            }
        }

        /// <summary>
        /// Invokes the appropriate action based on the state of the result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccess">The action invoked when <paramref name="result"/> is successful. Must not be <see langword="null"/>.</param>
        /// <param name="onFailure">The action invoked when <paramref name="result"/> is a failure. Must not be <see langword="null"/>.</param>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onSuccess"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onFailure"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when either action throws an exception. The original exception is included as the inner exception.</exception>
        public static void Match<TValue, TError>(this Result<TValue, TError> result, Action<TValue> onSuccess, Action<TError> onFailure)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(onSuccess is not null, "Result.Match.ParameterCannotBeNull", "The onSuccess function cannot be null.");
            Invariant.That(onFailure is not null, "Result.Match.ParameterCannotBeNull", "The onFailure function cannot be null.");

            try
            {
                if (result.IsSuccess)
                    onSuccess!(result.Value);
                else
                    onFailure!(result.Error);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Match.HandlerFunctionThrewException",
                    "An error occurred while executing one of the match handler functions.",
                    ex);
            }
        }

        /// <summary>
        /// Projects a <see cref="Result{T, TError}"/> into a value asynchronously by invoking the appropriate handler.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the projected value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccess">The asynchronous handler invoked when <paramref name="result"/> is successful. Must not be <see langword="null"/>.</param>
        /// <param name="onFailure">The asynchronous handler invoked when <paramref name="result"/> is a failure. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the value produced by the invoked handler.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onSuccess"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onFailure"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when either handler throws an exception. The original exception is included as the inner exception.</exception>
        public static async Task<TOut> MatchAsync<TValue, TError, TOut>(this Result<TValue, TError> result, Func<TValue, Task<TOut>> onSuccess, Func<TError, Task<TOut>> onFailure)
            where TValue : notnull
            where TError : notnull
            where TOut : notnull
        {
            Invariant.That(onSuccess is not null, "Result.Match.ParameterCannotBeNull", "The onSuccess function cannot be null.");
            Invariant.That(onFailure is not null, "Result.Match.ParameterCannotBeNull", "The onFailure function cannot be null.");

            try
            {
                return result.IsSuccess
                    ? await onSuccess!(result.Value).ConfigureAwait(false)
                    : await onFailure!(result.Error).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Match.HandlerFunctionThrewException",
                    "An error occurred while executing one of the match handler functions.",
                    ex);
            }
        }

        /// <summary>
        /// Invokes the appropriate asynchronous action based on the state of the result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccess">The asynchronous action invoked when <paramref name="result"/> is successful. Must not be <see langword="null"/>.</param>
        /// <param name="onFailure">The asynchronous action invoked when <paramref name="result"/> is a failure. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onSuccess"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onFailure"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when either action throws an exception. The original exception is included as the inner exception.</exception>
        public static async Task MatchAsync<TValue, TError>(this Result<TValue, TError> result, Func<TValue, Task> onSuccess, Func<TError, Task> onFailure)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(onSuccess is not null, "Result.Match.ParameterCannotBeNull", "The onSuccess function cannot be null.");
            Invariant.That(onFailure is not null, "Result.Match.ParameterCannotBeNull", "The onFailure function cannot be null.");

            try
            {
                if (result.IsSuccess)
                    await onSuccess!(result.Value).ConfigureAwait(false);
                else
                    await onFailure!(result.Error).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Match.HandlerFunctionThrewException",
                    "An error occurred while executing one of the match handler functions.",
                    ex);
            }
        }

        #endregion

        #region Flatten / Sequence / Traverse

        /// <summary>
        /// Flattens a nested <see cref="Result{TValue, TError}"/> into a single-level result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The nested result to flatten.</param>
        /// <returns>The inner result if <paramref name="result"/> is successful; otherwise, a failure containing the outer error.</returns>
        public static Result<TValue, TError> Flatten<TValue, TError>(this Result<Result<TValue, TError>, TError> result)
            where TValue : notnull
            where TError : notnull
            => result.IsFailure ? Result.Failure<TValue, TError>(result.Error) : result.Value;

        /// <summary>
        /// Converts a sequence of results into a result containing a read-only list, failing fast on the first error.
        /// </summary>
        /// <typeparam name="TValue">The type of the values. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="results">The sequence of results to evaluate. Must not be <see langword="null"/>.</param>
        /// <returns>A success result containing all values if all results succeed; otherwise, the first encountered failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="results"/> is <see langword="null"/>.</exception>
        public static Result<IReadOnlyList<TValue>, TError> Sequence<TValue, TError>(this IEnumerable<Result<TValue, TError>> results)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(results is not null, "Result.Sequence.ParameterCannotBeNull", "The results sequence cannot be null.");

            var list = new List<TValue>();
            foreach (var r in results!)
            {
                if (r.IsFailure)
                    return Result.Failure<IReadOnlyList<TValue>, TError>(r.Error);

                list.Add(r.Value);
            }

            return Result.Success<IReadOnlyList<TValue>, TError>(list);
        }

        /// <summary>
        /// Projects each element of a sequence into a result and aggregates successful values, failing fast on the first error.
        /// </summary>
        /// <typeparam name="TIn">The type of the source elements. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the projected values. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="source">The source sequence. Must not be <see langword="null"/>.</param>
        /// <param name="selector">The projection function that produces a result for each element. Must not be <see langword="null"/>.</param>
        /// <returns>A success result containing all projected values if all succeed; otherwise, the first encountered failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="selector"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="selector"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<IReadOnlyList<TOut>, TError> Traverse<TIn, TOut, TError>(this IEnumerable<TIn> source, Func<TIn, Result<TOut, TError>> selector)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(source is not null, "Result.Traverse.ParameterCannotBeNull", "The source sequence cannot be null.");
            Invariant.That(selector is not null, "Result.Traverse.ParameterCannotBeNull", "The selector function cannot be null.");

            var list = new List<TOut>();
            foreach (var item in source!)
            {
                try
                {
                    var r = selector!(item);
                    if (r.IsFailure)
                        return Result.Failure<IReadOnlyList<TOut>, TError>(r.Error);

                    list.Add(r.Value);
                }
                catch (Exception ex)
                {
                    throw InvariantViolationException.Create(
                        "Result.Traverse.SelectorFunctionThrewException",
                        "An error occurred while executing the selector function.",
                        ex);
                }
            }

            return Result.Success<IReadOnlyList<TOut>, TError>(list);
        }

        /// <summary>
        /// Projects each element of a sequence into a result asynchronously and aggregates successful values, failing fast on the first error.
        /// </summary>
        /// <typeparam name="TIn">The type of the source elements. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the projected values. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="source">The source sequence. Must not be <see langword="null"/>.</param>
        /// <param name="selector">The asynchronous projection function that produces a result for each element. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains aggregated values or the first failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="selector"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="selector"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async Task<Result<IReadOnlyList<TOut>, TError>> TraverseAsync<TIn, TOut, TError>(this IEnumerable<TIn> source, Func<TIn, Task<Result<TOut, TError>>> selector)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(source is not null, "Result.Traverse.ParameterCannotBeNull", "The source sequence cannot be null.");
            Invariant.That(selector is not null, "Result.Traverse.ParameterCannotBeNull", "The selector function cannot be null.");

            var list = new List<TOut>();
            foreach (var item in source!)
            {
                try
                {
                    var r = await selector!(item).ConfigureAwait(false);
                    if (r.IsFailure)
                        return Result.Failure<IReadOnlyList<TOut>, TError>(r.Error);

                    list.Add(r.Value);
                }
                catch (Exception ex)
                {
                    throw InvariantViolationException.Create(
                        "Result.Traverse.SelectorFunctionThrewException",
                        "An error occurred while executing the selector function asynchronously.",
                        ex);
                }
            }

            return Result.Success<IReadOnlyList<TOut>, TError>(list);
        }

        #endregion

        #region Try/Catch helpers

        /// <summary>
        /// Executes the specified function and captures any thrown exception as a failure result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="func">The function to execute. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The factory that produces an error from the caught exception. Must not be <see langword="null"/>.</param>
        /// <returns>A success result containing the function value if no exception occurs; otherwise, a failure result.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="func"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        public static Result<TValue, TError> TryCatch<TValue, TError>(Func<TValue> func, Func<Exception, TError> errorFactory)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(func is not null, "Result.TryCatch.ParameterCannotBeNull", "The function cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatch.ParameterCannotBeNull", "The error factory function cannot be null.");

            return TrySucceed(func!, errorFactory!);
        }

        /// <summary>
        /// Executes the specified action and captures any thrown exception as a failure result.
        /// </summary>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="action">The action to execute. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The factory that produces an error from the caught exception. Must not be <see langword="null"/>.</param>
        /// <returns>A success result containing <see cref="Unit"/> if no exception occurs; otherwise, a failure result.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        public static Result<Unit, TError> TryCatch<TError>(Action action, Func<Exception, TError> errorFactory)
            where TError : notnull
        {
            Invariant.That(action is not null, "Result.TryCatch.ParameterCannotBeNull", "The action cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatch.ParameterCannotBeNull", "The error factory function cannot be null.");

            return TrySucceed(
                () =>
                {
                    action!();
                    return Unit.Value;
                },
                errorFactory!);
        }

        /// <summary>
        /// Executes the specified asynchronous function and captures any thrown exception as a failure result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="func">The asynchronous function to execute. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The factory that produces an error from the caught exception. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains success or captured failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="func"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        public static async Task<Result<TValue, TError>> TryCatchAsync<TValue, TError>(Func<Task<TValue>> func, Func<Exception, TError> errorFactory)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(func is not null, "Result.TryCatchAsync.Func.CannotBeNull", "The function to execute cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatchAsync.ErrorFactory.CannotBeNull", "The error factory cannot be null.");

            return await TrySucceedAsync(func!, errorFactory!).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the specified asynchronous action and captures any thrown exception as a failure result.
        /// </summary>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="action">The asynchronous action to execute. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The factory that produces an error from the caught exception. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains success or captured failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        public static async Task<Result<Unit, TError>> TryCatchAsync<TError>(Func<Task> action, Func<Exception, TError> errorFactory)
            where TError : notnull
        {
            Invariant.That(action is not null, "Result.TryCatchAsync.Action.CannotBeNull", "The action to execute cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatchAsync.ErrorFactory.CannotBeNull", "The error factory cannot be null.");

            return await TrySucceedAsync<Unit, TError>(
                async () =>
                {
                    await action!().ConfigureAwait(false);
                    return Unit.Value;
                },
                errorFactory!).ConfigureAwait(false);
        }

        #endregion

        #region Conversions / Utilities

        /// <summary>
        /// Returns the success value of the result, throwing if the result is a failure.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <returns>The success value contained in <paramref name="result"/>.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="result"/> is in a failure state.</exception>
        public static TValue Unwrap<TValue, TError>(this Result<TValue, TError> result)
            where TValue : notnull
            where TError : notnull
        {
            if (result.IsFailure)
            {
                throw InvariantViolationException.Create("Result.Unwrap.Failure", "Attempted to unwrap a Result that is in a failure state.");
            }

            return result.Value;
        }

        /// <summary>
        /// Wraps the result in a completed <see cref="Task{TResult}"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <returns>A completed task containing <paramref name="result"/>.</returns>
        public static Task<Result<TValue, TError>> AsTask<TValue, TError>(this Result<TValue, TError> result)
            where TValue : notnull
            where TError : notnull
            => Task.FromResult(result);

        /// <summary>
        /// Wraps the result in a completed <see cref="ValueTask{TResult}"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <returns>A completed value task containing <paramref name="result"/>.</returns>
        public static ValueTask<Result<TValue, TError>> AsValueTask<TValue, TError>(this Result<TValue, TError> result)
            where TValue : notnull
            where TError : notnull
            => new(result);

        /// <summary>
        /// Converts a result with a generic error type to a result using <see cref="DomainError"/> by mapping the error.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the source error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="mapError">The function that maps the source error to a <see cref="DomainError"/>. Must not be <see langword="null"/>.</param>
        /// <returns>A success result if <paramref name="result"/> is successful; otherwise, a failure containing the mapped <see cref="DomainError"/>.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="mapError"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="mapError"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue> ToDomainError<TValue, TError>(this Result<TValue, TError> result, Func<TError, DomainError> mapError)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(mapError is not null,
                "Result.ToDomainError.MapErrorCannotBeNull",
                "The error mapping function cannot be null.");

            if (result.IsSuccess)
                return Result<TValue>.Success(result.Value);

            try
            {
                var domainError = mapError!(result.Error);
                return Result.Failure<TValue>(domainError);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.ToDomainError.MappingFailed",
                    "An exception was thrown while mapping the error to a DomainError.",
                    ex);
            }
        }

        /// <summary>
        /// Transforms the error of a failure result using the specified mapping function.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TErrorIn">The type of the source error. Must be non-nullable.</typeparam>
        /// <typeparam name="TErrorOut">The type of the mapped error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="mapError">The function that maps the error to a new error type. Must not be <see langword="null"/>.</param>
        /// <returns>A success result if <paramref name="result"/> is successful; otherwise, a failure containing the mapped error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="mapError"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="mapError"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue, TErrorOut> MapError<TValue, TErrorIn, TErrorOut>(this Result<TValue, TErrorIn> result, Func<TErrorIn, TErrorOut> mapError)
            where TValue : notnull
            where TErrorIn : notnull
            where TErrorOut : notnull
        {
            Invariant.That(mapError is not null,
                "Result.MapError.MapErrorCannotBeNull",
                "The error mapping function cannot be null.");

            if (result.IsSuccess)
                return Result.Success<TValue, TErrorOut>(result.Value);

            try
            {
                var mappedError = mapError!(result.Error);
                return Result.Failure<TValue, TErrorOut>(mappedError);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.MapError.MappingFailed",
                    "An exception was thrown while mapping the error to the new error type.",
                    ex);
            }
        }

        #endregion

        #region Helpers

        private static async Task<Result<TValue, TError>> TrySucceedAsync<TValue, TError>(Func<Task<TValue>> func, Func<Exception, TError> errorFactory)
            where TValue : notnull
            where TError : notnull
        {
            try
            {
                var result = await func().ConfigureAwait(false);
                return Result.Success<TValue, TError>(result);
            }
            catch (Exception ex)
            {
                return TryFail<TValue, TError>(ex, errorFactory);
            }
        }

        private static Result<TValue, TError> TrySucceed<TValue, TError>(Func<TValue> func, Func<Exception, TError> errorFactory)
            where TValue : notnull
            where TError : notnull
        {
            try
            {
                var result = func();
                return Result.Success<TValue, TError>(result);
            }
            catch (Exception ex)
            {
                return TryFail<TValue, TError>(ex, errorFactory);
            }
        }

        private static Result<TValue, TError> TryFail<TValue, TError>(Exception ex, Func<Exception, TError> errorFactory)
            where TValue : notnull
            where TError : notnull
        {
            try
            {
                return Result.Failure<TValue, TError>(errorFactory(ex));
            }
            catch (Exception ex2)
            {
                var aggregateException = new AggregateException(
                    "An exception was thrown while creating the error from the caught exception.",
                    ex,
                    ex2);

                throw InvariantViolationException.Create(
                    "Result.TryCatchAsync.ErrorFactory.Thrown",
                    "An exception was thrown while creating the error from the caught exception.",
                    aggregateException);
            }
        }

        #endregion
    }
}
