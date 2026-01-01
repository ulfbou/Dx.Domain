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

// src/Dx.Domain/Results/ResultExtensions.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dx.Domain
{
    /// <summary>
    /// Provides extension methods for composing, transforming, and handling results in a functional style. These
    /// methods enable fluent chaining, error propagation, recovery, and pattern matching for operations that return
    /// Result types, supporting both synchronous and asynchronous workflows.
    /// </summary>
    /// <remarks>The ResultExtensions class offers a comprehensive set of utilities for working with Result
    /// and related types, including mapping, binding, validation, error recovery, pattern matching, and conversion.
    /// Methods are designed to facilitate functional programming patterns, improve error handling consistency, and
    /// reduce boilerplate code when dealing with operations that may fail. All methods propagate errors automatically
    /// and avoid invoking user-provided delegates when the input result is in a failure state. Thread safety and side
    /// effects depend on the delegates supplied by the caller. For asynchronous scenarios, ValueTask and Task-based
    /// overloads are provided to support efficient composition.</remarks>
    public static class ResultExtensions
    {
        #region Map

        /// <summary>
        /// Transforms the successful result value to a new type using the specified mapping function.
        /// </summary>
        /// <remarks>This method enables fluent chaining of operations on results, allowing transformation
        /// of the contained value only when the result is successful. If the input result is a failure, the mapping
        /// function is not called and the error is preserved.</remarks>
        /// <typeparam name="TIn">The type of the value contained in the input result.</typeparam>
        /// <typeparam name="TOut">The type of the value to return in the mapped result.</typeparam>
        /// <param name="result">The result to map. If the result represents a failure, the error is propagated without invoking the mapping
        /// function.</param>
        /// <param name="mapFunc">A function to apply to the value of a successful result. Cannot be null.</param>
        /// <returns>A new result containing the mapped value if the input result is successful; otherwise, a failure result with
        /// the original error.</returns>
        public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapFunc)
        {
            DxDomain.Invariant.That(mapFunc is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(mapFunc)}' cannot be null."));
            return result.IsFailure
                ? DxDomain.Result.Failure<TOut>(result.Error)
                : DxDomain.Result.Ok(mapFunc(result.Value));
        }

        /// <summary>
        /// Asynchronously transforms the successful value of the result using the specified mapping function.
        /// </summary>
        /// <remarks>If the input result is a failure, the mapping function is not called and the returned
        /// result preserves the error. The mapping function is invoked only for successful results.</remarks>
        /// <typeparam name="TIn">The type of the input value contained in the result. Must not be null.</typeparam>
        /// <typeparam name="TOut">The type of the output value produced by the mapping function. Must not be null.</typeparam>
        /// <param name="result">The result to be mapped. If the result represents a failure, the mapping function is not invoked.</param>
        /// <param name="map">A function that asynchronously maps the input value to an output value. Cannot be null.</param>
        /// <returns>A result containing the mapped output value if the input result is successful; otherwise, a failure result
        /// with the original error.</returns>
        public static async ValueTask<Result<TOut>> MapAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<TOut>> map)
            where TIn : notnull
            where TOut : notnull
        {
            DxDomain.Invariant.That(map is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(map)}' cannot be null."));

            if (result.IsFailure)
            {
                return DxDomain.Result.Failure<TOut>(result.Error);
            }

            var mappedValue = await map(result.Value).ConfigureAwait(false);
            return DxDomain.Result.Ok(mappedValue);
        }
        #endregion

        #region Bind

        /// <summary>
        /// Invokes the specified binding function on the successful result value, returning a new result of the
        /// specified output type.
        /// </summary>
        /// <remarks>This method enables chaining of operations that return results, propagating errors
        /// without invoking the binding function if a failure has occurred. Commonly used to compose multiple
        /// result-producing operations in a functional style.</remarks>
        /// <typeparam name="TIn">The type of the value contained in the input result.</typeparam>
        /// <typeparam name="TOut">The type of the value contained in the output result.</typeparam>
        /// <param name="result">The input result to bind. If the result represents a failure, the binding function is not invoked.</param>
        /// <param name="bindFunc">A function to apply to the value of the input result if it is successful. Cannot be null.</param>
        /// <returns>A result containing the value returned by the binding function if the input result is successful; otherwise,
        /// a failure result containing the original error.</returns>
        public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bindFunc)
        {
            DxDomain.Invariant.That(bindFunc is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(bindFunc)}' cannot be null."));

            return result.IsFailure
                ? DxDomain.Result.Failure<TOut>(result.Error)
                : bindFunc(result.Value);
        }

        /// <summary>
        /// Asynchronously applies the specified binding function to the successful result value, returning a new result
        /// that represents the outcome of the operation.
        /// </summary>
        /// <remarks>If the input result is a failure, the returned result will contain the same error and
        /// the binding function will not be called. This method enables chaining asynchronous operations that return
        /// results, propagating errors automatically.</remarks>
        /// <typeparam name="TIn">The type of the value contained in the input result. Must not be null.</typeparam>
        /// <typeparam name="TOut">The type of the value contained in the output result. Must not be null.</typeparam>
        /// <param name="result">The input result to bind. If the result represents a failure, the binding function is not invoked and the
        /// failure is propagated.</param>
        /// <param name="bindFunc">A function that takes the successful result value and returns a task producing a new result. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous binding operation. The result contains either the output value or
        /// the propagated error.</returns>
        public static async ValueTask<Result<TOut>> BindAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> bindFunc)
            where TIn : notnull
            where TOut : notnull
        {
            DxDomain.Invariant.That(bindFunc is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(bindFunc)}' cannot be null."));

            if (result.IsFailure)
            {
                return DxDomain.Result.Failure<TOut>(result.Error);
            }

            var boundResult = await bindFunc(result.Value).ConfigureAwait(false);
            return boundResult;
        }

        #endregion

        #region Tap
        /// <summary>
        /// Invokes an action for side-effects when the result is successful, returning the original result.
        /// This is useful for logging, metrics, or other non-mutating side effects.
        /// </summary>
        /// <typeparam name="T">Type of the result value.</typeparam>
        /// <param name="result">The result to inspect.</param>
        /// <param name="action">Action to invoke when the result is successful.</param>
        /// <returns>The original <paramref name="result"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> or <paramref name="action"/> is <see langword="null"/>.</exception>
        public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
            where T : notnull
        {
            if (result.IsSuccess)
            {
                action(result.Value);
            }

            return result;
        }

        public static async ValueTask<Result<T>> TapAsync<T>(this Result<T> result, Func<T, Task> action)
            where T : notnull
        {
            DxDomain.Invariant.That(action is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(action)}' cannot be null."));

            if (result.IsSuccess)
            {
                await action(result.Value).ConfigureAwait(false);
            }

            return result;
        }

        #endregion

        #region Ensure

        /// <summary>
        /// Ensures that the result value satisfies the specified predicate, returning a failure result with the
        /// provided error if the predicate is not met.
        /// </summary>
        /// <remarks>Use this method to enforce additional domain constraints on a successful result. If
        /// the predicate is not satisfied, the result is converted to a failure with the specified domain
        /// error.</remarks>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <param name="result">The result to validate against the predicate.</param>
        /// <param name="predicate">A function that defines the condition to check against the result value. Cannot be null.</param>
        /// <param name="error">The error to return if the predicate is not satisfied.</param>
        /// <returns>A successful result if the original result is successful and the predicate returns <see langword="true"/>;
        /// otherwise, a failure result with the specified error. If the original result is already a failure, it is
        /// returned unchanged.</returns>
        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, DomainError error) where T : notnull
        {
            DxDomain.Invariant.That(predicate is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(predicate)}' cannot be null."));

            return result.IsFailure
                ? result
                : predicate(result.Value)
                    ? result
                    : DxDomain.Result.Failure<T>(error);
        }

        /// <summary>
        /// Ensures that the result satisfies the specified predicate, returning a failure if the condition is not met.
        /// </summary>
        /// <remarks>Use this method to enforce additional business rules or invariants on a successful
        /// result. If the result is already a failure, it is returned unchanged.</remarks>
        /// <typeparam name="T">The type of the value contained in the result. Must be non-null.</typeparam>
        /// <param name="result">The result to validate against the predicate.</param>
        /// <param name="predicate">A function that determines whether the result's value meets the required condition. Cannot be null.</param>
        /// <param name="errorFactory">A function that produces a domain error to associate with the failure if the predicate is not satisfied.
        /// Cannot be null.</param>
        /// <returns>The original result if it is already a failure or if the predicate returns true; otherwise, a failure result
        /// containing the error produced by the error factory.</returns>
        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Func<DomainError> errorFactory) where T : notnull
        {
            DxDomain.Invariant.That(predicate is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(predicate)}' cannot be null."));
            DxDomain.Invariant.That(errorFactory is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(errorFactory)}' cannot be null."));

            return result.IsFailure
                ? result
                : predicate(result.Value)
                    ? result
                    : DxDomain.Result.Failure<T>(errorFactory());
        }

        /// <summary>
        /// Asynchronously ensures that the result value satisfies the specified predicate, returning a failure result
        /// with the provided error if the predicate is not met.
        /// </summary>
        /// <remarks>If the original result is a failure, the predicate is not evaluated and the failure
        /// is returned immediately.</remarks>
        /// <typeparam name="T">The type of the value contained in the result. Must not be null.</typeparam>
        /// <param name="result">The result to validate against the predicate.</param>
        /// <param name="predicate">A function that asynchronously evaluates the value contained in the result. Must not be null.</param>
        /// <param name="error">The error to associate with the result if the predicate is not satisfied.</param>
        /// <returns>A successful result if the original result is successful and the predicate returns <see langword="true"/>;
        /// otherwise, a failure result containing the specified error.</returns>
        public static async ValueTask<Result<T>> EnsureAsync<T>(this Result<T> result, Func<T, Task<bool>> predicate, DomainError error) where T : notnull
        {
            DxDomain.Invariant.That(predicate is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(predicate)}' cannot be null."));

            return result.IsFailure
                ? result
                : await predicate(result.Value).ConfigureAwait(false)
                    ? result
                    : DxDomain.Result.Failure<T>(error);
        }

        /// <summary>
        /// Asynchronously ensures that the result value satisfies the specified predicate, returning a failure result
        /// with a custom error if the predicate is not met.
        /// </summary>
        /// <remarks>If the original result is a failure, the predicate and error factory are not
        /// evaluated. This method is useful for chaining additional asynchronous validations on a successful
        /// result.</remarks>
        /// <typeparam name="T">The type of the value contained in the result. Must not be null.</typeparam>
        /// <param name="result">The result to validate against the predicate.</param>
        /// <param name="predicate">An asynchronous function that evaluates the result value and returns <see langword="true"/> if the value
        /// satisfies the condition; otherwise, <see langword="false"/>.</param>
        /// <param name="errorFactory">An asynchronous function that produces a <see cref="DomainError"/> to use if the predicate is not satisfied.</param>
        /// <returns>A <see cref="Result{T}"/> containing the original value if the predicate is satisfied or the original result
        /// is a failure; otherwise, a failure result with the error provided by <paramref name="errorFactory"/>.</returns>
        public static async ValueTask<Result<T>> EnsureAsync<T>(this Result<T> result, Func<T, Task<bool>> predicate, Func<Task<DomainError>> errorFactory) where T : notnull
        {
            DxDomain.Invariant.That(predicate is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(predicate)}' cannot be null."));
            DxDomain.Invariant.That(errorFactory is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(errorFactory)}' cannot be null."));

            return result.IsFailure
                ? result
                : await predicate(result.Value).ConfigureAwait(false)
                    ? result
                    : DxDomain.Result.Failure<T>(await errorFactory().ConfigureAwait(false));
        }

        #endregion

        #region Recover

        /// <summary>
        /// Returns a successful result by applying a recovery function to the error if the original result represents a
        /// failure; otherwise, returns the original result.
        /// </summary>
        /// <remarks>Use this method to provide a fallback value when a result is unsuccessful, enabling graceful error
        /// handling without throwing exceptions.</remarks>
        /// <typeparam name="T">The type of the value contained in the result. Must be a non-nullable type.</typeparam>
        /// <param name="result">The result to evaluate for failure and potentially recover.</param>
        /// <param name="recoveryFunc">A function that takes a domain error and returns a replacement value to use if recovery is needed. Cannot be null.</param>
        /// <returns>A successful result containing the value returned by <paramref name="recoveryFunc"/> if <paramref name="result"/> is
        /// a failure; otherwise, the original result.</returns>
        public static Result<T> Recover<T>(this Result<T> result, Func<DomainError, T> recoveryFunc) where T : notnull
        {
            DxDomain.Invariant.That(recoveryFunc is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(recoveryFunc)}' cannot be null."));
            return result.IsFailure
                ? DxDomain.Result.Ok(recoveryFunc(result.Error))
                : result;
        }

        /// <summary>
        /// Attempts to recover from a failed result by applying the specified recovery function to the error, returning
        /// a new result if recovery is performed.
        /// </summary>
        /// <remarks>Use this method to provide alternative handling or fallback logic when a result is
        /// unsuccessful. If the result is successful, the recovery function is not invoked.</remarks>
        /// <typeparam name="T">The type of the value contained in the result. Must be non-null.</typeparam>
        /// <param name="result">The result to evaluate for failure and potential recovery.</param>
        /// <param name="recoveryFunc">A function that receives the error from a failed result and returns a new result to recover from the
        /// failure. Cannot be null.</param>
        /// <returns>A new result produced by the recovery function if the original result represents a failure; otherwise, the
        /// original result if it is successful.</returns>
        public static Result<T> Recover<T>(this Result<T> result, Func<DomainError, Result<T>> recoveryFunc) where T : notnull
        {
            DxDomain.Invariant.That(recoveryFunc is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(recoveryFunc)}' cannot be null."));
            return result.IsFailure ? recoveryFunc(result.Error) : result;
        }

        /// <summary>
        /// Attempts to recover from a failed result by invoking an asynchronous recovery function. If the result is
        /// successful, returns the original result; otherwise, returns a new successful result using the value produced
        /// by the recovery function.
        /// </summary>
        /// <remarks>The recovery function is only invoked if the input result represents a failure. This
        /// method enables chaining asynchronous error recovery logic in workflows that use the Result type.</remarks>
        /// <typeparam name="T">The type of the value contained in the result. Must be non-null.</typeparam>
        /// <param name="result">The result to evaluate for failure and potentially recover.</param>
        /// <param name="recoveryFunc">A function that takes a domain error and returns a task producing a replacement value. Cannot be null.</param>
        /// <returns>A successful result containing the original value if the input result is successful; otherwise, a successful
        /// result containing the value returned by the recovery function.</returns>
        public static async ValueTask<Result<T>> RecoverAsync<T>(this Result<T> result, Func<DomainError, Task<T>> recoveryFunc) where T : notnull
        {
            DxDomain.Invariant.That(recoveryFunc is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(recoveryFunc)}' cannot be null."));
            return result.IsFailure ? DxDomain.Result.Ok(await recoveryFunc(result.Error).ConfigureAwait(false)) : result;
        }

        /// <summary>
        /// Attempts to recover from a failed result by invoking the specified asynchronous recovery function.
        /// </summary>
        /// <remarks>Use this method to provide custom recovery logic for failed results in asynchronous
        /// workflows. The recovery function is only invoked if the input result indicates failure.</remarks>
        /// <typeparam name="T">The type of the value contained in the result. Must not be null.</typeparam>
        /// <param name="result">The result to evaluate for failure and potentially recover.</param>
        /// <param name="recovery">An asynchronous function that provides a recovery result when the input result is a failure. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. If the input result is a failure, returns the result of
        /// the recovery function; otherwise, returns the original result.</returns>
        public static async ValueTask<Result<T>> RecoverAsync<T>(this Result<T> result, Func<DomainError, Task<Result<T>>> recovery) where T : notnull
        {
            DxDomain.Invariant.That(recovery is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(recovery)}' cannot be null."));
            return result.IsFailure ? await recovery(result.Error).ConfigureAwait(false) : result;
        }

        #endregion

        #region Match

        /// <summary>
        /// Executes one of the specified functions based on whether the result represents success or failure, and
        /// returns the corresponding value.
        /// </summary>
        /// <remarks>This method enables functional-style pattern matching on a Result<T> instance,
        /// allowing callers to handle both success and failure cases in a single expression.</remarks>
        /// <typeparam name="TOut">The type of the value returned by the onSuccess or onFailure function.</typeparam>
        /// <typeparam name="T">The type of the value contained in the result if the operation was successful.</typeparam>
        /// <param name="result">The result to match against, indicating either a successful value or a failure.</param>
        /// <param name="onSuccess">A function to invoke if the result is successful. The function receives the successful value and returns a
        /// value of type TOut. Cannot be null.</param>
        /// <param name="onFailure">A function to invoke if the result represents a failure. The function receives the error and returns a value
        /// of type TOut. Cannot be null.</param>
        /// <returns>The value returned by either the onSuccess function if the result is successful, or the onFailure function
        /// if the result is a failure.</returns>
        public static TOut Match<TOut, T>(this Result<T> result, Func<T, TOut> onSuccess, Func<DomainError, TOut> onFailure)
        {
            DxDomain.Invariant.That(onSuccess is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(onSuccess)}' cannot be null."));
            DxDomain.Invariant.That(onFailure is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(onFailure)}' cannot be null."));

            return result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.Error);
        }

        /// <summary>
        /// Invokes the specified action based on whether the result represents a success or a failure.
        /// </summary>
        /// <remarks>Use this method to handle both success and failure cases in a single, explicit
        /// location, improving code clarity and reducing branching logic. Both actions must be provided; passing null
        /// will result in an exception.</remarks>
        /// <typeparam name="T">The type of the value contained in the result if the operation was successful.</typeparam>
        /// <param name="result">The result to evaluate, which determines whether to invoke the success or failure action.</param>
        /// <param name="onSuccess">The action to execute if the result is successful. The result value is passed as an argument. Cannot be
        /// null.</param>
        /// <param name="onFailure">The action to execute if the result represents a failure. The error is passed as an argument. Cannot be
        /// null.</param>
        public static void Match<T>(this Result<T> result, Action<T> onSuccess, Action<DomainError> onFailure) where T : notnull
        {
            DxDomain.Invariant.That(onSuccess is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(onSuccess)}' cannot be null."));
            DxDomain.Invariant.That(onFailure is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(onFailure)}' cannot be null."));

            if (result.IsSuccess)
            {
                onSuccess(result.Value);
            }
            else
            {
                onFailure(result.Error);
            }
        }

        /// <summary>
        /// Asynchronously executes one of the specified functions based on whether the result represents success or
        /// failure.
        /// </summary>
        /// <remarks>This method provides a convenient way to handle both success and failure cases in an
        /// asynchronous workflow. The appropriate function is invoked based on the state of the result, allowing for
        /// distinct handling of each case.</remarks>
        /// <typeparam name="T">The type of the value contained in the result. Must be non-null.</typeparam>
        /// <typeparam name="TOut">The type of the value returned by the success or failure function.</typeparam>
        /// <param name="result">The result to match against, indicating either a successful value or a failure.</param>
        /// <param name="onSuccess">A function to execute if the result is successful. Receives the successful value and returns a task that
        /// produces the output value. Cannot be null.</param>
        /// <param name="onFailure">A function to execute if the result is a failure. Receives the error and returns a task that produces the
        /// output value. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the value returned by either the
        /// success or failure function, depending on the state of the result.</returns>
        public static async ValueTask<TOut> MatchAsync<T, TOut>(this Result<T> result, Func<T, Task<TOut>> onSuccess, Func<DomainError, Task<TOut>> onFailure) where T : notnull
        {
            DxDomain.Invariant.That(onSuccess is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(onSuccess)}' cannot be null."));
            DxDomain.Invariant.That(onFailure is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(onFailure)}' cannot be null."));

            return result.IsSuccess
                ? await onSuccess(result.Value).ConfigureAwait(false)
                : await onFailure(result.Error).ConfigureAwait(false);
        }

        /// <summary>
        /// Invokes the specified asynchronous delegate based on whether the result represents success or failure.
        /// </summary>
        /// <remarks>This method provides a convenient way to handle both success and failure cases of a
        /// result in an asynchronous manner. Only one of the delegates will be invoked, depending on the state of the
        /// result.</remarks>
        /// <typeparam name="T">The type of the value contained in the result. Must not be null.</typeparam>
        /// <param name="result">The result to match against, indicating either a successful value or a failure.</param>
        /// <param name="onSuccess">The asynchronous delegate to invoke if the result is successful. Receives the value contained in the result.
        /// Cannot be null.</param>
        /// <param name="onFailure">The asynchronous delegate to invoke if the result represents a failure. Receives the associated domain
        /// error. Cannot be null.</param>
        /// <returns>A ValueTask that represents the asynchronous operation of invoking the appropriate delegate.</returns>
        public static async ValueTask MatchAsync<T>(this Result<T> result, Func<T, Task> onSuccess, Func<DomainError, Task> onFailure) where T : notnull
        {
            DxDomain.Invariant.That(onSuccess is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(onSuccess)}' cannot be null."));
            DxDomain.Invariant.That(onFailure is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(onFailure)}' cannot be null."));

            if (result.IsSuccess)
                await onSuccess(result.Value).ConfigureAwait(false);
            else
                await onFailure(result.Error).ConfigureAwait(false);
        }

        #endregion

        #region Flatten / Sequence / Traverse

        /// <summary>
        /// Converts a nested result into a single result by unwrapping one level of containment. If the outer result is
        /// a failure, returns a failure result with the same error; otherwise, returns the inner result.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result. Must be a non-nullable type.</typeparam>
        /// <param name="result">The nested result to flatten. If this result is a failure, its error will be propagated.</param>
        /// <returns>A result containing the value of type T if both the outer and inner results are successful; otherwise, a
        /// failure result with the appropriate error.</returns>
        public static Result<T> Flatten<T>(this Result<Result<T>> result) where T : notnull
            => result.IsFailure
                ? DxDomain.Result.Failure<T>(result.Error)
                : result.Value;

        /// <summary>
        /// Asynchronously flattens a nested result by converting a ValueTask containing a Result of Result into a
        /// single Result. This simplifies handling of operations that may produce nested results in asynchronous
        /// workflows.
        /// </summary>
        /// <remarks>Use this method to streamline error handling and result propagation when working with
        /// asynchronous operations that may produce nested results. This is particularly useful in scenarios where
        /// multiple layers of result wrapping can occur, such as when composing asynchronous methods that each return a
        /// Result.</remarks>
        /// <typeparam name="T">The type of the value contained in the result. Must not be null.</typeparam>
        /// <param name="resultTask">A ValueTask that, when awaited, yields a Result containing another Result of type T. The outer result may
        /// represent either a success or a failure.</param>
        /// <returns>A ValueTask that yields a Result of type T. If the outer result is a failure, the returned result will
        /// contain the same error; otherwise, the inner result is returned.</returns>
        public static async ValueTask<Result<T>> FlattenAsync<T>(this ValueTask<Result<Result<T>>> resultTask) where T : notnull
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.IsFailure
                ? DxDomain.Result.Failure<T>(result.Error)
                : result.Value;
        }

        /// <summary>
        /// Aggregates a sequence of results into a single result containing a list of successful values, or returns the
        /// first failure encountered.
        /// </summary>
        /// <remarks>If any result in the sequence represents a failure, the returned result will contain
        /// the error from the first failure and no values. The returned list preserves the order of the input
        /// sequence.</remarks>
        /// <typeparam name="T">The type of the value contained in each result. Must be non-null.</typeparam>
        /// <param name="results">The sequence of results to aggregate. Cannot be null.</param>
        /// <returns>A successful result containing a read-only list of values if all results are successful; otherwise, a
        /// failure result containing the error from the first failed result.</returns>
        public static Result<IReadOnlyList<T>> Sequence<T>(this IEnumerable<Result<T>> results) where T : notnull
        {
            DxDomain.Invariant.That(results is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(results)}' cannot be null."));

            if (results.Any(result => result.IsFailure))
                return DxDomain.Result.Failure<IReadOnlyList<T>>(results.First(result => result.IsFailure).Error);

            List<T> list = new();
            foreach (var result in results)
            {
                list.Add(result.Value);
            }

            return DxDomain.Result.Ok<IReadOnlyList<T>>(list);
        }

        /// <summary>
        /// Asynchronously evaluates a sequence of tasks that each yield a result, and returns a result containing a
        /// list of all successful values, or the first error encountered.
        /// </summary>
        /// <remarks>If any task in the sequence yields a failure result, the method returns immediately
        /// with that error and does not evaluate subsequent tasks. The order of values in the returned list matches the
        /// order of the input sequence.</remarks>
        /// <typeparam name="T">The type of the value contained in each result. Must not be null.</typeparam>
        /// <param name="resultTasks">A sequence of tasks that each produce a result of type <typeparamref name="T"/>. Cannot be null.</param>
        /// <returns>A result containing a read-only list of all values if every task completes successfully; otherwise, a
        /// failure result containing the error from the first failed task.</returns>
        public static async ValueTask<Result<IReadOnlyList<T>>> SequenceAsync<T>(this IEnumerable<Task<Result<T>>> resultTasks) where T : notnull
        {
            DxDomain.Invariant.That(resultTasks is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(resultTasks)}' cannot be null."));

            var list = new List<T>();

            foreach (var resultTask in resultTasks)
            {
                var result = await resultTask.ConfigureAwait(false);
                if (result.IsFailure)
                    return DxDomain.Result.Failure<IReadOnlyList<T>>(result.Error);
                list.Add(result.Value);
            }
            return DxDomain.Result.Ok<IReadOnlyList<T>>(list);
        }

        /// <summary>
        /// Applies the specified selector function to each element of the source sequence, returning a result
        /// containing a list of all successful outputs or a failure if any element yields a failure.
        /// </summary>
        /// <remarks>If any invocation of the selector function returns a failure, traversal stops and the
        /// failure is returned immediately. The output list preserves the order of the source sequence.</remarks>
        /// <typeparam name="TIn">The type of elements in the source sequence. Must not be null.</typeparam>
        /// <typeparam name="TOut">The type of the result produced by the selector function. Must not be null.</typeparam>
        /// <param name="source">The sequence of input elements to traverse. Cannot be null.</param>
        /// <param name="selector">A function that transforms each input element into a result. Cannot be null.</param>
        /// <returns>A result containing a read-only list of transformed elements if all selector invocations succeed; otherwise,
        /// a failure result containing the first encountered error.</returns>
        public static Result<IReadOnlyList<TOut>> Traverse<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, Result<TOut>> selector)
            where TIn : notnull
            where TOut : notnull
        {
            DxDomain.Invariant.That(selector is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(selector)}' cannot be null."));
            DxDomain.Invariant.That(source is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(source)}' cannot be null."));

            var list = new List<TOut>();

            foreach (var item in source)
            {
                var result = selector(item);
                if (result.IsFailure)
                    return DxDomain.Result.Failure<IReadOnlyList<TOut>>(result.Error);

                list.Add(result.Value);
            }

            return DxDomain.Result.Ok<IReadOnlyList<TOut>>(list);
        }

        /// <summary>
        /// Asynchronously projects each element of the source sequence into a result using the specified selector
        /// function, and returns a combined result containing all successful projections or the first encountered
        /// error.
        /// </summary>
        /// <remarks>If any invocation of the selector function returns a failure result, traversal stops
        /// and the returned result contains that error. Otherwise, the result contains all projected values in the
        /// order of the source sequence.</remarks>
        /// <typeparam name="TIn">The type of elements in the source sequence. Must not be null.</typeparam>
        /// <typeparam name="TOut">The type of elements returned by the selector function. Must not be null.</typeparam>
        /// <param name="source">The sequence of input elements to be projected. Cannot be null.</param>
        /// <param name="selector">A function that asynchronously transforms each input element into a result. Cannot be null.</param>
        /// <returns>A ValueTask that, when completed, contains a Result holding a read-only list of all successfully projected
        /// elements, or the first error encountered during projection.</returns>
        public static async ValueTask<Result<IReadOnlyList<TOut>>> TraverseAsync<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, Task<Result<TOut>>> selector)
            where TIn : notnull
            where TOut : notnull
        {
            var list = new List<TOut>();
            foreach (var item in source)
            {
                var result = await selector(item).ConfigureAwait(false);
                if (result.IsFailure)
                    return DxDomain.Result.Failure<IReadOnlyList<TOut>>(result.Error);

                list.Add(result.Value);
            }
            return DxDomain.Result.Ok<IReadOnlyList<TOut>>(list);
        }

        #endregion

        #region Try / Catch / Finally

        /// <summary>
        /// Executes the specified function and returns a successful result if no exception is thrown; otherwise,
        /// returns a failure result containing a domain error produced from the exception.
        /// </summary>
        /// <remarks>This method provides a standardized way to handle exceptions and convert them into
        /// domain errors, enabling consistent error handling throughout the application. Both parameters must not be
        /// null.</remarks>
        /// <typeparam name="T">The type of the value returned by the function and contained in the result. Must be a non-nullable type.</typeparam>
        /// <param name="func">The function to execute. If the function throws an exception, the exception is passed to the error factory.</param>
        /// <param name="errorFactory">A delegate that creates a domain error from an exception. Used to generate the failure result if an
        /// exception occurs.</param>
        /// <returns>A result containing the value returned by the function if successful; otherwise, a failure result containing
        /// the domain error produced by the error factory.</returns>
        public static Result<T> TryCatch<T>(Func<T> func, Func<Exception, DomainError> errorFactory) where T : notnull
        {
            DxDomain.Invariant.That(func is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(func)}' cannot be null."));
            DxDomain.Invariant.That(errorFactory is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(errorFactory)}' cannot be null."));

            try
            {
                return DxDomain.Result.Ok(func());
            }
            catch (Exception ex)
            {
                return DxDomain.Result.Failure<T>(errorFactory(ex));
            }
        }

        /// <summary>
        /// Executes the specified action and returns a successful result if no exception is thrown; otherwise, returns
        /// a failure result containing a domain error produced from the exception.
        /// </summary>
        /// <remarks>Use this method to wrap imperative code in a result type, enabling consistent error
        /// handling and propagation of domain-specific errors. The returned result will contain <see langword="Unit"/>
        /// on success, or a domain error on failure.</remarks>
        /// <param name="action">The action to execute. Cannot be null.</param>
        /// <param name="errorFactory">A function that creates a domain error from an exception. Cannot be null.</param>
        /// <returns>A successful result if the action completes without throwing an exception; otherwise, a failure result
        /// containing the domain error returned by <paramref name="errorFactory"/>.</returns>
        public static Result<Unit> TryCatch(Action action, Func<Exception, DomainError> errorFactory)
        {
            DxDomain.Invariant.That(action is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(action)}' cannot be null."));
            DxDomain.Invariant.That(errorFactory is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(errorFactory)}' cannot be null."));

            try
            {
                action();
                return DxDomain.Result.Ok(Unit.Value);
            }
            catch (Exception ex)
            {
                return DxDomain.Result.Failure<Unit>(errorFactory(ex));
            }
        }

        /// <summary>
        /// Executes the specified asynchronous operation and returns a successful result if it completes without
        /// exception; otherwise, returns a failure result containing a domain error produced from the exception.
        /// </summary>
        /// <remarks>This method provides a standardized way to handle exceptions from asynchronous
        /// operations by converting them into domain errors. The returned result will indicate success or failure,
        /// allowing callers to avoid exception handling in their own code.</remarks>
        /// <typeparam name="T">The type of the value returned by the asynchronous operation. Must not be null.</typeparam>
        /// <param name="func">A function that represents the asynchronous operation to execute. Cannot be null.</param>
        /// <param name="errorFactory">A function that creates a domain error from an exception encountered during execution. Cannot be null.</param>
        /// <returns>A result containing the value returned by the operation if successful; otherwise, a failure result
        /// containing the domain error generated from the exception.</returns>
        public static async ValueTask<Result<T>> TryCatchAsync<T>(Func<Task<T>> func, Func<Exception, DomainError> errorFactory) where T : notnull
        {
            DxDomain.Invariant.That(func is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(func)}' cannot be null."));
            DxDomain.Invariant.That(errorFactory is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(errorFactory)}' cannot be null."));

            try
            {
                return DxDomain.Result.Ok(await func().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return DxDomain.Result.Failure<T>(errorFactory(ex));
            }
        }

        /// <summary>
        /// Executes the specified asynchronous action and returns a result indicating success or failure, capturing any
        /// exception as a domain error.
        /// </summary>
        /// <remarks>This method wraps the execution of an asynchronous action in a try-catch block,
        /// converting any thrown exception into a domain error using the provided factory. The returned result can be
        /// used to handle errors in a functional style without throwing exceptions.</remarks>
        /// <param name="action">The asynchronous operation to execute. Cannot be null.</param>
        /// <param name="errorFactory">A function that creates a domain error from an exception. Cannot be null.</param>
        /// <returns>A result containing <see cref="Unit"/> if the action completes successfully; otherwise, a failure result
        /// containing the domain error produced by <paramref name="errorFactory"/>.</returns>
        public static async ValueTask<Result<Unit>> TryCatchAsync(Func<Task> action, Func<Exception, DomainError> errorFactory)
        {
            DxDomain.Invariant.That(action is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(action)}' cannot be null."));
            DxDomain.Invariant.That(errorFactory is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(errorFactory)}' cannot be null."));

            try
            {
                await action().ConfigureAwait(false);
                return DxDomain.Result.Ok(Unit.Value);
            }
            catch (Exception ex)
            {
                return DxDomain.Result.Failure<Unit>(errorFactory(ex));
            }
        }

        #endregion

        #region Conversions / Utilities

        public static Task<Result<T>> AsTask<T>(this Result<T> result) where T : notnull
            => Task.FromResult(result);

        public static ValueTask<Result<T>> AsValueTask<T>(this Result<T> result) where T : notnull
            => new ValueTask<Result<T>>(result);

        public static Result<T, TError> ToGenericError<T, TError>(this Result<T> result, Func<DomainError, TError> mapError)
            where T : notnull
            where TError : notnull
        {
            DxDomain.Invariant.That(mapError is not null, DxDomain.Faults.InvalidInput($"Parameter '{nameof(mapError)}' cannot be null."));
            return result.IsFailure
                ? DxDomain.Result.Failure<T, TError>(mapError(result.Error))
                : DxDomain.Result.Ok<T, TError>(result.Value);
        }

        public static T Unwrap<T>(this Result<T> result) where T : notnull
        {
            if (result.IsFailure)
                throw InvariantViolationException.Create(
                    DxDomain.Invariant.CreateInvariantError(
                        result.Error,
                        "Attempted to unwrap a Result that is in a failure state."));
            return result.Value;
        }

        #endregion
    }
}
