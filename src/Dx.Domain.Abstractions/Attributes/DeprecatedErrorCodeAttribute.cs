// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DeprecatedErrorCodeAttribute.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Attributes
{
    // Optional attribute to mark deprecation (passive metadata)

    /// <summary>
    /// Indicates that an error code field is deprecated and should no longer be used.
    /// </summary>
    /// <remarks>Apply this attribute to fields representing error codes to provide metadata about their
    /// deprecation, including the version since which they are deprecated and the reason for deprecation. This
    /// attribute is intended for informational purposes and does not enforce any compiler warnings or errors.</remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DeprecatedErrorCodeAttribute : Attribute
    {
        public string Since { get; }
        public string Reason { get; }
        public DeprecatedErrorCodeAttribute(string since, string reason) { Since = since; Reason = reason; }
    }
}
