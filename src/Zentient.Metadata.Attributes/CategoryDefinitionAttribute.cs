using System;
using Zentient.Abstractions.Metadata.Definitions;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Declares a type as a metadata category definition. Use on classes or interfaces implementing <see cref="ICategoryDefinition"/>.
    /// </summary>
    /// <remarks>
    /// This attribute is a marker for category definitions in the Zentient ecosystem.
    /// </remarks>
    /// <example>
    /// [CategoryDefinition("service")]
    /// public class ServiceCategory : ICategoryDefinition { }
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class CategoryDefinitionAttribute : MetadataAttribute
    {
        /// <summary>
        /// Gets the optional, human-readable name of the category.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryDefinitionAttribute"/> class.
        /// </summary>
        /// <param name="name">The human-readable name of the category.</param>
        /// <param name="order">The order in which this attribute should be processed.</param>
        /// <param name="preset">An optional preset name associated with the attribute.</param>
        public CategoryDefinitionAttribute(string? name = null, int order = 0, string? preset = null) : base(order, preset)
        {
            if (name != null && string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty or whitespace.", nameof(name));
            Name = name ?? string.Empty;
        }
    }
}
