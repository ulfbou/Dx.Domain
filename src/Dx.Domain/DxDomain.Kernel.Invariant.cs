// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxDomain.Invariant.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain;
using Dx.Domain.Errors;
using Dx.Domain.Primitives;

using System;
using System.Runtime.CompilerServices;

namespace Dx.Domain
{
    public static partial class DxDomain
    {
        internal static partial class Kernel
        {
            /// <summary>
            /// Exception-based invariant enforcement for kernel primitives.
            /// </summary>
            internal static partial class Invariant
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static void That(
                    bool condition,
                    DomainError error,
                    string? message = null,
                    CorrelationId? correlationId = null,
                    TraceId? traceId = null,
                    SpanId? spanId = null,
                    [CallerMemberName] string member = "",
                    [CallerFilePath] string file = "",
                    [CallerLineNumber] int line = 0)
                {
                    if (condition)
                        return;

                    var diagnostic = InvariantError.InternalCreate(
                        error,
                        message ?? error.Message,
                        correlationId ?? CorrelationId.Empty,
                        traceId ?? TraceId.Empty,
                        spanId ?? SpanId.Empty,
                        member,
                        file,
                        line);

                    throw InvariantViolationException.Create(diagnostic);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static void That(
                    bool condition,
                    Func<DomainError> errorFactory,
                    string? message = null,
                    CorrelationId? correlationId = null,
                    TraceId? traceId = null,
                    SpanId? spanId = null,
                    [CallerMemberName] string member = "",
                    [CallerFilePath] string file = "",
                    [CallerLineNumber] int line = 0)
                {
                    DxDomain.Kernel.Invariant.That(errorFactory != null, Faults.Guard.ParameterCannotBeNull(nameof(errorFactory)));

                    if (condition)
                        return;

                    var error = errorFactory();
                    var diagnostic = InvariantError.InternalCreate(
                        error,
                        message ?? error.Message,
                        correlationId ?? CorrelationId.Empty,
                        traceId ?? TraceId.Empty,
                        spanId ?? SpanId.Empty,
                        member,
                        file,
                        line);

                    throw InvariantViolationException.Create(diagnostic);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static InvariantError CreateInvariantError(
                    DomainError domainError,
                    string? messageOverride = null,
                    CorrelationId? correlationId = null,
                    TraceId? traceId = null,
                    SpanId? spanId = null,
                    [CallerMemberName] string member = "",
                    [CallerFilePath] string file = "",
                    [CallerLineNumber] int line = 0)
                {
                    return InvariantError.InternalCreate(
                        domainError,
                        messageOverride,
                        correlationId ?? CorrelationId.Empty,
                        traceId ?? TraceId.Empty,
                        spanId ?? SpanId.Empty,
                        member,
                        file,
                        line);
                }
            }
        }
    }
}
