// <copyright file="Metadata.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Builders;
using Zentient.Abstractions.Metadata.Definitions;
using Zentient.Metadata.Internal;

namespace Zentient.Metadata
{
    /// <summary>
    /// Serves as the public-facing factory for creating new <see cref="IMetadata"/> instances.
    /// This is the primary entry point for consumers to interact with the metadata system.
    /// </summary>
    public static class Metadata
    {
        private static readonly IMetadata _empty = new MetadataImpl();

        /// <summary>
        /// Gets an empty, immutable <see cref="IMetadata"/> instance.
        /// </summary>
        public static IMetadata Empty => _empty;

        /// <summary>
        /// Creates a new, fluent builder for an <see cref="IMetadata"/> instance.
        /// </summary>
        /// <returns>A new <see cref="IMetadataBuilder"/> instance.</returns>
        public static IMetadataBuilder Create()
        {
            return new MetadataBuilder();
        }

        /// <summary>
        /// Creates a new <see cref="IMetadata"/> instance with a single behavior.
        /// </summary>
        /// <typeparam name="TBehavior">The concrete behavior definition type, which must implement <see cref="IBehaviorDefinition"/>.</typeparam>
        /// <returns>A new <see cref="IMetadata"/> instance containing the specified behavior.</returns>
        public static IMetadata WithBehavior<TBehavior>()
            where TBehavior : IBehaviorDefinition, new()
        {
            return new MetadataBuilder().SetTag(typeof(TBehavior).FullName!, new TBehavior()).Build();
        }

        /// <summary>
        /// Creates a new <see cref="IMetadata"/> instance with a single category.
        /// </summary>
        /// <typeparam name="TCategory">The concrete category definition type, which must implement <see cref="ICategoryDefinition"/>.</typeparam>
        /// <returns>A new <see cref="IMetadata"/> instance containing the specified category.</returns>
        public static IMetadata WithCategory<TCategory>()
            where TCategory : ICategoryDefinition, new()
        {
            return new MetadataBuilder().SetTag(typeof(TCategory).FullName!, new TCategory()).Build();
        }

        /// <summary>
        /// Creates a new <see cref="IMetadata"/> instance with a single tag.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type, which must implement <see cref="IMetadataTagDefinition"/>.</typeparam>
        /// <typeparam name="TValue">The type of the tag's value.</typeparam>
        /// <param name="value">The value of the tag.</param>
        /// <returns>A new <see cref="IMetadata"/> instance containing the specified tag.</returns>
        public static IMetadata WithTag<TTag, TValue>(TValue value)
            where TTag : IMetadataTagDefinition, new()
        {
            return new MetadataBuilder().SetTag(typeof(TTag).FullName!, value).Build();
        }
    }
}
