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

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using static Dx.Domain.DxDomain;
using static Dx.Domain.DxDomain.Kernel;

namespace Dx.Domain.Errors
{
    /// <summary>
    /// Represents a stable, comparable domain error.
    /// Errors are value objects: immutable, explicit, and non-exceptional by default.
    /// </summary>
    [DebuggerDisplay("{Code,nq}")]
    public readonly struct DomainError : IEquatable<DomainError>
    {
        /// <summary>
        /// A stable, machine-readable error code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// A human-readable description of the error.
        /// </summary>
        public string Message { get; }

        private DomainError(string code, string message)
        {
            Invariant.That(!string.IsNullOrWhiteSpace(code), Faults.Guard.StringParameterCannotBeNullOrWhitespace(nameof(code)));
            Invariant.That(!string.IsNullOrWhiteSpace(message), Faults.Guard.StringParameterCannotBeNullOrWhitespace(nameof(message)));
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Creates a new instance of the DomainError class with the specified error code and message.
        /// </summary>
        /// <param name="code">The unique code that identifies the type of domain error. Cannot be null or empty.</param>
        /// <param name="message">The descriptive message that explains the error. Cannot be null.</param>
        /// <returns>A new DomainError instance initialized with the specified code and message.</returns>
        internal static DomainError Create(string code, string message)
            => new(code, message);

        /// <inheritdoc />
        public bool Equals(DomainError other)
            => string.Equals(Code, other.Code, StringComparison.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is DomainError other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
            => Code.GetHashCode(StringComparison.Ordinal);

        /// <inheritdoc />
        public static bool operator ==(DomainError left, DomainError right) => left.Equals(right);

        /// <inheritdoc />
        public static bool operator !=(DomainError left, DomainError right) => !left.Equals(right);

        /// <inheritdoc />
        public override string ToString() => Code;
    }
}
