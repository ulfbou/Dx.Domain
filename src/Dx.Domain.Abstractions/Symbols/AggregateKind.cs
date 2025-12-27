// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AggregateKind.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain.Symbols
{
    public enum AggregateKind
    {
        Unknown = 0,
        Root = 1,
        Entity = 2,
        ValueObject = 3
    }
    public enum DomainRuleSeverity
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }
    public enum LifecycleStage
    {
        Construction = 0,
        Mutation = 1,
        Validation = 2,
        Persistence = 3
    }
}
