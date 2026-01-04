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

using static Dx.Domain.DxDomain.Kernel;

namespace Dx.Domain
{
    /// <summary>
    /// Provides extension methods for composing, transforming, and handling <see cref="Result{TValue, TError}"/> instances
    /// with a generic error type. These methods mirror the non-generic <see cref="ResultExtensions"/> counterparts while
    /// preserving the caller-specified error type, enabling fluent, strongly-typed functional workflows across both
    /// synchronous and asynchronous operations.
    /// </summary>
    /// <remarks>
    /// All methods in this class follow the same conventions as <see cref="ResultExtensions"/>:
    /// <list type="bullet">
    /// <item><description>Delegates are validated using <c>DxDomain.Invariant.That</c> and never invoked when the source result is a failure.</description></item>
    /// <item><description>Failures are propagated without modification unless a recovery or error-mapping function is explicitly applied.</description></item>
    /// <item><description>Asynchronous methods use <see cref="Task"/> and configure awaits with <see cref="Task.ConfigureAwait(bool)"/>.</description></item>
    /// <item><description>Invariant violations (such as unwrapping a failure) are surfaced via <see cref="InvariantViolationException"/>.</description></item>
    /// </list>
    /// This specialization is intended for scenarios where the error type is richer or more domain-specific than
    /// <see cref="DomainError"/>, while still allowing seamless conversion back to the kernel error representation when
    /// needed.
    /// </remarks>
    public static class ResultExtensionsGeneric
    {
        #region Map

        /// <summary>
        /// Transforms the successful value of a generic result into a new value using the specified mapping function.
        /// </summary>
        /// <typeparam name="TIn">The type of the input value contained in the result.</typeparam>
        /// <typeparam name="TOut">The type of the output value produced by the mapping function.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to map. If it represents a failure, the mapping function is not invoked and the error is preserved.</param>
        /// <param name="map">A function that transforms the successful value into a new value. Cannot be null.</param>
        /// <returns>A new result containing the mapped value if <paramref name="result"/> is successful; otherwise, a failure
        /// result containing the original error.</returns>
        public static Result<TOut, TError> Map<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TIn, TOut> map)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(map is not null, Faults.Guard.ParameterCannotBeNull(nameof(map)));

