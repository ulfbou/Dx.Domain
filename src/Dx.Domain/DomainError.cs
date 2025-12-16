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

namespace Dx.Domain
{
    /// <summary>
    /// Represents a domain-specific error with a code and descriptive message.
    /// </summary>
    /// <remarks>Use this struct to convey business or validation errors within a domain model. Two
    /// <see cref="DomainError"/> instances are considered equal if their <see cref="Code"/> values are equal. This type is
    /// immutable and can be used as a value object in domain-driven design.</remarks>
    public readonly struct DomainError : IEquatable<DomainError>
    {
        /// <summary>Gets the error code.</summary>
        public string Code { get; }

        /// <summary>Gets the error message.</summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainError"/> struct with the specified error code and message.
        /// </summary>
        /// <param name="code">The unique code that identifies the domain error. Cannot be <see langword="null"/> or empty.</param>
        /// <param name="message">The descriptive message that explains the domain error. Cannot be <see langword="null"/> or empty.</param>
        private DomainError(string code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DomainError"/> struct with the specified error code and message.
        /// </summary>
        /// <param name="code">The unique code that identifies the domain error. Cannot be <see langword="null"/> or whitespace.</param>
        /// <param name="message">The descriptive message that explains the domain error. Cannot be <see langword="null"/> or whitespace.</param>
        /// <returns>A <see cref="DomainError"/> instance initialized with the specified code and message.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="code"/> or <paramref name="message"/> is <see langword="null"/> or consists only of white-space characters.</exception>
        public static DomainError Create(string code, string message)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(DomainErrors.Code.NullOrWhitespace, nameof(code));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(DomainErrors.Message.NullOrWhitespace, nameof(message));
            }

            return new DomainError(code, message);
        }

        /// <inheritdoc />
        public bool Equals(DomainError other) => Code == other.Code;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is DomainError other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Code.GetHashCode(StringComparison.Ordinal);

        /// <inheritdoc />
        public static bool operator ==(DomainError a, DomainError b) => a.Equals(b);

        /// <inheritdoc />
        public static bool operator !=(DomainError a, DomainError b) => !a.Equals(b);

        /// <inheritdoc />
        public override string ToString() => Code;
    }
}
