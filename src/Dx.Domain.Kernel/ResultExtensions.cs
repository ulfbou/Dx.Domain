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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Dx.Domain.Dx;

namespace Dx.Domain
{
    /// <summary>
    /// Provides extension methods for composing, transforming, and handling results in a functional style.
    /// </summary>
    public static class ResultExtensions
    {
        #region Map

        /// <summary>
        /// Transforms the success value of a <see cref="Result{TIn}"/> using the specified mapping function.
        /// </summary>
        /// <typeparam name="TIn">The type of the source value. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the mapped value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="mapFunc">The mapping function to apply to the success value. Must not be <see langword="null"/>.</param>
        /// <returns>A new <see cref="Result{TOut}"/> containing the mapped value if <paramref name="result"/> is successful; otherwise, a failure result containing the original error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="mapFunc"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="mapFunc"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapFunc)
            where TIn : notnull
            where TOut : notnull
        {
            Invariant.That(mapFunc is not null, "ResultMap.Parameter.CannotBeNull", $"Parameter '{nameof(mapFunc)}' cannot be null.");

            if (result.IsFailure)
                return Result.Failure<TOut>(result.Error);

            try
            {
                var mappedValue = mapFunc!(result.Value);
                return Result.Success(mappedValue);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Map.Exception", "An exception occurred while mapping the Result.", ex);
            }
        }

        /// <summary>
        /// Transforms the success value of a <see cref="Result{TIn}"/> asynchronously using the specified mapping function.
        /// </summary>
        /// <typeparam name="TIn">The type of the source value. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the mapped value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="map">The asynchronous mapping function to apply to the success value. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains a new <see cref="Result{TOut}"/> with the mapped value if successful; otherwise, the original error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="map"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="map"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<Result<TOut>> MapAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<TOut>> map)
            where TIn : notnull
            where TOut : notnull
        {
            Invariant.That(map is not null, "ResultMapAsync.Parameter.CannotBeNull", $"Parameter '{nameof(map)}' cannot be null.");

            if (result.IsFailure)
                return Result.Failure<TOut>(result.Error);

            try
            {
                var mappedValue = await map!(result.Value).ConfigureAwait(false);
                return Result.Success(mappedValue);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.MapAsync.Exception", "An exception occurred while mapping the result asynchronously.", ex);
            }
        }

        #endregion

        #region Bind

        /// <summary>
        /// Binds the success value of a <see cref="Result{TIn}"/> to a new result using the specified binding function.
        /// </summary>
        /// <typeparam name="TIn">The type of the source value. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the bound value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="bindFunc">The binding function to apply to the success value. Must not be <see langword="null"/>.</param>
        /// <returns>The result produced by <paramref name="bindFunc"/> if <paramref name="result"/> is successful; otherwise, a failure result containing the original error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="bindFunc"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="bindFunc"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bindFunc)
            where TIn : notnull
            where TOut : notnull
        {
            Invariant.That(bindFunc is not null, "Result.Bind.ParameterCannotBeNull", "The binding function cannot be null.");

            try
            {
                return result.IsFailure
                    ? Result.Failure<TOut>(result.Error)
                    : bindFunc!(result.Value);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Bind.Exception", "An exception occurred while binding the Result.", ex);
            }
        }

        /// <summary>
        /// Binds the success value of a <see cref="Result{TIn}"/> to a new result asynchronously using the specified binding function.
        /// </summary>
        /// <typeparam name="TIn">The type of the source value. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the bound value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="bindFunc">The asynchronous binding function to apply to the success value. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains the bound result if successful; otherwise, the original error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="bindFunc"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="bindFunc"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<Result<TOut>> BindAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> bindFunc)
            where TIn : notnull
            where TOut : notnull
        {
            Invariant.That(bindFunc is not null, "Result.BindAsync.ParameterCannotBeNull", "The binding function cannot be null.");

            try
            {
                return result.IsFailure
                    ? Result.Failure<TOut>(result.Error)
                    : await bindFunc!(result.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.BindAsync.Exception", "An exception occurred while binding the result asynchronously.", ex);
            }
        }

        #endregion

        #region Tap

        /// <summary>
        /// Executes the specified action if the result is successful, returning the original result unchanged.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="action">The action to execute on the success value. Must not be <see langword="null"/>.</param>
        /// <returns>The original <paramref name="result"/>.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue> Tap<TValue>(this Result<TValue> result, Action<TValue> action)
            where TValue : notnull
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
                throw InvariantViolationException.Create("Result.Tap.Exception", "An exception occurred while executing the tap action.", ex);
            }
        }

