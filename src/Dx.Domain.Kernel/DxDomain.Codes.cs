// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxDomainCodes.cs" company="Dx.Domain Team">
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
    public static partial class DxDomain
    {
        /// <summary>
        /// Central catalog of domain error codes. Purely declarative; no behavior.
        /// </summary>
        public static partial class Codes
        {
            public static class Invariant
            {
                public const string Violation = "DX.INVARIANT.VIOLATION";
                public const string ValueRequired = "DX.INVARIANT.VALUE_REQUIRED";
                public const string ValueOutOfRange = "DX.INVARIANT.VALUE_OUT_OF_RANGE";
                public const string InvalidInput = "DX.INVARIANT.INVALID_INPUT";
                public const string InvalidFormat = "DX.INVARIANT.INVALID_FORMAT";
                public const string FactoryBypass = "DX.INVARIANT.FACTORY_BYPASS";
            }

            public static class Domain
            {
                public const string Failure = "DX.DOMAIN.FAILURE";
                public const string UnexpectedState = "DX.DOMAIN.UNEXPECTED_STATE";
            }

            public static class Common
            {
                public const string MissingCorrelation = "DX.COMMON.MISSING_CORRELATION";
                public const string MissingTrace = "DX.COMMON.MISSING_TRACE";
                public const string MissingFactType = "DX.COMMON.MISSING_FACT_TYPE";
                public const string MissingPayload = "DX.COMMON.MISSING_PAYLOAD";
            }

            public static class Validation
            {
                public const string InvalidFieldName = "DX.VALIDATION.INVALID_FIELD_NAME";
                public const string MissingRequiredField = "DX.VALIDATION.MISSING_REQUIRED_FIELD";
                // Potentially other validation codes...
            }
        }
    }
}
