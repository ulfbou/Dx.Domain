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
using Dx.Domain.Kernel;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dx.Domain
{
    /// <summary>
    /// Provides extension methods for composing, transforming, and handling <see cref="Result{TValue, TError}"/> instances
    /// with a generic error type.
    /// </summary>
    public static class ResultExtensionsGeneric
    {
        #region Map

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

        public static Result<T, TError> Tap<T, TError>(this Result<T, TError> result, Action<T> action)
            where T : notnull
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

        public static async Task<Result<T, TError>> TapAsync<T, TError>(this Result<T, TError> result, Func<T, Task> action)
            where T : notnull
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

        public static Result<T, TError> Ensure<T, TError>(this Result<T, TError> result, Func<T, bool> predicate, TError error)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return predicate!(result.Value) ? result : Result.Failure<T, TError>(error);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Ensure.PredicateThrewException",
                    "An error occurred while evaluating the predicate in Ensure.",
                    ex);
            }
        }

        public static Result<T, TError> Ensure<T, TError>(this Result<T, TError> result, Func<T, bool> predicate, Func<TError> errorFactory)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");
            Invariant.That(errorFactory is not null, "Result.Ensure.ParameterCannotBeNull", "The error factory cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return predicate!(result.Value) ? result : Result.Failure<T, TError>(errorFactory!());
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Ensure.PredicateThrewException",
                    "An error occurred while evaluating the predicate in Ensure.",
                    ex);
            }
        }

        public static async Task<Result<T, TError>> EnsureAsync<T, TError>(this Result<T, TError> result, Func<T, Task<bool>> predicate, TError error)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return await predicate!(result.Value).ConfigureAwait(false)
                    ? result
                    : Result.Failure<T, TError>(error);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create(
                    "Result.Ensure.PredicateThrewException",
                    "An error occurred while evaluating the predicate in Ensure.",
                    ex);
            }
        }

        public static async Task<Result<T, TError>> EnsureAsync<T, TError>(this Result<T, TError> result, Func<T, Task<bool>> predicate, Func<Task<TError>> errorFactory)
            where T : notnull
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
                    : Result.Failure<T, TError>(await errorFactory!().ConfigureAwait(false));
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

        public static Result<T, TError> Recover<T, TError>(this Result<T, TError> result, Func<TError, T> recovery)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(recovery is not null, "Result.Recover.ParameterCannotBeNull", "The recovery function cannot be null.");

            try
            {
                return result.IsFailure
                    ? Result.Success<T, TError>(recovery!(result.Error))
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

        public static Result<T, TError> Recover<T, TError>(this Result<T, TError> result, Func<TError, Result<T, TError>> recovery)
            where T : notnull
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

        public static async Task<Result<T, TError>> RecoverAsync<T, TError>(this Result<T, TError> result, Func<TError, Task<T>> recovery)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(recovery is not null, "Result.Recover.ParameterCannotBeNull", "The recovery function cannot be null.");

            try
            {
                return result.IsFailure
                    ? Result.Success<T, TError>(await recovery!(result.Error).ConfigureAwait(false))
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

        public static async Task<Result<T, TError>> RecoverAsync<T, TError>(this Result<T, TError> result, Func<TError, Task<Result<T, TError>>> recovery)
            where T : notnull
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

        public static TOut Match<T, TError, TOut>(this Result<T, TError> result, Func<T, TOut> onSuccess, Func<TError, TOut> onFailure)
            where T : notnull
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

        public static void Match<T, TError>(this Result<T, TError> result, Action<T> onSuccess, Action<TError> onFailure)
            where T : notnull
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

        public static async Task<TOut> MatchAsync<T, TError, TOut>(this Result<T, TError> result, Func<T, Task<TOut>> onSuccess, Func<TError, Task<TOut>> onFailure)
            where T : notnull
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

        public static async Task MatchAsync<T, TError>(this Result<T, TError> result, Func<T, Task> onSuccess, Func<TError, Task> onFailure)
            where T : notnull
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

        public static Result<T, TError> Flatten<T, TError>(this Result<Result<T, TError>, TError> result)
            where T : notnull
            where TError : notnull
            => result.IsFailure ? Result.Failure<T, TError>(result.Error) : result.Value;

        public static Result<IReadOnlyList<T>, TError> Sequence<T, TError>(this IEnumerable<Result<T, TError>> results)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(results is not null, "Result.Sequence.ParameterCannotBeNull", "The results sequence cannot be null.");

            var list = new List<T>();
            foreach (var r in results!)
            {
                if (r.IsFailure)
                    return Result.Failure<IReadOnlyList<T>, TError>(r.Error);

                list.Add(r.Value);
            }

            return Result.Success<IReadOnlyList<T>, TError>(list);
        }

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

        public static Result<T, TError> TryCatch<T, TError>(Func<T> func, Func<Exception, TError> errorFactory)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(func is not null, "Result.TryCatch.ParameterCannotBeNull", "The function cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatch.ParameterCannotBeNull", "The error factory function cannot be null.");

            return TrySucceed(func!, errorFactory!);
        }

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

        public static async Task<Result<T, TError>> TryCatchAsync<T, TError>(Func<Task<T>> func, Func<Exception, TError> errorFactory)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(func is not null, "Result.TryCatchAsync.Func.CannotBeNull", "The function to execute cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatchAsync.ErrorFactory.CannotBeNull", "The error factory cannot be null.");

            return await TrySucceedAsync(func!, errorFactory!).ConfigureAwait(false);
        }

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

        public static T Unwrap<T, TError>(this Result<T, TError> result)
            where T : notnull
            where TError : notnull
        {
            if (result.IsFailure)
            {
                throw InvariantViolationException.Create("Result.Unwrap.Failure", "Attempted to unwrap a Result that is in a failure state.");
            }

            return result.Value;
        }

        public static Task<Result<T, TError>> AsTask<T, TError>(this Result<T, TError> result)
            where T : notnull
            where TError : notnull
            => Task.FromResult(result);

        public static ValueTask<Result<T, TError>> AsValueTask<T, TError>(this Result<T, TError> result)
            where T : notnull
            where TError : notnull
            => new(result);

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

        public static Result<T, TErrorOut> MapError<T, TErrorIn, TErrorOut>(this Result<T, TErrorIn> result, Func<TErrorIn, TErrorOut> mapError)
            where T : notnull
            where TErrorIn : notnull
            where TErrorOut : notnull
        {
            Invariant.That(mapError is not null,
                "Result.MapError.MapErrorCannotBeNull",
                "The error mapping function cannot be null.");

            if (result.IsSuccess)
                return Result.Success<T, TErrorOut>(result.Value);

            try
            {
                var mappedError = mapError!(result.Error);
                return Result.Failure<T, TErrorOut>(mappedError);
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

        private static async Task<Result<T, TError>> TrySucceedAsync<T, TError>(Func<Task<T>> func, Func<Exception, TError> errorFactory)
            where T : notnull
            where TError : notnull
        {
            try
            {
                var result = await func().ConfigureAwait(false);
                return Result.Success<T, TError>(result);
            }
            catch (Exception ex)
            {
                return TryFail<T, TError>(ex, errorFactory);
            }
        }

        private static Result<T, TError> TrySucceed<T, TError>(Func<T> func, Func<Exception, TError> errorFactory)
            where T : notnull
            where TError : notnull
        {
            try
            {
                var result = func();
                return Result.Success<T, TError>(result);
            }
            catch (Exception ex)
            {
                return TryFail<T, TError>(ex, errorFactory);
            }
        }

        private static Result<T, TError> TryFail<T, TError>(Exception ex, Func<Exception, TError> errorFactory)
            where T : notnull
            where TError : notnull
        {
            try
            {
                return Result.Failure<T, TError>(errorFactory(ex));
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
