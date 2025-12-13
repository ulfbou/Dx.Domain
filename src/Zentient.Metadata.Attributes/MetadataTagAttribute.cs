using System;
using Zentient.Abstractions.Metadata.Definitions;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Associates a metadata tag on a type or member. Use for key-value metadata tagging.
    /// </summary>
    /// <remarks>
    /// This attribute is repeatable and can be used for multiple tags on the same type/member.
    /// </remarks>
    /// <example>
    /// [MetadataTag(typeof(VersionTag), "1.2")]
    /// [MetadataTag(typeof(CacheableTag), true)]
    /// public class MyService { }
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class MetadataTagAttribute : MetadataAttribute
    {
        /// <summary>
        /// Gets the tag type used as the key.
        /// </summary>
        public Type TagType { get; }
        /// <summary>
        /// Gets the value associated with the tag.
        /// </summary>
        public object? TagValue { get; }
        /// <summary>
        /// Gets the key for the tag (the full name of the tag type).
        /// </summary>
        public string Key => TagType.FullName ?? TagType.Name;
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataTagAttribute"/> class.
        /// </summary>
        /// <param name="tagType">The tag type used as the key.</param>
        /// <param name="value">The value associated with the tag.</param>
        /// <param name="order">The order in which this attribute should be processed.</param>
        /// <param name="preset">An optional preset name associated with the attribute.</param>
        public MetadataTagAttribute(Type tagType, object? value, int order = 0, string? preset = null) : base(order, preset)
        {
            TagType = tagType ?? throw new ArgumentNullException(nameof(tagType));
            TagValue = value;
        }
    }
}
