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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

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
        /// Gets a stable, machine-readable error code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets a human-readable description of the error.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets a collection of metadata associated with the current instance.
        /// </summary>
        public ImmutableDictionary<string, object> Metadata { get; init; }

        private DomainError(string code, string message, ImmutableArray<KeyValuePair<string, object>> metadata)
        {
            Code = code;
            Message = message;
            Metadata = ImmutableDictionary.CreateRange(metadata);
        }

        /// <summary>
        /// Creates a new instance of the DomainError class with the specified error code and message.
        /// </summary>
        /// <param name="code">The unique code that identifies the type of domain error. Cannot be <see langword="null"/> or empty.</param>
        /// <param name="message">The descriptive message that explains the error. Cannot be <see langword="null"/>.</param>
        /// <param name="metadata">Metadata associated with the error. Cannot be <see langword="null"/>.</param>
        /// <returns>A new DomainError instance initialized with the specified code and message.</returns>
        public static DomainError Create(string code, string message, ImmutableArray<KeyValuePair<string, object>>? metadata = null)
            => new(code, message, metadata: metadata ?? ImmutableArray<KeyValuePair<string, object>>.Empty);

        /// <summary>
        /// Returns a new instance of the current error with the specified metadata key and value added or updated.
        /// </summary>
        /// <param name="key">The metadata key to add or update. Cannot be <see langword="null"/>.</param>
        /// <param name="value">The value to associate with the specified metadata key. Cannot be <see langword="null"/>.</param>
        /// <returns>A new <see cref="DomainError"/> instance that includes the specified metadata key and value.</returns>
        public DomainError Enrich(string key, string value)
            => this with { Metadata = Metadata.SetItem(key, value) };

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
