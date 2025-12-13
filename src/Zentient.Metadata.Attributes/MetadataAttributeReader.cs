// <copyright file="MetadataAttributeReader.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Definitions;
using Zentient.Abstractions.Common.Metadata;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Provides utilities for discovering and reading metadata attributes from types
    /// and constructing <see cref="IMetadata"/> instances.
    /// </summary>
    /// <remarks>
    /// This class supports unified discovery of Zentient.Metadata and Zentient.Abstractions.Common.Metadata attributes.
    /// </remarks>
    /// <example>
    /// var metadata = MetadataAttributeReader.GetMetadata(typeof(MyService));
    /// // metadata will have tags "VersionTag"="1.2", "CacheableTag"=true
    /// </example>
    public static class MetadataAttributeReader
    {
        /// <summary>
        /// Scans a given type and its base types for metadata attributes, returning a
        /// consolidated <see cref="IMetadata"/> instance.
        /// </summary>
        /// <param name="type">The type to scan for metadata attributes.</param>
        /// <returns>An <see cref="IMetadata"/> instance containing all discovered metadata, or an empty instance if none are found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="type"/> is null.</exception>
        public static IMetadata GetMetadata(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            var builder = Metadata.Create();

            // Recursively scan the type and all base types.
            for (var t = type; t != null; t = t.BaseType)
            {
                var attributes = GetAttributes(t);

                foreach (var attribute in attributes)
                {
                    // Custom handler support
                    if (AttributeHandlerRegistry.TryHandle(attribute, new AttributeHandlerContext(builder)))
                        continue;
                    switch (attribute)
                    {
                        case BehaviorDefinitionAttribute:
                            builder.SetTag(t.FullName!, Metadata.Empty);
                            break;
                        case CategoryDefinitionAttribute categoryAttribute:
                            builder.SetTag(t.FullName!, categoryAttribute.Name);
                            break;
                        case MetadataTagAttribute tagAttribute:
                            builder.SetTag(tagAttribute.Key, tagAttribute.TagValue);
                            break;
                        case Zentient.Abstractions.Common.Metadata.DefinitionCategoryAttribute legacyCat:
                            builder.SetTag("category", legacyCat.CategoryName);
                            break;
                        case Zentient.Abstractions.Common.Metadata.DefinitionTagAttribute legacyTag:
                            builder.SetTag("tags", legacyTag.Tags);
                            break;
                        default:
                            break;
                    }
                }
            }

            return builder.Build();
        }

        /// <summary>
        /// Scans a given member (property, method, event, etc.) for metadata attributes,
        /// returning an <see cref="IMetadata"/> instance.
        /// </summary>
        /// <param name="member">The member to scan for metadata attributes.</param>
        /// <returns>An <see cref="IMetadata"/> instance containing all discovered metadata, or an empty instance if none are found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="member"/> is null.</exception>
        public static IMetadata GetMetadata(MemberInfo member)
        {
            ArgumentNullException.ThrowIfNull(member);
            var builder = Metadata.Create();
            var attributes = member.GetCustomAttributes(false).OfType<Attribute>();

            foreach (var attribute in attributes)
            {
                if (AttributeHandlerRegistry.TryHandle(attribute, new AttributeHandlerContext(builder)))
                    continue;
                switch (attribute)
                {
                    case BehaviorDefinitionAttribute:
                        builder.SetTag(member.Name, Metadata.Empty);
                        break;
                    case CategoryDefinitionAttribute categoryAttribute:
                        builder.SetTag(member.Name, categoryAttribute.Name);
                        break;
                    case MetadataTagAttribute tagAttribute:
                        builder.SetTag(tagAttribute.Key, tagAttribute.TagValue);
                        break;
                    case DefinitionCategoryAttribute legacyCat:
                        builder.SetTag("category", legacyCat.CategoryName);
                        break;
                    case DefinitionTagAttribute legacyTag:
                        builder.SetTag("tags", legacyTag.Tags);
                        break;
                    default:
                        break;
                }
            }
            return builder.Build();
        }

        private static IEnumerable<Attribute> GetAttributes(Type type)
        {
            // Get both Zentient.Metadata and Zentient.Abstractions.Common.Metadata attributes
            return type.GetCustomAttributes(false).OfType<Attribute>();
        }
    }
}
