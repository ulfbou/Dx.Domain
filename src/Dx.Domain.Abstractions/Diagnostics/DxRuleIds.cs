// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DxRuleIds.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Diagnostics
{
    public static class DxDiagnostics
    {
        public static class RuleIds
        {
            public const string AggregateMustBePartial = "DXA001";
            public const string AggregateMustHaveFactory = "DXA002";
            public const string ValueObjectMustBeImmutable = "DXV001";
            public const string EntityMustHaveIdentity = "DXE001";
            public const string DomainEventMustBeImmutable = "DXD001";
        }
        public static class Categories
        {
            public const string DomainModel = "Dx.Domain.Model";
            public const string DomainCorrectness = "Dx.Domain.Correctness";
            public const string DomainDesign = "Dx.Domain.Design";
        }
    }
}
