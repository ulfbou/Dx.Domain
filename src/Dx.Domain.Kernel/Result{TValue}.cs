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

using Dx.Domain.Errors;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

using static Dx.Domain.DxDomain;
using static Dx.Domain.DxDomain.Kernel;

namespace Dx.Domain
{
    /// <summary>
    /// Represents the outcome of a computation that may fail.
    /// </summary>
    /// <typeparam name="TValue">Successful value type.</typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct Result<TValue> where TValue : notnull
    {
        private readonly TValue? _value;
        private readonly DomainError? _error;

        private Result(TValue value)
        {
            _value = value;
            _error = null;
        }

        private Result(DomainError error)
        {
            _value = default;
            _error = error;
        }

        private Result(Result<TValue, DomainError> inner)
        {
            _value = inner.Value;
            _error = inner.Error;
        }

        /// <summary>
        /// Indicates whether the result represents success.
        /// </summary>
        public bool IsSuccess => _error is null;

        /// <summary>
        /// Indicates whether the result represents failure.
        /// </summary>
        public bool IsFailure => _error is not null;

        /// <summary>
        /// Gets the successful value or throws if accessed on failure.
        /// </summary>
        public TValue Value
        {
            get
            {
                Invariant.That(
                    IsSuccess,
                    DomainError.Create(
                        "Result.InvalidState",
                        "Cannot access Value on a failed result."));
                return _value!;
            }
        }

        /// <summary>
        /// Gets the failure error or throws if accessed on success.
        /// </summary>
        internal DomainError Error
        {
            get
            {
                Invariant.That(
                    IsFailure,
                    DomainError.Create(
                        "Result.InvalidState",
                        "Cannot access Error on a successful result."));
                return _error!.Value;
            }
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="value">The value to wrap in a successful result.</param>
        /// <returns>A successful result containing the provided value.</returns>
        internal static Result<TValue> Success(TValue value) => new(value);

        /// <summary>
        /// Creates a successful result for <see cref="Unit"/> type.
        /// </summary>
        /// <returns>A successful result containing <see cref="Unit.Value"/>.</returns>
        internal static Result<Unit> Success() => new(Unit.Value);

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="error">The domain error describing the failure.</param>
        /// <returns>A failed result containing the provided error.</returns>
        internal static Result<TValue> Failure(DomainError error) => new(error);

        /// <summary>
        /// Creates a result from an inner result with <see cref="DomainError" /> as the error type.
        /// </summary>
        /// <param name="inner">The inner result to convert.</param>
        /// <returns>A result containing the value and error from the inner result.</returns>
        internal static Result<TValue> From(Result<TValue, DomainError> inner) => new(inner);

        /// <summary>
        /// Deconstructs the result into its failure status, error, and value components.
        /// </summary>
        /// <param name="isSuccess">When this method returns, contains <see langword="true"/> if the result represents a failure; otherwise,
        /// <see langword="false"/>.</param>
        /// <param name="error">When this method returns, contains the associated <see cref="DomainError"/> if the result is a failure;
        /// otherwise, <see langword="null"/>.</param>
        /// <param name="value">When this method returns, contains the value if the result is successful; otherwise, <see langword="null"/>.</param>
        public void Deconstruct(out bool isSuccess, out TValue? value, out DomainError? error)
        {
            isSuccess = IsSuccess;
            value = isSuccess ? Value : default;
            error = !isSuccess ? Error : default;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
            => IsSuccess ? $"Ok({typeof(TValue).Name})" : $"Failure({_error!.Value.Code})";
    }
}
