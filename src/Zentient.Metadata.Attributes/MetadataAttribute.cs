// <copyright file="MetadataAttribute.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;

using Zentient.Abstractions.Metadata.Definitions;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Serves as the base class for all metadata-related attributes within the Zentient Framework.
    /// This abstract class provides a common base for discovery and future extensibility.
    /// </summary>
    /// <remarks>
    /// All custom metadata attributes should inherit from this class to be consistently
    /// discoverable by the Zentient metadata engine and analyzers.
    /// </remarks>
    /// <example>
    /// // Custom attribute example
    /// public sealed class MyCustomTagAttribute : MetadataAttribute { }
    /// </example>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public abstract class MetadataAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataAttribute"/> class.
        /// </summary>
        /// <param name="order">The order in which this attribute should be processed. Higher numbers have higher priority in conflict resolution.</param>
        /// <param name="preset">An optional preset name associated with the attribute.</param>
        public MetadataAttribute(int order = 0, string? preset = null)
        {
            Order = order;
            Preset = preset ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the order in which this attribute should be processed.
        /// Higher numbers have higher priority in conflict resolution.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets an optional preset name associated with the attribute.
        /// </summary>
        public string Preset { get; set; }
    }
}
