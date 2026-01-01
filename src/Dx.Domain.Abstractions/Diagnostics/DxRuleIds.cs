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
        /// <summary>
        /// Provides constant identifiers for domain modeling rules enforced by analyzers.
        /// </summary>
        /// <remarks>Each constant represents a unique rule ID used to identify specific domain modeling
        /// requirements, such as aggregate structure, immutability, and entity identity. These IDs are typically
        /// referenced in analyzer diagnostics and code analysis tools to indicate which rule has been
        /// violated.</remarks>
        public static class RuleIds
        {
            /// <summary>
            /// Represents the diagnostic code indicating that an aggregate type must be declared as partial.
            /// </summary>
            public const string AggregateMustBePartial = "DXA001";

            /// <summary>
            /// Represents the diagnostic code indicating that an aggregate type must have a factory method defined.
            /// </summary>
            /// <remarks>This constant can be used to identify diagnostics related to missing factory
            /// methods in aggregate types, typically in code analysis or validation scenarios.</remarks>
            public const string AggregateMustHaveFactory = "DXA002";

            /// <summary>
            /// Represents the diagnostic code indicating that a value object must be immutable.
            /// </summary>
            /// <remarks>This constant can be used to identify diagnostics related to immutability
            /// requirements for value objects, such as in code analysis or validation scenarios.</remarks>
            public const string ValueObjectMustBeImmutable = "DXV001";

            /// <summary>
            /// Represents the error code indicating that an entity must have an identity value defined.
            /// </summary>
            public const string EntityMustHaveIdentity = "DXE001";

            /// <summary>
            /// Represents the diagnostic code indicating that a domain event must be immutable.
            /// </summary>
            /// <remarks>This constant can be used to identify or categorize diagnostics related to
            /// domain event immutability, such as when enforcing architectural or design constraints in validation or
            /// analysis tools.</remarks>
            public const string DomainEventMustBeImmutable = "DXD001";
        }

        /// <summary>
        /// Provides string constants that represent category names used for organizing domain-related code elements.
        /// </summary>
        /// <remarks>These category constants can be used to annotate or group code elements according to
        /// their domain model, correctness, or design aspects. This facilitates consistent categorization and filtering
        /// in tools or documentation.</remarks>
        public static class Categories
        {
            /// <summary>
            /// Represents the namespace string for the domain model used in the application.
            /// </summary>
            public const string DomainModel = "Dx.Domain.Model";

            /// <summary>
            /// Represents the identifier for the domain correctness diagnostic category.
            /// </summary>
            public const string DomainCorrectness = "Dx.Domain.Correctness";

            /// <summary>
            /// Represents the domain name identifier for the design module.
            /// </summary>
            public const string DomainDesign = "Dx.Domain.Design";
        }
    }
}
