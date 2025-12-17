// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DomainError.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Diagnostics;
using static Dx.Dx;

namespace Dx.Domain
{
    /// <summary>
    /// Represents a domain-specific error with a unique code and a descriptive message.
    /// </summary>
    /// <remarks>
    /// This struct is used to standardize error reporting across the domain layer. It avoids the overhead
    /// of exceptions for control flow and provides a serializable error format. Equality is based solely
    /// on the <see cref="Code"/>.
    /// </remarks>
    [DebuggerDisplay("{Code,nq} @ {Message,nq}")]
    public readonly struct DomainError : IEquatable<DomainError>
    {
        /// <summary>Gets the unique error code identifying the type of failure.</summary>
        public string Code { get; }

        /// <summary>Gets the descriptive message explaining the reason for the failure.</summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainError"/> struct.
        /// </summary>
        /// <param name="code">The unique code that identifies the domain error.</param>
        /// <param name="message">The descriptive message that explains the domain error.</param>
        private DomainError(string code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DomainError"/> struct with the specified error code and message.
        /// </summary>
        /// <param name="code">The unique code that identifies the domain error. Cannot be null or whitespace.</param>
        /// <param name="message">The descriptive message that explains the domain error. Cannot be null or whitespace.</param>
        /// <param name="validate">If set to <see langword="true"/>, performs invariant validation on the input parameters.</param>
        /// <returns>A <see cref="DomainError"/> instance initialized with the specified code and message.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DomainError Create(string code, string message, bool validate = true)
        {
            if (validate)
            {
                // Note: We use a bypass/raw fault here to prevent infinite recursion in the invariant system 
                // if the validation itself fails.
                Invariant.That(!string.IsNullOrWhiteSpace(code), () => DomainError.Create("DomainError.NullCode", "The error code cannot be null or whitespace.", false));
                Invariant.That(!string.IsNullOrWhiteSpace(message), () => DomainError.Create("DomainError.NullMessage", "The error message cannot be null or whitespace.", false));
            }

            return new DomainError(code, message);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type based on the <see cref="Code"/>.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the other parameter; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DomainError other) => string.Equals(Code, other.Code, StringComparison.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is DomainError other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Code?.GetHashCode(StringComparison.Ordinal) ?? 0;

        /// <summary>
        /// Compares two <see cref="DomainError"/> instances for equality.
        /// </summary>
        public static bool operator ==(DomainError left, DomainError right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="DomainError"/> instances for inequality.
        /// </summary>
        public static bool operator !=(DomainError left, DomainError right) => !left.Equals(right);

        /// <summary>
        /// Returns the <see cref="Code"/> as the string representation of the error.
        /// </summary>
        public override string ToString() => Code ?? string.Empty;
    }
}
