using System;
using System.Collections.Generic;
using System.Reflection;
using Zentient.Abstractions.Metadata;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Provides conversion from attribute collections to IMetadata.
    /// </summary>
    public static class AttributeMetadataConverter
    {
        /// <summary>
        /// Converts a collection of attributes into an <see cref="IMetadata"/> instance.
        /// </summary>
        /// <param name="attributes">The collection of attributes to convert.</param>
        /// <returns>An <see cref="IMetadata"/> instance representing the metadata defined by the attributes.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attributes"/> is null.</exception>
        public static IMetadata Convert(IEnumerable<Attribute> attributes)
        {
            // Use the same logic as MetadataAttributeReader, but for arbitrary attribute collections
            var builder = Metadata.Create();
            foreach (var attribute in attributes)
            {
                // Custom handler support
                if (AttributeHandlerRegistry.TryHandle(attribute, new AttributeHandlerContext(builder)))
                    continue;
                switch (attribute)
                {
                    case BehaviorDefinitionAttribute:
                        builder.SetTag(attribute.GetType().FullName!, Metadata.Empty);
                        break;
                    case CategoryDefinitionAttribute categoryAttribute:
                        builder.SetTag(attribute.GetType().FullName!, categoryAttribute.Name);
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
            return builder.Build();
        }
    }
}
