// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Dx.Domain.Facts;

/// <summary>
/// Represents a versioned fact type identifier.
/// </summary>
public readonly record struct FactType
{
    /// <summary>
    /// Gets the fact type code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the fact type version.
    /// </summary>
    public int Version { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FactType"/> struct.
    /// </summary>
    /// <param name="code">The fact type code.</param>
    /// <param name="version">The fact type version.</param>
    public FactType(string code, int version)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Fact type code must not be null or whitespace.", nameof(code));

        if (version < 1)
            throw new ArgumentOutOfRangeException(nameof(version), version, "Fact type version must be greater than zero.");

        Code = code;
        Version = version;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Code}@v{Version}";
}

/// <summary>
/// Provides default fact type metadata for a payload type.
/// </summary>
/// <typeparam name="TPayload">The payload type.</typeparam>
[SuppressMessage("Design", "CA1000:Do not declare static members on generic types",
    Justification = "Fact type metadata is exposed via generic helper for payload types.")]
public static class FactTypeOf<TPayload>
    where TPayload : notnull
{
    /// <summary>
    /// Gets the default fact type code for the payload.
    /// </summary>
    public static string Code => Value.Code;

    /// <summary>
    /// Gets the default fact type version for the payload.
    /// </summary>
    public static int Version => Value.Version;

    /// <summary>
    /// Gets the default <see cref="FactType"/> for the payload.
    /// </summary>
    public static FactType Value { get; } = new(typeof(TPayload).Name, 1);
}
