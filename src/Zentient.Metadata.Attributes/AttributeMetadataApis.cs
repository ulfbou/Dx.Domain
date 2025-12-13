// <copyright file="AttributeMetadataApis.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Common.Metadata;
using Zentient.Metadata.Attributes;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Defines a contract for scanning types and members for metadata attributes and producing <see cref="IMetadata"/> instances.
    /// </summary>
    public interface IAttributeMetadataScanner
    {
        /// <summary>
        /// Scans a <see cref="Type"/> for metadata attributes and produces an <see cref="IMetadata"/> instance.
        /// </summary>
        /// <param name="type">The type to scan for metadata attributes.</param>
        /// <returns>An <see cref="IMetadata"/> instance representing the metadata found on the type.</returns>
        IMetadata Scan(Type type);

        /// <summary>
        /// Scans a <see cref="MemberInfo"/> for metadata attributes and produces an <see cref="IMetadata"/> instance.
        /// </summary>
        /// <param name="member">The member to scan for metadata attributes.</param>
        /// <returns>An <see cref="IMetadata"/> instance representing the metadata found on the member.</returns>
        IMetadata Scan(MemberInfo member);

        /// <summary>
        /// Scans all types and members in an <see cref="Assembly"/> for metadata attributes.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        /// <returns>
        /// An enumerable of tuples, each containing a <see cref="MemberInfo"/> and its associated <see cref="IMetadata"/>.
        /// </returns>
        IEnumerable<(MemberInfo member, IMetadata metadata)> ScanAll(Assembly assembly);
    }
}
