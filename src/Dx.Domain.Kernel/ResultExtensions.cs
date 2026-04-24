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

        public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
            where T : notnull
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

        public static async ValueTask<Result<T>> TapAsync<T>(this Result<T> result, Func<T, Task> action)
            where T : notnull
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

        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, DomainError error) where T : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return predicate!(result.Value)
                    ? result
                    : Result.Failure<T>(error);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Ensure.Exception", "An exception occurred while evaluating the predicate.", ex);
            }
        }

        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Func<DomainError> errorFactory) where T : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");
            Invariant.That(errorFactory is not null, "Result.Ensure.ParameterCannotBeNull", "The error factory cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return predicate!(result.Value)
                    ? result
                    : Result.Failure<T>(errorFactory!());
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Ensure.Exception", "An exception occurred while evaluating the predicate.", ex);
            }
        }

        public static async ValueTask<Result<T>> EnsureAsync<T>(this Result<T> result, Func<T, Task<bool>> predicate, DomainError error) where T : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return await predicate!(result.Value).ConfigureAwait(false)
                    ? result
                    : Result.Failure<T>(error);
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Ensure.Exception", "An exception occurred while evaluating the predicate asynchronously.", ex);
            }
        }

        public static async ValueTask<Result<T>> EnsureAsync<T>(this Result<T> result, Func<T, Task<bool>> predicate, Func<Task<DomainError>> errorFactory) where T : notnull
        {
            Invariant.That(predicate is not null, "Result.Ensure.ParameterCannotBeNull", "The predicate cannot be null.");
            Invariant.That(errorFactory is not null, "Result.Ensure.ParameterCannotBeNull", "The error factory cannot be null.");

            if (result.IsFailure)
                return result;

            try
            {
                return await predicate!(result.Value).ConfigureAwait(false)
                    ? result
                    : Result.Failure<T>(await errorFactory!().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw InvariantViolationException.Create("Result.Ensure.Exception", "An exception occurred while evaluating the predicate asynchronously.", ex);
            }
        }

        #endregion

        #region Recover

        public static Result<T> Recover<T>(this Result<T> result, Func<DomainError, T> recoveryFunc) where T : notnull
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

        public static Result<T> Recover<T>(this Result<T> result, Func<DomainError, Result<T>> recoveryFunc) where T : notnull
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

        public static async ValueTask<Result<T>> RecoverAsync<T>(this Result<T> result, Func<DomainError, Task<T>> recoveryFunc) where T : notnull
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

        public static async ValueTask<Result<T>> RecoverAsync<T>(this Result<T> result, Func<DomainError, Task<Result<T>>> recovery) where T : notnull
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

        public static TOut Match<TOut, T>(this Result<T> result, Func<T, TOut> onSuccess, Func<DomainError, TOut> onFailure)
            where T : notnull
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

        public static void Match<T>(this Result<T> result, Action<T> onSuccess, Action<DomainError> onFailure) where T : notnull
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

        public static async ValueTask<TOut> MatchAsync<T, TOut>(this Result<T> result, Func<T, Task<TOut>> onSuccess, Func<DomainError, Task<TOut>> onFailure) where T : notnull
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

        public static async ValueTask MatchAsync<T>(this Result<T> result, Func<T, Task> onSuccess, Func<DomainError, Task> onFailure) where T : notnull
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

        public static Result<T> Flatten<T>(this Result<Result<T>> result) where T : notnull
            => result.IsFailure
                ? Result.Failure<T>(result.Error)
                : result.Value;

        public static async ValueTask<Result<T>> FlattenAsync<T>(this ValueTask<Result<Result<T>>> resultTask) where T : notnull
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.IsFailure
                ? Result.Failure<T>(result.Error)
                : result.Value;
        }

        public static Result<IReadOnlyList<T>> Sequence<T>(this IEnumerable<Result<T>> results) where T : notnull
        {
            Invariant.That(results is not null, "Result.Sequence.ParameterCannotBeNull", "The results sequence cannot be null.");

            var list = new List<T>();
            foreach (var result in results!)
            {
                if (result.IsFailure)
                    return Result.Failure<IReadOnlyList<T>>(result.Error);

                list.Add(result.Value);
            }

            return Result.Success<IReadOnlyList<T>>(list);
        }

        public static async ValueTask<Result<IReadOnlyList<T>>> SequenceAsync<T>(this IEnumerable<Task<Result<T>>> resultTasks) where T : notnull
        {
            Invariant.That(resultTasks is not null, "Result.Sequence.ParameterCannotBeNull", "The results sequence cannot be null.");

            var list = new List<T>();

            foreach (var resultTask in resultTasks!)
            {
                var result = await resultTask.ConfigureAwait(false);
                if (result.IsFailure)
                    return Result.Failure<IReadOnlyList<T>>(result.Error);
                list.Add(result.Value);
            }

            return Result.Success<IReadOnlyList<T>>(list);
        }

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

        public static Result<T> TryCatch<T>(Func<T> func, Func<Exception, DomainError> errorFactory) where T : notnull
        {
            Invariant.That(func is not null, "Result.TryCatch.ParameterCannotBeNull", "The function cannot be null.");
            Invariant.That(errorFactory is not null, "Result.TryCatch.ParameterCannotBeNull", "The error factory function cannot be null.");

            try
            {
                return Result.Success(func!());
            }
            catch (Exception ex)
            {
                return Result.Failure<T>(errorFactory!(ex));
            }
        }

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

        public static async ValueTask<Result<T>> TryCatchAsync<T>(Func<Task<T>> func, Func<Exception, DomainError> errorFactory) where T : notnull
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
                    return Result.Failure<T>(errorFactory!(ex));
                }
                catch (Exception innerEx)
                {
                    throw InvariantViolationException.Create("Result.TryCatchAsync.Exception", "An exception occurred while executing the error factory.", innerEx);
                }
            }
        }

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

        public static Task<Result<T>> AsTask<T>(this Result<T> result) where T : notnull
            => Task.FromResult(result);

        public static ValueTask<Result<T>> AsValueTask<T>(this Result<T> result) where T : notnull
            => new(result);

        public static Result<T, TError> ToGenericError<T, TError>(this Result<T> result, Func<DomainError, TError> mapError)
            where T : notnull
            where TError : notnull
        {
            Invariant.That(mapError is not null, "Result.MapError.MapErrorCannotBeNull", "The error mapping function cannot be null.");

            if (result.IsSuccess)
                return Result.Success<T, TError>(result.Value);

            var mappedError = mapError!(result.Error);
            return Result.Failure<T, TError>(mappedError);
        }

        public static T Unwrap<T>(this Result<T> result) where T : notnull
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
