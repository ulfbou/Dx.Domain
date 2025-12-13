// <copyright file="AttributeMetadataScanner.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System.Reflection;

using Zentient.Abstractions.Metadata;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Provides an implementation of <see cref="IAttributeMetadataScanner"/> that uses <see cref="MetadataAttributeReader"/>
    /// to scan types and members for metadata attributes and produce <see cref="IMetadata"/> instances.
    /// </summary>
    public class AttributeMetadataScanner : IAttributeMetadataScanner
    {
        /// <inheritdoc />
        public IMetadata Scan(Type type) => MetadataAttributeReader.GetMetadata(type);

        /// <inheritdoc />
        public IMetadata Scan(MemberInfo member) => MetadataAttributeReader.GetMetadata(member);

        /// <inheritdoc />
        public IEnumerable<(MemberInfo member, IMetadata metadata)> ScanAll(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                yield return (type, MetadataAttributeReader.GetMetadata(type));
                foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    yield return (member, MetadataAttributeReader.GetMetadata(member));
                }
            }
        }
    }
}
