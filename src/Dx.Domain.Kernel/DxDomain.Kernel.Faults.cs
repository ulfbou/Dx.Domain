// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxDomain.Faults.cs" company="Dx.Domain Team">
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
using System.Runtime.CompilerServices;

using static Dx.Domain.DxDomain.Kernel;

namespace Dx.Domain
{
    public static partial class DxDomain
    {
        internal static partial class Kernel
        {
            /// <summary>
            /// Internal catalog of standardized kernel refusals and diagnostic errors.
            /// </summary>
            internal static partial class Faults
            {
                /// <summary>
                /// Primitive guard errors used inside the kernel.
                /// </summary>
                internal static class Guard
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static DomainError StringParameterCannotBeNullOrWhitespace(string argumentName)
                        => DomainError.Create(DxDomain.Codes.Invariant.Violation, $"String argument '{argumentName}' cannot be null or whitespace.");

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static DomainError ParameterCannotBeNull(string argumentName)
                        => DomainError.Create(DxDomain.Codes.Invariant.Violation, $"Parameter '{argumentName}' cannot be null.");

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static DomainError GuidParameterCannotBeEmpty(string argumentName)
                        => DomainError.Create(DxDomain.Codes.Invariant.Violation, $"GUID argument '{argumentName}' cannot be an empty GUID.");
                }

                internal static DomainError CorrelationIdCannotBeEmpty()
                    => DomainError.Create(
                        DxDomain.Codes.Invariant.Violation,
                        "CorrelationId cannot be an empty GUID.");
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static DomainError FactoryBypass(string detail)
                => DomainError.Create(DxDomain.Codes.Invariant.FactoryBypass, detail);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static DomainError InvalidInput(string detail)
                => DomainError.Create(DxDomain.Codes.Invariant.InvalidInput, detail);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static DomainError UnhandledException(Exception ex)
                => DomainError.Create(DxDomain.Codes.Domain.Failure, $"Unhandled exception: {ex.GetType().FullName}: {ex.Message}");

            internal static partial class Causation
            {
                public static DomainError MissingCorrelation
                    => DomainError.Create(DxDomain.Codes.Common.MissingCorrelation, "Causation requires a non-empty CorrelationId.");

                public static DomainError MissingTrace
                    => DomainError.Create(DxDomain.Codes.Common.MissingTrace, "Causation requires a non-empty TraceId.");
            }

            internal static partial class Fact
            {
                public static DomainError MissingFactType
                    => DomainError.Create(DxDomain.Codes.Common.MissingFactType, "Domain facts must declare a fact type.");

                public static DomainError MissingTrace
                    => DomainError.Create(DxDomain.Codes.Common.MissingTrace, "Domain facts require a non-empty TraceId in their Causation.");

                public static DomainError MissingPayload
                    => DomainError.Create(DxDomain.Codes.Common.MissingPayload, "Domain facts require a non-null payload.");
            }

            internal static partial class Result
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static DomainError MissingPayload()
                    => DomainError.Create(DxDomain.Codes.Common.MissingPayload, "Result is missing expected payload data.");
            }

            internal static partial class Validation
            {
                public static DomainError MissingRequiredField(string fieldName)
                {
                    DxDomain.Kernel.Invariant.That(
                        !string.IsNullOrEmpty(fieldName),
                        DomainError.Create(
                            DxDomain.Codes.Validation.InvalidFieldName,
                            "Field name cannot be null or empty."));

                    return DomainError.Create(
                        DxDomain.Codes.Validation.MissingRequiredField,
                        $"Required field '{fieldName}' is missing.");
                }
            }

            /// <summary>
            /// Public factory helpers for creating DomainError instances for external consumers.
            /// </summary>
            public static class Factory
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static DomainError Create(string code, string message)
                    => DomainError.Create(code, message);
            }
        }
    }
}
