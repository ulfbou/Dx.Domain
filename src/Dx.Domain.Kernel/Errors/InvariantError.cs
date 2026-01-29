// Copyright (c) Dx.Domain Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Dx.Domain.Errors;

/// <summary>
/// Represents detailed diagnostic information for a violated invariant.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="InvariantError"/> captures structural diagnostic context for invariant violations
/// (domain error, caller info, soft correlation, timestamp) without depending on typed identity primitives.
/// </para>
/// <para>
/// This preserves Kernel Law 3 ("Diagnostics as Data") while avoiding Primitives dependencies.
/// Correlation metadata is provided as strings or Guids by the caller, not typed identity structs.
/// </para>
/// <para>
/// For domain failures crossing boundaries without diagnostic context, use <see cref="DomainError"/> instead.
/// </para>
/// </remarks>
[DebuggerDisplay("InvariantError [{DomainError.Code}] @ {Member}:{Line}")]
public sealed class InvariantError
{
    /// <summary>
    /// Gets the domain error associated with the invariant violation.
    /// </summary>
    public DomainError DomainError { get; }

    /// <summary>
    /// Gets an optional message override to use instead of <see cref="DomainError.Message"/>.
    /// </summary>
    public string? MessageOverride { get; }

    /// <summary>
    /// Gets the member name where the invariant was violated (via CallerMemberName).
    /// </summary>
    public string Member { get; }

    /// <summary>
    /// Gets the source file name where the invariant was violated (via CallerFilePath).
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the line number where the invariant was violated (via CallerLineNumber).
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the UTC timestamp when the invariant violation was recorded.
    /// </summary>
    public DateTimeOffset UtcTimestamp { get; }

    private InvariantError(
        DomainError domainError,
        string? messageOverride,
        string member,
        string fileName,
        int line,
        DateTimeOffset utcTimestamp)
    {
        DomainError = domainError;
        MessageOverride = messageOverride;
        Member = member ?? string.Empty;
        FileName = fileName ?? string.Empty;
        Line = line;
        UtcTimestamp = utcTimestamp;
    }

    /// <summary>
    /// Creates a new <see cref="InvariantError"/> with diagnostic context.
    /// </summary>
    /// <param name="domainError">The domain error describing the invariant violation.</param>
    /// <param name="messageOverride">Optional message to override the domain error's default message.</param>
    /// <param name="member">Caller member name (auto-populated via CallerMemberName).</param>
    /// <param name="file">Caller file path (auto-populated via CallerFilePath).</param>
    /// <param name="line">Caller line number (auto-populated via CallerLineNumber).</param>
    /// <returns>A new <see cref="InvariantError"/> instance.</returns>
    /// <remarks>
    /// This method is intended for internal use to capture detailed context about where and
    /// why an invariant violation occurred. Caller information is automatically populated by
    /// the compiler and should not be set manually.
    /// </remarks>
    internal static InvariantError Create(
        DomainError domainError,
        string? messageOverride = null,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        var fileName = string.IsNullOrEmpty(file) ? string.Empty : Path.GetFileName(file);

        return new InvariantError(
            domainError,
            messageOverride,
            member,
            fileName,
            line,
            DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Gets the effective message for the invariant violation, using <see cref="MessageOverride"/>
    /// when present and falling back to <see cref="DomainError.Message"/> otherwise.
    /// </summary>
    public string EffectiveMessage => MessageOverride ?? DomainError.Message;

    /// <summary>
    /// Gets the effective error code from the wrapped <see cref="DomainError"/>.
    /// </summary>
    public string Code => DomainError.Code;

    /// <inheritdoc />
    public override string ToString()
    {
        var result = $"[{DomainError.Code}] {EffectiveMessage} @ {Member}";
        
        if (Line > 0)
            result += $":{Line}";
        
        if (!string.IsNullOrWhiteSpace(FileName))
            result += $" in {FileName}";
        
        return result;
    }
}
