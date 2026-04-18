// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="InputFingerprint.cs" company="Dx.Domain Team">
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
using System.Security.Cryptography;
using System.Text;

namespace Dx.Domain.Generators.Core
{
    /// <summary>
    /// Represents a deterministic fingerprint of generator inputs.
    /// Computed as SHA256(canonicalize(intent) || canonicalize(manifest) || canonicalize(policies) || generatorVersion).
    /// </summary>
    public sealed class InputFingerprint : IEquatable<InputFingerprint>
    {
        /// <summary>
        /// Gets the SHA256 hash value of the input fingerprint.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputFingerprint"/> class.
        /// </summary>
        /// <param name="value">The SHA256 hash value.</param>
        private InputFingerprint(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Computes an input fingerprint from canonicalized generator inputs.
        /// </summary>
        /// <param name="canonicalizedIntent">Canonicalized intent string.</param>
        /// <param name="canonicalizedManifest">Canonicalized manifest string.</param>
        /// <param name="canonicalizedPolicies">Canonicalized policies string.</param>
        /// <param name="generatorVersion">Generator version string.</param>
        /// <returns>A new <see cref="InputFingerprint"/> instance.</returns>
        public static InputFingerprint Compute(
            string canonicalizedIntent,
            string canonicalizedManifest,
            string canonicalizedPolicies,
            string generatorVersion)
        {
            ArgumentNullException.ThrowIfNull(canonicalizedIntent);
            ArgumentNullException.ThrowIfNull(canonicalizedManifest);
            ArgumentNullException.ThrowIfNull(canonicalizedPolicies);
            ArgumentNullException.ThrowIfNull(generatorVersion);

            var combined = string.Concat(
                canonicalizedIntent,
                canonicalizedManifest,
                canonicalizedPolicies,
                generatorVersion);

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(combined));
            var hashString = Convert.ToHexString(hash).ToLowerInvariant();

            return new InputFingerprint(hashString);
        }

        /// <summary>
        /// Creates an <see cref="InputFingerprint"/> from an existing hash value.
        /// </summary>
        /// <param name="hashValue">The SHA256 hash value.</param>
        /// <returns>A new <see cref="InputFingerprint"/> instance.</returns>
        public static InputFingerprint FromHash(string hashValue)
        {
            if (string.IsNullOrWhiteSpace(hashValue))
                throw new ArgumentException("Hash value cannot be null or whitespace.", nameof(hashValue));

            return new InputFingerprint(hashValue);
        }

        /// <inheritdoc/>
        public bool Equals(InputFingerprint? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as InputFingerprint);

        /// <inheritdoc/>
        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

        /// <inheritdoc/>
        public override string ToString() => Value;

        public static bool operator ==(InputFingerprint? left, InputFingerprint? right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(InputFingerprint? left, InputFingerprint? right)
        {
            return !(left == right);
        }
    }
}