        /// <summary>
        /// Executes the specified asynchronous action if the result is successful, returning the original result unchanged.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="action">The asynchronous action to execute on the success value. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains the original <paramref name="result"/>.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<Result<TValue>> TapAsync<TValue>(this Result<TValue> result, Func<TValue, Task> action)
            where TValue : notnull
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
                throw InvariantViolationException.Create("Result.TapAsync.Exception", "An exception occurred while executing the tap action asynchronously.", ex);
            }
        }

        #endregion

        #region Ensure

        /// <summary>
        /// Ensures the success value satisfies the specified predicate, otherwise returns a failure with the provided error.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="predicate">The predicate to evaluate against the success value. Must not be <see langword="null"/>.</param>
        /// <param name="error">The error to return if <paramref name="predicate"/> returns <see langword="false"/>.</param>
        /// <returns>The original result if the predicate is satisfied; otherwise, a failure result containing <paramref name="error"/>.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> throws an exception. The original
        /// exception is included as the inner exception.</exception>
        public static Result<TValue> Ensure<TValue>(this Result<TValue> result, Func<TValue, bool> predicate, DomainError error) where TValue : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return predicate!(result.Value)
                    ? result
                    : Result.Failure<TValue>(error);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Ensure.Exception", "An exception occurred while evaluating the predicate.", ex);
            }
        }

        /// <summary>
        /// Ensures the success value satisfies the specified predicate, otherwise returns a failure with an error produced by the factory.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="predicate">The predicate to evaluate against the success value. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The factory function that produces the error when the predicate fails. Must not be <see langword="null"/>.</param>
        /// <returns>The original result if the predicate is satisfied; otherwise, a failure result containing the produced error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue> Ensure<TValue>(this Result<TValue> result, Func<TValue, bool> predicate, Func<DomainError> errorFactory) where TValue : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");
            Invariant.That(errorFactory is not null, "Result.Ensure.ParameterCannotBeNull", "The error factory cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return predicate!(result.Value)
                    ? result
                    : Result.Failure<TValue>(errorFactory!());
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Ensure.Exception", "An exception occurred while evaluating the predicate.", ex);
            }
        }

        /// <summary>
        /// Ensures the success value satisfies the specified asynchronous predicate, otherwise returns a failure with the provided error.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="predicate">The asynchronous predicate to evaluate against the success value. Must not be <see langword="null"/>.</param>
        /// <param name="error">The error to return if <paramref name="predicate"/> returns <see langword="false"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains the original result if satisfied; otherwise, a failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<Result<TValue>> EnsureAsync<TValue>(this Result<TValue> result, Func<TValue, Task<bool>> predicate, DomainError error) where TValue : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return await predicate!(result.Value).ConfigureAwait(false)
                    ? result
                    : Result.Failure<TValue>(error);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Ensure.Exception", "An exception occurred while evaluating the predicate asynchronously.", ex);
            }
        }

        /// <summary>
        /// Ensures the success value satisfies the specified asynchronous predicate, otherwise returns a failure with an error produced asynchronously.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="predicate">The asynchronous predicate to evaluate against the success value. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The asynchronous factory that produces the error when the predicate fails. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains the original result if satisfied; otherwise, a failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="predicate"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<Result<TValue>> EnsureAsync<TValue>(this Result<TValue> result, Func<TValue, Task<bool>> predicate, Func<Task<DomainError>> errorFactory) where TValue : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");
            Invariant.That(errorFactory is not null, "Result.Ensure.ParameterCannotBeNull", "The error factory cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return await predicate!(result.Value).ConfigureAwait(false)
                    ? result
                    : Result.Failure<TValue>(await errorFactory!().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Ensure.Exception", "An exception occurred while evaluating the predicate asynchronously.", ex);
            }
        }

        #endregion

        #region Recover

        /// <summary>
        /// Recovers from a failure by transforming the error into a success value using the specified recovery function.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="recoveryFunc">The recovery function that produces a value from the error. Must not be <see langword="null"/>.</param>
        /// <returns>A success result containing the recovered value if <paramref name="result"/> is a failure; otherwise, the original result.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recoveryFunc"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recoveryFunc"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue> Recover<TValue>(this Result<TValue> result, Func<DomainError, TValue> recoveryFunc) where TValue : notnull
        {
            Invariant.That(recoveryFunc is not null, "Result.Recover.ParameterCannotBeNull", "The recovery function cannot be null.");

            try
            {
                return result.IsFailure
                    ? Result.Success(recoveryFunc!(result.Error))
                    : result;
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Recover.Exception", "An exception occurred while executing the recovery function.", ex);
            }
        }

        /// <summary>
        /// Recovers from a failure by producing a new result using the specified recovery function.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="recoveryFunc">The recovery function that produces a result from the error. Must not be <see langword="null"/>.</param>
        /// <returns>The result produced by <paramref name="recoveryFunc"/> if <paramref name="result"/> is a failure; otherwise, the original result.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recoveryFunc"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recoveryFunc"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<TValue> Recover<TValue>(this Result<TValue> result, Func<DomainError, Result<TValue>> recoveryFunc) where TValue : notnull
        {
            Invariant.That(recoveryFunc is not null, "Result.Recover.ParameterCannotBeNull", "The recovery function cannot be null.");

            try
            {
                return result.IsFailure ? recoveryFunc!(result.Error) : result;
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Recover.Exception", "An exception occurred while executing the recovery function.", ex);
            }
        }

        /// <summary>
        /// Recovers from a failure asynchronously by transforming the error into a success value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="recoveryFunc">The asynchronous recovery function that produces a value from the error. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains the recovered success if applicable.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recoveryFunc"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recoveryFunc"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<Result<TValue>> RecoverAsync<TValue>(this Result<TValue> result, Func<DomainError, Task<TValue>> recoveryFunc) where TValue : notnull
        {
            Invariant.That(recoveryFunc is not null, "Result.Recover.ParameterCannotBeNull", "The recovery function cannot be null.");

            try
            {
                return result.IsFailure
                    ? Result.Success(await recoveryFunc!(result.Error).ConfigureAwait(false))
                    : result;
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.RecoverAsync.Exception", "An exception occurred while executing the recovery function asynchronously.", ex);
            }
        }

        /// <summary>
        /// Recovers from a failure asynchronously by producing a new result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="recovery">The asynchronous recovery function that produces a result from the error. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains the recovered result if applicable.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recovery"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="recovery"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<Result<TValue>> RecoverAsync<TValue>(this Result<TValue> result, Func<DomainError, Task<Result<TValue>>> recovery) where TValue : notnull
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
                throw InvariantViolationException.Create("Result.Recover.Exception", "An exception occurred while executing the recovery function asynchronously.", ex);
            }
        }

        #endregion

        #region Match

        /// <summary>
        /// Projects a <see cref="Result{TValue}"/> into a value by invoking the appropriate handler.
        /// </summary>
        /// <typeparam name="TOut">The type of the projected value. Must be non-nullable.</typeparam>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccess">The handler invoked when <paramref name="result"/> is successful. Must not be <see langword="null"/>.</param>
        /// <param name="onFailure">The handler invoked when <paramref name="result"/> is a failure. Must not be <see langword="null"/>.</param>
        /// <returns>The value produced by the invoked handler.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onSuccess"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onFailure"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when either handler throws an exception. The original exception is included as the inner exception.</exception>
        public static TOut Match<TOut, TValue>(this Result<TValue> result, Func<TValue, TOut> onSuccess, Func<DomainError, TOut> onFailure)
            where TValue : notnull
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
                throw InvariantViolationException.Create("Result.Match.Exception", "An exception occurred while executing the match functions.", ex);
            }
        }

        /// <summary>
        /// Invokes the appropriate action based on the state of the result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccess">The action invoked when <paramref name="result"/> is successful. Must not be <see langword="null"/>.</param>
        /// <param name="onFailure">The action invoked when <paramref name="result"/> is a failure. Must not be <see langword="null"/>.</param>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onSuccess"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onFailure"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when either action throws an exception. The original exception is included as the inner exception.</exception>
        public static void Match<TValue>(this Result<TValue> result, Action<TValue> onSuccess, Action<DomainError> onFailure) where TValue : notnull
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
                throw InvariantViolationException.Create("Result.Match.Exception", "An exception occurred while executing the match actions.", ex);
            }
        }

        /// <summary>
        /// Projects a <see cref="Result{TValue}"/> into a value asynchronously by invoking the appropriate handler.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the projected value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccess">The asynchronous handler invoked when <paramref name="result"/> is successful. Must not be <see langword="null"/>.</param>
        /// <param name="onFailure">The asynchronous handler invoked when <paramref name="result"/> is a failure. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains the value produced by the invoked handler.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onSuccess"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onFailure"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when either handler throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<TOut> MatchAsync<TValue, TOut>(this Result<TValue> result, Func<TValue, Task<TOut>> onSuccess, Func<DomainError, Task<TOut>> onFailure) where TValue : notnull
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
                throw InvariantViolationException.Create("Result.MatchAsync.Exception", "An exception occurred while executing the match functions asynchronously.", ex);
            }
        }

        /// <summary>
        /// Invokes the appropriate asynchronous action based on the state of the result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="onSuccess">The asynchronous action invoked when <paramref name="result"/> is successful. Must not be <see langword="null"/>.</param>
        /// <param name="onFailure">The asynchronous action invoked when <paramref name="result"/> is a failure. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onSuccess"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="onFailure"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when either action throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask MatchAsync<TValue>(this Result<TValue> result, Func<TValue, Task> onSuccess, Func<DomainError, Task> onFailure) where TValue : notnull
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
                throw InvariantViolationException.Create("Result.MatchAsync.Exception", "An exception occurred while executing the match actions asynchronously.", ex);
            }
        }

        #endregion

        #region Flatten / Sequence / Traverse

        /// <summary>
        /// Flattens a nested <see cref="Result{TValue}"/> that contains another <see cref="Result{TValue}"/> into a single-level result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The nested result to flatten.</param>
        /// <returns>The inner result if <paramref name="result"/> is successful; otherwise, a failure containing the outer error.</returns>
        public static Result<TValue> Flatten<TValue>(this Result<Result<TValue>> result) where TValue : notnull
            => result.IsFailure
                ? Result.Failure<TValue>(result.Error)
                : result.Value;

        /// <summary>
        /// Flattens a nested result asynchronously.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="resultTask">The task producing the nested <see cref="Result{TValue}"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains the flattened result.</returns>
        public static async ValueTask<Result<TValue>> FlattenAsync<TValue>(this ValueTask<Result<Result<TValue>>> resultTask) where TValue : notnull
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.IsFailure
                ? Result.Failure<TValue>(result.Error)
                : result.Value;
        }

        /// <summary>
        /// Converts a sequence of results into a result containing a read-only list, failing fast on the first error.
        /// </summary>
        /// <typeparam name="TValue">The type of the values. Must be non-nullable.</typeparam>
        /// <param name="results">The sequence of results to evaluate. Must not be <see langword="null"/>.</param>
        /// <returns>A success result containing all values if all results succeed; otherwise, the first encountered failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="results"/> is <see langword="null"/>.</exception>
        public static Result<IReadOnlyList<TValue>> Sequence<TValue>(this IEnumerable<Result<TValue>> results) where TValue : notnull
        {
            Invariant.That(results is not null, "Result.Sequence.ParameterCannotBeNull", "The results sequence cannot be null.");

            var list = new List<TValue>();
            foreach (var result in results!)
            {
                if (result.IsFailure)
                    return Result.Failure<IReadOnlyList<TValue>>(result.Error);

                list.Add(result.Value);
            }

            return Result.Success<IReadOnlyList<TValue>>(list);
        }

        /// <summary>
        /// Converts a sequence of result tasks into a result containing a read-only list, failing fast on the first error.
        /// </summary>
        /// <typeparam name="TValue">The type of the values. Must be non-nullable.</typeparam>
        /// <param name="resultTasks">The sequence of result tasks to evaluate. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains aggregated values or the first failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="resultTasks"/> is <see langword="null"/>.</exception>
        public static async ValueTask<Result<IReadOnlyList<TValue>>> SequenceAsync<TValue>(this IEnumerable<Task<Result<TValue>>> resultTasks) where TValue : notnull
        {
            Invariant.That(resultTasks is not null, "Result.Sequence.ParameterCannotBeNull", "The results sequence cannot be null.");

            var list = new List<TValue>();

            foreach (var resultTask in resultTasks!)
            {
                var result = await resultTask.ConfigureAwait(false);
                if (result.IsFailure)
                    return Result.Failure<IReadOnlyList<TValue>>(result.Error);
                list.Add(result.Value);
            }

            return Result.Success<IReadOnlyList<TValue>>(list);
        }

        /// <summary>
        /// Projects each element of a sequence into a result and aggregates successful values, failing fast on the first error.
        /// </summary>
        /// <typeparam name="TIn">The type of the source elements. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the projected values. Must be non-nullable.</typeparam>
        /// <param name="source">The source sequence. Must not be <see langword="null"/>.</param>
        /// <param name="selector">The projection function that produces a result for each element. Must not be <see langword="null"/>.</param>
        /// <returns>A success result containing all projected values if all succeed; otherwise, the first encountered failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="selector"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="selector"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<IReadOnlyList<TOut>> Traverse<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, Result<TOut>> selector)
            where TIn : notnull
            where TOut : notnull
        {
            Invariant.That(selector is not null, "Result.Traverse.ParameterCannotBeNull", "The selector function cannot be null.");
            Invariant.That(source is not null, "Result.Traverse.ParameterCannotBeNull", "The source sequence cannot be null.");

            var list = new List<TOut>();

            foreach (var item in source!)
            {
                try
                {
                    var result = selector!(item);
                    if (result.IsFailure)
                        return Result.Failure<IReadOnlyList<TOut>>(result.Error);

                    list.Add(result.Value);
                }
                catch (Exception ex)
                {
                    throw InvariantViolationException.Create("Result.Traverse.Exception", "An exception occurred while traversing the sequence.", ex);
                }
            }

            return Result.Success<IReadOnlyList<TOut>>(list);
        }

        /// <summary>
        /// Projects each element of a sequence into a result asynchronously and aggregates successful values, failing fast on the first error.
        /// </summary>
        /// <typeparam name="TIn">The type of the source elements. Must be non-nullable.</typeparam>
        /// <typeparam name="TOut">The type of the projected values. Must be non-nullable.</typeparam>
        /// <param name="source">The source sequence. Must not be <see langword="null"/>.</param>
        /// <param name="selector">The asynchronous projection function that produces a result for each element. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains aggregated values or the first failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="selector"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="selector"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<Result<IReadOnlyList<TOut>>> TraverseAsync<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, Task<Result<TOut>>> selector)
            where TIn : notnull
            where TOut : notnull
        {
            Invariant.That(source is not null, "Result.Traverse.ParameterCannotBeNull", "The source sequence cannot be null.");
            Invariant.That(selector is not null, "Result.Traverse.ParameterCannotBeNull", "The selector function cannot be null.");

            var list = new List<TOut>();
            foreach (var item in source!)
            {
                try
                {
                    var result = await selector!(item).ConfigureAwait(false);
                    if (result.IsFailure)
                        return Result.Failure<IReadOnlyList<TOut>>(result.Error);

                    list.Add(result.Value);
                }
                catch (Exception ex)
                {
                    throw InvariantViolationException.Create("Result.TraverseAsync.Exception", "An exception occurred while traversing the sequence asynchronously.", ex);
                }
            }

            return Result.Success<IReadOnlyList<TOut>>(list);
        }

        #endregion

        #region Try / Catch / Finally

        /// <summary>
        /// Executes the specified function and captures any thrown exception as a failure result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="func">The function to execute. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The factory that produces an error from the caught exception. Must not be <see langword="null"/>.</param>
        /// <returns>A success result containing the function value if no exception occurs; otherwise, a failure result.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="func"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        public static Result<TValue> TryCatch<TValue>(Func<TValue> func, Func<Exception, DomainError> errorFactory) where TValue : notnull
        {
            Invariant.That(func is not null, "Result.TryCatch.ParameterCannotBeNull", "The function cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatch.ParameterCannotBeNull", "The error factory function cannot be null.");

            try
            {
                return Result.Success(func!());
            }
            catch (Exception ex)
            {
                return Result.Failure<TValue>(errorFactory!(ex));
            }
        }

        /// <summary>
        /// Executes the specified action and captures any thrown exception as a failure result.
        /// </summary>
        /// <param name="action">The action to execute. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The factory that produces an error from the caught exception. Must not be <see langword="null"/>.</param>
        /// <returns>A success result containing <see cref="Unit"/> if no exception occurs; otherwise, a failure result.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static Result<Unit> TryCatch(Action action, Func<Exception, DomainError> errorFactory)
        {
            Invariant.That(action is not null, "Result.TryCatch.ParameterCannotBeNull", "The action cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatch.ParameterCannotBeNull", "The error factory function cannot be null.");

            try
            {
                action!();
                return Result.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                try
                {
                    return Result.Failure<Unit>(errorFactory!(ex));
                }
                catch (Exception innerEx)
                {
                    throw InvariantViolationException.Create("Result.TryCatch.Exception", "An exception occurred while executing the error factory.", innerEx);
                }
            }
        }

        /// <summary>
        /// Executes the specified asynchronous function and captures any thrown exception as a failure result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="func">The asynchronous function to execute. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The factory that produces an error from the caught exception. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains success or captured failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="func"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<Result<TValue>> TryCatchAsync<TValue>(Func<Task<TValue>> func, Func<Exception, DomainError> errorFactory) where TValue : notnull
        {
            Invariant.That(func is not null, "Result.TryCatchAsync.Func.CannotBeNull", "The function to execute cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatchAsync.ErrorFactory.CannotBeNull", "The error factory cannot be null.");

            try
            {
                var value = await func!().ConfigureAwait(false);
                return Result.Success(value);
            }
            catch (Exception ex)
            {
                try
                {
                    return Result.Failure<TValue>(errorFactory!(ex));
                }
                catch (Exception innerEx)
                {
                    throw InvariantViolationException.Create("Result.TryCatchAsync.Exception", "An exception occurred while executing the error factory.", innerEx);
                }
            }
        }

        /// <summary>
        /// Executes the specified asynchronous action and captures any thrown exception as a failure result.
        /// </summary>
        /// <param name="action">The asynchronous action to execute. Must not be <see langword="null"/>.</param>
        /// <param name="errorFactory">The factory that produces an error from the caught exception. Must not be <see langword="null"/>.</param>
        /// <returns>A value task that represents the asynchronous operation. The result contains success or captured failure.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="errorFactory"/> throws an exception. The original exception is included as the inner exception.</exception>
        public static async ValueTask<Result<Unit>> TryCatchAsync(Func<Task> action, Func<Exception, DomainError> errorFactory)
        {
            Invariant.That(action is not null, "Result.TryCatchAsync.Action.CannotBeNull", "The action to execute cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatchAsync.ErrorFactory.CannotBeNull", "The error factory cannot be null.");

            try
            {
                await action!().ConfigureAwait(false);
                return Result.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                try
                {
                    return Result.Failure<Unit>(errorFactory!(ex));
                }
                catch (Exception innerEx)
                {
                    throw InvariantViolationException.Create("Result.TryCatchAsync.Exception", "An exception occurred while executing the error factory.", innerEx);
                }
            }
        }

        #endregion

        #region Conversions / Utilities

        /// <summary>
        /// Wraps the result in a completed <see cref="Task{TResult}"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <returns>A completed task containing <paramref name="result"/>.</returns>
        public static Task<Result<TValue>> AsTask<TValue>(this Result<TValue> result) where TValue : notnull
            => Task.FromResult(result);

        /// <summary>
        /// Wraps the result in a completed <see cref="ValueTask{TResult}"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <returns>A completed value task containing <paramref name="result"/>.</returns>
        public static ValueTask<Result<TValue>> AsValueTask<TValue>(this Result<TValue> result) where TValue : notnull
            => new(result);

        /// <summary>
        /// Converts a result using <see cref="DomainError"/> to a result with a generic error type by mapping the error.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <typeparam name="TError">The type of the target error. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <param name="mapError">The function that maps the <see cref="DomainError"/> to <typeparamref name="TError"/>. Must not be <see langword="null"/>.</param>
        /// <returns>A success result if <paramref name="result"/> is successful; otherwise, a failure containing the mapped error.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="mapError"/> is <see langword="null"/>.</exception>
        public static Result<TValue, TError> ToGenericError<TValue, TError>(this Result<TValue> result, Func<DomainError, TError> mapError)
            where TValue : notnull
            where TError : notnull
        {
            Invariant.That(mapError is not null, "Result.MapError.MapErrorCannotBeNull", "The error mapping function cannot be null.");

            if (result.IsSuccess)
                return Result.Success<TValue, TError>(result.Value);

            var mappedError = mapError!(result.Error);
            return Result.Failure<TValue, TError>(mappedError);
        }

        /// <summary>
        /// Returns the success value of the result, throwing if the result is a failure.
        /// </summary>
        /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
        /// <param name="result">The source result.</param>
        /// <returns>The success value contained in <paramref name="result"/>.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="result"/> is in a failure state.</exception>
        public static TValue Unwrap<TValue>(this Result<TValue> result) where TValue : notnull
        {
            if (result.IsFailure)
            {
                throw InvariantViolationException.Create("Result.Unwrap.Failure", "Attempted to unwrap a Result that is in a failure state.");
            }

            return result.Value;
        }

        #endregion
    }
}