            return result.IsFailure
                ? DxDomain.Result.Failure<TOut, TError>(result.Error)
                : DxDomain.Result.Ok<TOut, TError>(map(result.Value));
        }

        /// <summary>
        /// Asynchronously transforms the successful value of a generic result into a new value using the specified
        /// mapping function.
        /// </summary>
        /// <typeparam name="TIn">The type of the input value contained in the result.</typeparam>
        /// <typeparam name="TOut">The type of the output value produced by the mapping function.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to map. If it represents a failure, the mapping function is not invoked and the error is preserved.</param>
        /// <param name="map">An asynchronous function that transforms the successful value into a new value. Cannot be null.</param>
        /// <returns>A task that, when completed, yields a result containing the mapped value if <paramref name="result"/> is
        /// successful; otherwise, a failure result containing the original error.</returns>
        public static async Task<Result<TOut, TError>> MapAsync<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TIn, Task<TOut>> map)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(map is not null, Faults.Guard.ParameterCannotBeNull(nameof(map)));

            if (result.IsFailure)
                return Result<TOut, TError>.InternalFailure(result.Error);

            var mapped = await map(result.Value).ConfigureAwait(false);
            return DxDomain.Result.Ok<TOut, TError>(mapped);
        }

        #endregion

        #region Bind

        /// <summary>
        /// Invokes the specified binding function on the successful value of a generic result, returning the resulting
        /// result instance.
        /// </summary>
        /// <typeparam name="TIn">The type of the value contained in the input result.</typeparam>
        /// <typeparam name="TOut">The type of the value contained in the output result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in both input and output results.</typeparam>
        /// <param name="result">The input result to bind. If it represents a failure, the binding function is not invoked.</param>
        /// <param name="bind">A function that takes the successful value of the result and returns a new result. Cannot be null.</param>
        /// <returns>The result produced by <paramref name="bind"/> if <paramref name="result"/> is successful; otherwise, a failure
        /// result containing the original error.</returns>
        public static Result<TOut, TError> Bind<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TIn, Result<TOut, TError>> bind)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(bind is not null, Faults.Guard.ParameterCannotBeNull(nameof(bind)));

            return result.IsFailure ? DxDomain.Result.Failure<TOut, TError>(result.Error) : bind(result.Value);
        }

        /// <summary>
        /// Asynchronously invokes the specified binding function on the successful value of a generic result, returning
        /// the resulting result instance.
        /// </summary>
        /// <typeparam name="TIn">The type of the value contained in the input result.</typeparam>
        /// <typeparam name="TOut">The type of the value contained in the output result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in both input and output results.</typeparam>
        /// <param name="result">The input result to bind. If it represents a failure, the binding function is not invoked.</param>
        /// <param name="bind">An asynchronous function that takes the successful value of the result and returns a new result.
        /// Cannot be null.</param>
        /// <returns>A task that, when completed, yields the result produced by <paramref name="bind"/> if
        /// <paramref name="result"/> is successful; otherwise, a failure result containing the original error.</returns>
        public static async Task<Result<TOut, TError>> BindAsync<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TIn, Task<Result<TOut, TError>>> bind)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(bind is not null, Faults.Guard.ParameterCannotBeNull(nameof(bind)));

            if (result.IsFailure)
                return DxDomain.Result.Failure<TOut, TError>(result.Error);

            return await bind(result.Value).ConfigureAwait(false);
        }

        #endregion

        #region Tap

        /// <summary>
        /// Invokes the specified action for side effects when the result is successful, returning the original result.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to inspect.</param>
        /// <param name="action">The action to invoke when the result is successful. Cannot be null.</param>
        /// <returns>The original <paramref name="result"/> instance.</returns>
        public static Result<T, TError> Tap<T, TError>(this Result<T, TError> result, Action<T> action)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(action is not null, Faults.Guard.ParameterCannotBeNull(nameof(action)));

            if (!result.IsFailure)
                action(result.Value);

            return result;
        }

        /// <summary>
        /// Asynchronously invokes the specified action for side effects when the result is successful, returning the
        /// original result.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to inspect.</param>
        /// <param name="action">An asynchronous action to invoke when the result is successful. Cannot be null.</param>
        /// <returns>A task that, when completed, yields the original <paramref name="result"/> instance.</returns>
        public static async Task<Result<T, TError>> TapAsync<T, TError>(this Result<T, TError> result, Func<T, Task> action)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(action is not null, Faults.Guard.ParameterCannotBeNull(nameof(action)));

            if (!result.IsFailure)
                await action(result.Value).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region Ensure / Validate

        /// <summary>
        /// Ensures that the successful result value satisfies the specified predicate, returning a failure result with
        /// the provided error if the predicate evaluates to <see langword="false"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to validate.</param>
        /// <param name="predicate">A function that evaluates the result value. Cannot be null.</param>
        /// <param name="error">The error to use when the predicate is not satisfied.</param>
        /// <returns>The original result if it is already a failure or the predicate returns <see langword="true"/>; otherwise,
        /// a failure result with the specified <paramref name="error"/>.</returns>
        public static Result<T, TError> Ensure<T, TError>(this Result<T, TError> result, Func<T, bool> predicate, TError error)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, Faults.Guard.ParameterCannotBeNull(nameof(predicate)));

            if (result.IsFailure)
                return result;

            return predicate(result.Value) ? result : DxDomain.Result.Failure<T, TError>(error);
        }

        /// <summary>
        /// Ensures that the successful result value satisfies the specified predicate, returning a failure result with
        /// an error produced by the given factory if the predicate evaluates to <see langword="false"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to validate.</param>
        /// <param name="predicate">A function that evaluates the result value. Cannot be null.</param>
        /// <param name="errorFactory">A function that produces an error when the predicate is not satisfied. Cannot be null.</param>
        /// <returns>The original result if it is already a failure or the predicate returns <see langword="true"/>; otherwise,
        /// a failure result with the error produced by <paramref name="errorFactory"/>.</returns>
        public static Result<T, TError> Ensure<T, TError>(this Result<T, TError> result, Func<T, bool> predicate, Func<TError> errorFactory)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, Faults.Guard.ParameterCannotBeNull(nameof(predicate)));
            Invariant.That(errorFactory is not null, Faults.Guard.ParameterCannotBeNull(nameof(errorFactory)));

            if (result.IsFailure)
                return result;

            return predicate(result.Value) ? result : DxDomain.Result.Failure<T, TError>(errorFactory());
        }

        /// <summary>
        /// Asynchronously ensures that the successful result value satisfies the specified predicate, returning a
        /// failure result with the provided error if the predicate evaluates to <see langword="false"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to validate.</param>
        /// <param name="predicate">An asynchronous function that evaluates the result value. Cannot be null.</param>
        /// <param name="error">The error to use when the predicate is not satisfied.</param>
        /// <returns>A task that, when completed, yields the original result if it is already a failure or the predicate
        /// returns <see langword="true"/>; otherwise, a failure result with the specified <paramref name="error"/>.</returns>
        public static async Task<Result<T, TError>> EnsureAsync<T, TError>(this Result<T, TError> result, Func<T, Task<bool>> predicate, TError error)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, Faults.Guard.ParameterCannotBeNull(nameof(predicate)));

            if (result.IsFailure)
                return result;

            return await predicate(result.Value).ConfigureAwait(false)
                ? result
                : DxDomain.Result.Failure<T, TError>(error);
        }

        /// <summary>
        /// Asynchronously ensures that the successful result value satisfies the specified predicate, returning a
        /// failure result with an error produced by the given factory if the predicate evaluates to
        /// <see langword="false"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to validate.</param>
        /// <param name="predicate">An asynchronous function that evaluates the result value. Cannot be null.</param>
        /// <param name="errorFactory">An asynchronous function that produces an error when the predicate is not satisfied. Cannot be null.</param>
        /// <returns>A task that, when completed, yields the original result if it is already a failure or the predicate
        /// returns <see langword="true"/>; otherwise, a failure result with the error produced by
        /// <paramref name="errorFactory"/>.</returns>
        public static async Task<Result<T, TError>> EnsureAsync<T, TError>(this Result<T, TError> result, Func<T, Task<bool>> predicate, Func<Task<TError>> errorFactory)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, Faults.Guard.ParameterCannotBeNull(nameof(predicate)));
            Invariant.That(errorFactory is not null, Faults.Guard.ParameterCannotBeNull(nameof(errorFactory)));

            if (result.IsFailure)
                return result;

            return await predicate(result.Value).ConfigureAwait(false)
                ? result
                : DxDomain.Result.Failure<T, TError>(await errorFactory().ConfigureAwait(false));
        }

        #endregion

        #region Recover / Fallback

        /// <summary>
        /// Attempts to recover from a failed result by applying the specified recovery function to the error and
        /// returning a new successful result, or returns the original result if it is already successful.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to recover from if it represents a failure.</param>
        /// <param name="recovery">A function that produces a fallback value from the error. Cannot be null.</param>
        /// <returns>A successful result containing the fallback value if <paramref name="result"/> is a failure; otherwise, the
        /// original result.</returns>
        public static Result<T, TError> Recover<T, TError>(this Result<T, TError> result, Func<TError, T> recovery)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(recovery is not null, Faults.Guard.ParameterCannotBeNull(nameof(recovery)));

            return result.IsFailure ? DxDomain.Result.Ok<T, TError>(recovery(result.Error)) : result;
        }

        /// <summary>
        /// Attempts to recover from a failed result by applying the specified recovery function to the error and
        /// returning the resulting result, or returns the original result if it is already successful.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to recover from if it represents a failure.</param>
        /// <param name="recovery">A function that produces a new result from the error. Cannot be null.</param>
        /// <returns>The result produced by <paramref name="recovery"/> if <paramref name="result"/> is a failure; otherwise, the
        /// original result.</returns>
        public static Result<T, TError> Recover<T, TError>(this Result<T, TError> result, Func<TError, Result<T, TError>> recovery)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(recovery is not null, Faults.Guard.ParameterCannotBeNull(nameof(recovery)));

            return result.IsFailure ? recovery(result.Error) : result;
        }

        /// <summary>
        /// Asynchronously attempts to recover from a failed result by applying the specified recovery function to the
        /// error and returning a new successful result, or returns the original result if it is already successful.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to recover from if it represents a failure.</param>
        /// <param name="recovery">An asynchronous function that produces a fallback value from the error. Cannot be null.</param>
        /// <returns>A task that, when completed, yields a successful result containing the fallback value if
        /// <paramref name="result"/> is a failure; otherwise, the original result.</returns>
        public static async Task<Result<T, TError>> RecoverAsync<T, TError>(this Result<T, TError> result, Func<TError, Task<T>> recovery)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(recovery is not null, Faults.Guard.ParameterCannotBeNull(nameof(recovery)));

            return result.IsFailure
                ? DxDomain.Result.Ok<T, TError>(await recovery(result.Error).ConfigureAwait(false))
                : result;
        }

        /// <summary>
        /// Asynchronously attempts to recover from a failed result by applying the specified recovery function to the
        /// error and returning the resulting result, or returns the original result if it is already successful.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to recover from if it represents a failure.</param>
        /// <param name="recovery">An asynchronous function that produces a new result from the error. Cannot be null.</param>
        /// <returns>A task that, when completed, yields the result produced by <paramref name="recovery"/> if
        /// <paramref name="result"/> is a failure; otherwise, the original result.</returns>
        public static async Task<Result<T, TError>> RecoverAsync<T, TError>(this Result<T, TError> result, Func<TError, Task<Result<T, TError>>> recovery)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(recovery is not null, Faults.Guard.ParameterCannotBeNull(nameof(recovery)));

            return result.IsFailure
                ? await recovery(result.Error).ConfigureAwait(false)
                : result;
        }

        #endregion

        #region Match / Observers

        /// <summary>
        /// Executes one of the specified functions based on whether the result represents success or failure and
        /// returns the corresponding value.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result when successful.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <typeparam name="TOut">The type of the value returned by the handler functions.</typeparam>
        /// <param name="result">The result to match against.</param>
        /// <param name="onSuccess">A function to invoke when the result is successful. Cannot be null.</param>
        /// <param name="onFailure">A function to invoke when the result represents a failure. Cannot be null.</param>
        /// <returns>The value returned by either <paramref name="onSuccess"/> or <paramref name="onFailure"/>, depending on the
        /// state of the result.</returns>
        public static TOut Match<T, TError, TOut>(this Result<T, TError> result, Func<T, TOut> onSuccess, Func<TError, TOut> onFailure)
            where T : notnull
            where TError : notnull
            where TOut : notnull
        {
            Invariant.That(onSuccess is not null, Faults.Guard.ParameterCannotBeNull(nameof(onSuccess)));
            Invariant.That(onFailure is not null, Faults.Guard.ParameterCannotBeNull(nameof(onFailure)));

            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
        }

        /// <summary>
        /// Invokes one of the specified actions based on whether the result represents success or failure.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result when successful.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to match against.</param>
        /// <param name="onSuccess">An action to invoke when the result is successful. Cannot be null.</param>
        /// <param name="onFailure">An action to invoke when the result represents a failure. Cannot be null.</param>
        public static void Match<T, TError>(this Result<T, TError> result, Action<T> onSuccess, Action<TError> onFailure)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(onSuccess is not null, Faults.Guard.ParameterCannotBeNull(nameof(onSuccess)));
            Invariant.That(onFailure is not null, Faults.Guard.ParameterCannotBeNull(nameof(onFailure)));

            if (result.IsSuccess)
                onSuccess(result.Value);
            else
                onFailure(result.Error);
        }

        /// <summary>
        /// Asynchronously executes one of the specified functions based on whether the result represents success or
        /// failure and returns the corresponding value.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result when successful.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <typeparam name="TOut">The type of the value returned by the handler functions.</typeparam>
        /// <param name="result">The result to match against.</param>
        /// <param name="onSuccess">An asynchronous function to invoke when the result is successful. Cannot be null.</param>
        /// <param name="onFailure">An asynchronous function to invoke when the result represents a failure. Cannot be null.</param>
        /// <returns>A task whose result is the value returned by either <paramref name="onSuccess"/> or
        /// <paramref name="onFailure"/>, depending on the state of the result.</returns>
        public static async Task<TOut> MatchAsync<T, TError, TOut>(this Result<T, TError> result, Func<T, Task<TOut>> onSuccess, Func<TError, Task<TOut>> onFailure)
            where T : notnull
            where TError : notnull
            where TOut : notnull
        {
            Invariant.That(onSuccess is not null, Faults.Guard.ParameterCannotBeNull(nameof(onSuccess)));
            Invariant.That(onFailure is not null, Faults.Guard.ParameterCannotBeNull(nameof(onFailure)));

            return result.IsSuccess
                ? await onSuccess(result.Value).ConfigureAwait(false)
                : await onFailure(result.Error).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously invokes one of the specified actions based on whether the result represents success or
        /// failure.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result when successful.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to match against.</param>
        /// <param name="onSuccess">An asynchronous action to invoke when the result is successful. Cannot be null.</param>
        /// <param name="onFailure">An asynchronous action to invoke when the result represents a failure. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous invocation of the appropriate handler.</returns>
        public static async Task MatchAsync<T, TError>(this Result<T, TError> result, Func<T, Task> onSuccess, Func<TError, Task> onFailure)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(onSuccess is not null, Faults.Guard.ParameterCannotBeNull(nameof(onSuccess)));
            Invariant.That(onFailure is not null, Faults.Guard.ParameterCannotBeNull(nameof(onFailure)));

            if (result.IsSuccess)
                await onSuccess(result.Value).ConfigureAwait(false);
            else
                await onFailure(result.Error).ConfigureAwait(false);
        }

        #endregion

        #region Flatten / Sequence / Traverse

        /// <summary>
        /// Flattens a nested result by unwrapping one level of containment, converting a
        /// <c>Result&lt;Result&lt;T, TError&gt;, TError&gt;</c> into a <c>Result&lt;T, TError&gt;</c>.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the inner result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the results.</typeparam>
        /// <param name="result">The nested result to flatten.</param>
        /// <returns>The inner result if the outer result is successful; otherwise, a failure result containing the outer
        /// error.</returns>
        public static Result<T, TError> Flatten<T, TError>(this Result<Result<T, TError>, TError> result)
            where T : notnull
            where TError : notnull
            => result.IsFailure ? DxDomain.Result.Failure<T, TError>(result.Error) : result.Value;

        /// <summary>
        /// Aggregates a sequence of results into a single result containing a list of successful values, or returns the
        /// first failure encountered.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in each result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the results when they represent failures.</typeparam>
        /// <param name="results">The sequence of results to aggregate. Cannot be null.</param>
        /// <returns>A successful result containing a read-only list of values if all results are successful; otherwise, a
        /// failure result containing the error from the first failed result.</returns>
        public static Result<IReadOnlyList<T>, TError> Sequence<T, TError>(this IEnumerable<Result<T, TError>> results)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(results is not null, Faults.Guard.ParameterCannotBeNull(nameof(results)));

            var list = new List<T>();
            foreach (var r in results)
            {
                if (r.IsFailure)
                    return DxDomain.Result.Failure<IReadOnlyList<T>, TError>(r.Error);

                list.Add(r.Value);
            }

            return DxDomain.Result.Ok<IReadOnlyList<T>, TError>(list);
        }

        /// <summary>
        /// Traverses a sequence, applying the specified selector to each element and aggregating the resulting
        /// results into a single result containing a list of successful values, or the first failure encountered.
        /// </summary>
        /// <typeparam name="TIn">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TOut">The type of the value contained in each result produced by the selector.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the results when they represent failures.</typeparam>
        /// <param name="source">The sequence of input elements to traverse. Cannot be null.</param>
        /// <param name="selector">A function that transforms each element into a result. Cannot be null.</param>
        /// <returns>A successful result containing a read-only list of values if all selector invocations succeed;
        /// otherwise, a failure result containing the error from the first failed result.</returns>
        public static Result<IReadOnlyList<TOut>, TError> Traverse<TIn, TOut, TError>(this IEnumerable<TIn> source, Func<TIn, Result<TOut, TError>> selector)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(source is not null, Faults.Guard.ParameterCannotBeNull(nameof(source)));
            Invariant.That(selector is not null, Faults.Guard.ParameterCannotBeNull(nameof(selector)));

            var list = new List<TOut>();
            foreach (var item in source)
            {
                var r = selector(item);
                if (r.IsFailure)
                    return DxDomain.Result.Failure<IReadOnlyList<TOut>, TError>(r.Error);

                list.Add(r.Value);
            }

            return DxDomain.Result.Ok<IReadOnlyList<TOut>, TError>(list);
        }

        /// <summary>
        /// Asynchronously traverses a sequence, applying the specified selector to each element and aggregating the
        /// resulting results into a single result containing a list of successful values, or the first failure
        /// encountered.
        /// </summary>
        /// <typeparam name="TIn">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TOut">The type of the value contained in each result produced by the selector.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the results when they represent failures.</typeparam>
        /// <param name="source">The sequence of input elements to traverse. Cannot be null.</param>
        /// <param name="selector">An asynchronous function that transforms each element into a result. Cannot be null.</param>
        /// <returns>A task that, when completed, yields a successful result containing a read-only list of values if all
        /// selector invocations succeed; otherwise, a failure result containing the error from the first failed
        /// result.</returns>
        public static async Task<Result<IReadOnlyList<TOut>, TError>> TraverseAsync<TIn, TOut, TError>(this IEnumerable<TIn> source, Func<TIn, Task<Result<TOut, TError>>> selector)
            where TIn : notnull
            where TOut : notnull
            where TError : notnull
        {
            Invariant.That(source is not null, Faults.Guard.ParameterCannotBeNull(nameof(source)));
            Invariant.That(selector is not null, Faults.Guard.ParameterCannotBeNull(nameof(selector)));

            var list = new List<TOut>();
            foreach (var item in source)
            {
                var r = await selector(item).ConfigureAwait(false);
                if (r.IsFailure)
                    return DxDomain.Result.Failure<IReadOnlyList<TOut>, TError>(r.Error);

                list.Add(r.Value);
            }

            return DxDomain.Result.Ok<IReadOnlyList<TOut>, TError>(list);
        }

        #endregion

        #region Try/Catch helpers

        /// <summary>
        /// Executes the specified function and returns a successful result if no exception is thrown; otherwise,
        /// returns a failure result containing an error produced from the exception.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the function and contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="func">The function to execute. Cannot be null.</param>
        /// <param name="errorFactory">A function that converts an exception into an error. Cannot be null.</param>
        /// <returns>A successful result containing the value returned by <paramref name="func"/> if it completes without
        /// throwing; otherwise, a failure result containing the error produced by <paramref name="errorFactory"/>.</returns>
        public static Result<T, TError> TryCatch<T, TError>(Func<T> func, Func<Exception, TError> errorFactory)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(func is not null, Faults.Guard.ParameterCannotBeNull(nameof(func)));
            Invariant.That(errorFactory is not null, Faults.Guard.ParameterCannotBeNull(nameof(errorFactory)));

            try
            {
                return DxDomain.Result.Ok<T, TError>(func());
            }
            catch (Exception ex)
            {
                return DxDomain.Result.Failure<T, TError>(errorFactory(ex));
            }
        }

        /// <summary>
        /// Executes the specified action and returns a successful unit result if no exception is thrown; otherwise,
        /// returns a failure result containing an error produced from the exception.
        /// </summary>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="action">The action to execute. Cannot be null.</param>
        /// <param name="errorFactory">A function that converts an exception into an error. Cannot be null.</param>
        /// <returns>A successful result containing <see cref="Unit.Value"/> if the action completes without throwing;
        /// otherwise, a failure result containing the error produced by <paramref name="errorFactory"/>.</returns>
        public static Result<Unit, TError> TryCatch<TError>(Action action, Func<Exception, TError> errorFactory)
            where TError : notnull
        {
            Invariant.That(action is not null, Faults.Guard.ParameterCannotBeNull(nameof(action)));
            Invariant.That(errorFactory is not null, Faults.Guard.ParameterCannotBeNull(nameof(errorFactory)));

            try
            {
                action();
                return DxDomain.Result.Ok<Unit, TError>(Unit.Value);
            }
            catch (Exception ex)
            {
                return DxDomain.Result.Failure<Unit, TError>(errorFactory(ex));
            }
        }

        /// <summary>
        /// Asynchronously executes the specified function and returns a successful result if no exception is thrown;
        /// otherwise, returns a failure result containing an error produced from the exception.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the function and contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="func">The asynchronous function to execute. Cannot be null.</param>
        /// <param name="errorFactory">A function that converts an exception into an error. Cannot be null.</param>
        /// <returns>A task that, when completed, yields a successful result containing the value returned by
        /// <paramref name="func"/> if it completes without throwing; otherwise, a failure result containing the error
        /// produced by <paramref name="errorFactory"/>.</returns>
        public static async Task<Result<T, TError>> TryCatchAsync<T, TError>(Func<Task<T>> func, Func<Exception, TError> errorFactory)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(func is not null, Faults.Guard.ParameterCannotBeNull(nameof(func)));
            Invariant.That(errorFactory is not null, Faults.Guard.ParameterCannotBeNull(nameof(errorFactory)));

            try
            {
                return DxDomain.Result.Ok<T, TError>(await func().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return DxDomain.Result.Failure<T, TError>(errorFactory(ex));
            }
        }

        /// <summary>
        /// Asynchronously executes the specified action and returns a successful unit result if no exception is thrown;
        /// otherwise, returns a failure result containing an error produced from the exception.
        /// </summary>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="action">The asynchronous action to execute. Cannot be null.</param>
        /// <param name="errorFactory">A function that converts an exception into an error. Cannot be null.</param>
        /// <returns>A task that, when completed, yields a successful result containing <see cref="Unit.Value"/> if the
        /// action completes without throwing; otherwise, a failure result containing the error produced by
        /// <paramref name="errorFactory"/>.</returns>
        public static async Task<Result<Unit, TError>> TryCatchAsync<TError>(Func<Task> action, Func<Exception, TError> errorFactory)
            where TError : notnull
        {
            Invariant.That(action is not null, Faults.Guard.ParameterCannotBeNull(nameof(action)));
            Invariant.That(errorFactory is not null, Faults.Guard.ParameterCannotBeNull(nameof(errorFactory)));

            try
            {
                await action().ConfigureAwait(false);
                return DxDomain.Result.Ok<Unit, TError>(Unit.Value);
            }
            catch (Exception ex)
            {
                return DxDomain.Result.Failure<Unit, TError>(errorFactory(ex));
            }
        }

        #endregion

        #region Conversions / Utilities

        /// <summary>
        /// Converts a <see cref="Result{T, TError}"/> into a completed <see cref="Task{TResult}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to wrap.</param>
        /// <returns>A completed task containing the specified <paramref name="result"/>.</returns>
        public static Task<Result<T, TError>> AsTask<T, TError>(this Result<T, TError> result)
            where T : notnull
            where TError : notnull
            => Task.FromResult(result);

        /// <summary>
        /// Converts a <see cref="Result{T, TError}"/> into a completed <see cref="ValueTask{TResult}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to wrap.</param>
        /// <returns>A completed value task containing the specified <paramref name="result"/>.</returns>
        public static ValueTask<Result<T, TError>> AsValueTask<T, TError>(this Result<T, TError> result)
            where T : notnull
            where TError : notnull
            => new ValueTask<Result<T, TError>>(result);

        /// <summary>
        /// Maps the generic error type of a result to the kernel <see cref="DomainError"/> type using the specified
        /// mapping function.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the input result.</typeparam>
        /// <param name="result">The result whose error should be converted.</param>
        /// <param name="mapError">A function that maps the generic error to a <see cref="DomainError"/>. Cannot be null.</param>
        /// <returns>A result containing the same value but with a <see cref="DomainError"/> error type.</returns>
        public static Result<T, DomainError> ToDomainError<T, TError>(this Result<T, TError> result, Func<TError, DomainError> mapError)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(mapError is not null, Faults.Guard.ParameterCannotBeNull(nameof(mapError)));

            return result.IsFailure
                ? DxDomain.Result.Failure<T, DomainError>(mapError(result.Error))
                : DxDomain.Result.Ok<T, DomainError>(result.Value);
        }

        /// <summary>
        /// Maps the error value of a result to a new error type while preserving the success value.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TErrorIn">The type of the error contained in the input result.</typeparam>
        /// <typeparam name="TErrorOut">The type of the error contained in the output result.</typeparam>
        /// <param name="result">The result whose error should be converted.</param>
        /// <param name="mapError">A function that maps the input error to the output error type. Cannot be null.</param>
        /// <returns>A result containing the same value but with the mapped error type.</returns>
        public static Result<T, TErrorOut> MapError<T, TErrorIn, TErrorOut>(this Result<T, TErrorIn> result, Func<TErrorIn, TErrorOut> mapError)
            where T : notnull
            where TErrorIn : notnull
            where TErrorOut : notnull
        {
            Invariant.That(mapError is not null, Faults.Guard.ParameterCannotBeNull(nameof(mapError)));

            return result.IsFailure
                ? DxDomain.Result.Failure<T, TErrorOut>(mapError(result.Error))
                : Result<T, TErrorOut>.Success(result.Value);
        }

        /// <summary>
        /// Returns the contained value or throws an <see cref="InvariantViolationException"/> if the result represents a
        /// failure. Intended for use in scenarios where a failure indicates a programming error rather than a recoverable
        /// business condition.
        ///</summary>
        /// <typeparam name="T">The type of the value contained in the result.</typeparam>
        /// <typeparam name="TError">The type of the error contained in the result when it represents a failure.</typeparam>
        /// <param name="result">The result to unwrap.</param>
        /// <returns>The value contained in the result if it is successful.</returns>
        /// <exception cref="InvariantViolationException">Thrown when <paramref name="result"/> represents a failure.</exception>
        public static T Unwrap<T, TError>(this Result<T, TError> result)
            where T : notnull
            where TError : notnull
        {
            if (result.IsFailure)
            {
                var invariantError = Invariant.CreateInvariantError(
                    Faults.Result.ValueAccessOnFailure<T, TError>(result.Error),
                    "Attempted to unwrap a Result that is in a failure state.");

                throw InvariantViolationException.Create(invariantError);
            }

            return result.Value;
        }

        #endregion
    }
}
