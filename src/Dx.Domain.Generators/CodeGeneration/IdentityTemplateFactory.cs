// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="FileName.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Generators.Core;
using Dx.Domain.Generators.Model;

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace Dx.Domain.Generators.CodeGeneration
{
    internal sealed class IdentityTemplateFactory
    {
        private static readonly string[] IdentityHeaderBullets =
        {
            "Encapsulates a unique identifier (Guid backing)",
            "Enforces value-based equality",
            "Prevents accidental usage of raw primitives"
        };

        private readonly InputFingerprint _fingerprint;
        private readonly string _generatorName;

        public IdentityTemplateFactory(InputFingerprint fingerprint, string generatorName)
        {
            _fingerprint = fingerprint;
            _generatorName = generatorName;
        }

        public string Generate(ValueObjectIntent identity)
        {
            var sb = new StringBuilder();

            // 1. Header (Pedagogical & Provenance)
            sb.Append(GeneratedFileHeader.Generate(
                _generatorName,
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"Provides the strongly-typed identity '{identity.Name}'."),
                IdentityHeaderBullets));

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Diagnostics;");
            sb.AppendLine("using System.Runtime.CompilerServices;");
            sb.AppendLine("using Dx;");
            sb.AppendLine("using Dx.Domain;");
            sb.AppendLine();

            sb.AppendLine("namespace Dx.Domain.Generated"); // Or logic to map to DIM namespace
            sb.AppendLine("{");

            // 2. Type Declaration
            sb.AppendLine("    /// <summary>");
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"    /// Represents a strongly-typed identifier for {identity.Name}."));
            sb.AppendLine("    /// </summary>");
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"    [System.CodeDom.Compiler.GeneratedCode(\"{_generatorName}\", \"1.0.0\")]"));
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"    public readonly partial struct {identity.Name} : IEquatable<{identity.Name}>"));
            sb.AppendLine("    {");

            // 3. Backing Field & Properties
            sb.AppendLine("        /// <summary>Gets the underlying value.</summary>");
            sb.AppendLine("        public Guid Value { get; }");
            sb.AppendLine();

            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"        public static readonly {identity.Name} Empty = new {identity.Name}(Guid.Empty);"));
            sb.AppendLine();

            // 4. Private Constructor (Authority via Factory)
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"        private {identity.Name}(Guid value) => Value = value;"));
            sb.AppendLine();

            // 5. Factories (Deterministic & Explicit)
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Creates a new unique identifier.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"        public static {identity.Name} New()"));
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"            => new {identity.Name}(Guid.NewGuid());"));
            sb.AppendLine();

            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Creates an identifier from an existing Guid.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"        public static {identity.Name} From(Guid value)"));
            sb.AppendLine("        {");
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"            Dx.Invariant.That(value != Guid.Empty, Dx.Faults.InvalidInput(\"{identity.Name} cannot be empty.\"));"));
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"            return new {identity.Name}(value);"));
            sb.AppendLine("        }");
            sb.AppendLine();

            // 6. Equality Members (Value Semantics)
            sb.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"        public bool Equals({identity.Name} other) => Value.Equals(other.Value);"));
            sb.AppendLine();
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"        public override bool Equals(object? obj) => obj is {identity.Name} other && Equals(other);"));
            sb.AppendLine();
            sb.AppendLine("        public override int GetHashCode() => Value.GetHashCode();");
            sb.AppendLine();
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"        public static bool operator ==({identity.Name} left, {identity.Name} right) => left.Equals(right);"));
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"        public static bool operator !=({identity.Name} left, {identity.Name} right) => !left.Equals(right);"));
            sb.AppendLine();

            // 7. Debugger & String Representation
            sb.AppendLine("        public override string ToString() => Value.ToString(\"N\");");
            sb.AppendLine();
            sb.AppendLine("        [DebuggerBrowsable(DebuggerBrowsableState.Never)]");
            sb.AppendLine(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"        private string DebuggerDisplay => $\"{identity.Name}={{Value:N}}\";"));

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
