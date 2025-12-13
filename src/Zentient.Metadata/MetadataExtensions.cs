// <copyright file="MetadataExtensions.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Definitions;
using Zentient.Abstractions.Metadata.Readers;

namespace Zentient.Metadata
{
    /// <summary>
    /// Provides a rich set of fluent, type-safe extension methods for working with <see cref="IMetadata"/>.
    /// These methods enhance developer ergonomics and enable intuitive interactions with the metadata system.
    /// </summary>
    public static class MetadataExtensions
    {
        #region Behavioral and Categorical Extensions

        /// <summary>
        /// Adds a behavior to the metadata.
        /// </summary>
        /// <typeparam name="TBehavior">The concrete behavior definition type, which must implement <see cref="IBehaviorDefinition"/>.</typeparam>
        /// <param name="metadata">The metadata instance to extend.</param>
        /// <returns>A new <see cref="IMetadata"/> instance with the added behavior.</returns>
        public static IMetadata WithBehavior<TBehavior>(this IMetadata metadata)
            where TBehavior : IBehaviorDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.WithTag(typeof(TBehavior).FullName!, new TBehavior());
        }

        /// <summary>
        /// Checks if the metadata contains a specific behavior.
        /// </summary>
        /// <typeparam name="TBehavior">The concrete behavior definition type.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <returns>True if the behavior is present; otherwise, false.</returns>
        public static bool HasBehavior<TBehavior>(this IMetadata metadata)
            where TBehavior : IBehaviorDefinition
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.ContainsKey(typeof(TBehavior).FullName!);
        }

        /// <summary>
        /// Adds a category to the metadata.
        /// </summary>
        /// <typeparam name="TCategory">The concrete category definition type, which must implement <see cref="ICategoryDefinition"/>.</typeparam>
        /// <param name="metadata">The metadata instance to extend.</param>
        /// <returns>A new <see cref="IMetadata"/> instance with the added category.</returns>
        public static IMetadata WithCategory<TCategory>(this IMetadata metadata)
            where TCategory : ICategoryDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.WithTag(typeof(TCategory).FullName!, new TCategory());
        }

        /// <summary>
        /// Checks if the metadata is associated with a specific category.
        /// </summary>
        /// <typeparam name="TCategory">The concrete category definition type.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <returns>True if the category is present; otherwise, false.</returns>
        public static bool HasCategory<TCategory>(this IMetadata metadata)
            where TCategory : ICategoryDefinition
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.ContainsKey(typeof(TCategory).FullName!);
        }

        #endregion

        #region Tag Extensions

        /// <summary>
        /// Adds or updates a tag on the metadata.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type, which must implement <see cref="IMetadataTagDefinition"/>.</typeparam>
        /// <typeparam name="TValue">The type of the tag's value.</typeparam>
        /// <param name="metadata">The metadata instance to extend.</param>
        /// <param name="value">The value of the tag.</param>
        /// <returns>A new <see cref="IMetadata"/> instance with the added or updated tag.</returns>
        public static IMetadata WithTag<TTag, TValue>(this IMetadata metadata, TValue value)
            where TTag : IMetadataTagDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.WithTag(typeof(TTag).FullName!, value);
        }

        /// <summary>
        /// Adds a tag to the metadata only if a tag with the same key does not already exist.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type, which must implement <see cref="IMetadataTagDefinition"/>.</typeparam>
        /// <typeparam name="TValue">The type of the tag's value.</typeparam>
        /// <param name="metadata">The metadata instance to extend.</param>
        /// <param name="value">The value of the tag.</param>
        /// <returns>A new <see cref="IMetadata"/> instance if the tag was added; otherwise, the original instance.</returns>
        public static IMetadata WithTagIfNotExists<TTag, TValue>(this IMetadata metadata, TValue value)
            where TTag : IMetadataTagDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var key = typeof(TTag).FullName!;
            if (metadata.ContainsKey(key))
            {
                return metadata;
            }
            return metadata.WithTag(key, value);
        }

        /// <summary>
        /// Gets the value of a tag in a type-safe manner.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type.</typeparam>
        /// <typeparam name="TValue">The expected type of the tag's value.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <param name="defaultValue">The value to return if the tag does not exist or has the wrong type.</param>
        /// <returns>The value of the tag, or the default value if not found or the type is incorrect.</returns>
        public static TValue GetTagValue<TTag, TValue>(this IMetadata metadata, TValue defaultValue = default!)
            where TTag : IMetadataTagDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var key = typeof(TTag).FullName!;
            return metadata.GetValueOrDefault(key, defaultValue);
        }

        public static TValue GetTagValue<TValue>(this IMetadata metadata, string key, TValue defaultValue = default!)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            return metadata.GetValueOrDefault(key, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of a tag in a type-safe manner.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type.</typeparam>
        /// <typeparam name="TValue">The expected type of the tag's value.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <param name="value">When this method returns, contains the value of the tag, if found and if the type is correct; otherwise, the default value for the type.</param>
        /// <returns>True if the value was successfully retrieved; otherwise, false.</returns>
        public static bool TryGetTagValue<TTag, TValue>(this IMetadata metadata, out TValue value)
            where TTag : IMetadataTagDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var key = typeof(TTag).FullName!;
            return metadata.TryGetValue(key, out value!);
        }

        /// <summary>
        /// Checks if the metadata contains a specific tag.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <returns>True if the tag is present; otherwise, false.</returns>
        public static bool HasTag<TTag>(this IMetadata metadata)
            where TTag : IMetadataTagDefinition
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.ContainsKey(typeof(TTag).FullName!);
        }

        #endregion

        #region Merging and Traversal

        /// <summary>
        /// Merges tags from another <see cref="IMetadataReader"/> instance into the current metadata.
        /// </summary>
        /// <param name="metadata">The metadata instance to extend.</param>
        /// <param name="other">The metadata reader to merge from.</param>
        /// <returns>A new <see cref="IMetadata"/> instance with the merged tags.</returns>
        public static IMetadata WithMergedTags(this IMetadata metadata, IMetadataReader other)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentNullException.ThrowIfNull(other);
            return metadata.Merge(other);
        }

        /// <summary>
        /// Attempts to retrieve a behavior from the metadata.
        /// </summary>
        /// <typeparam name="TBehavior">The concrete behavior definition type.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <returns>The metadata instance representing the behavior, or null if not found.</returns>
        public static IMetadata? GetBehavior<TBehavior>(this IMetadata metadata)
            where TBehavior : IBehaviorDefinition
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var key = typeof(TBehavior).FullName!;
            if (metadata.TryGetValue(key, out var behavior))
            {
                return behavior as IMetadata;
            }
            return null;
        }

        /// <summary>
        /// Gets the value of a tag or retrieves a default value if the tag is not present or cannot be cast to the desired type.
        /// </summary>
        /// <typeparam name="TValue">The expected type of the tag's value.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <param name="key">The key of the tag to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the tag is not present or cannot be cast.</param>
        /// <returns>The value of the tag, or the default value if the tag is not present or cannot be cast.</returns>
        public static TValue GetValueOrDefault<TValue>(this IMetadata metadata, string key, TValue defaultValue = default!)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            return metadata.TryGetValue(key, out var obj) && obj is TValue typedValue
                ? typedValue
                : defaultValue;
        }

        /// <summary>
        /// Tries to retrieve a value from the metadata based on the specified key and casts it to the desired type.
        /// </summary>
        /// <typeparam name="TValue">The expected type of the value.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <param name="value">When this method returns, contains the casted value if the operation was successful; otherwise, the default value for the type.</param>
        /// <returns>True if the value was successfully retrieved and casted; otherwise, false.</returns>
        public static bool TryGetValue<TValue>(this IMetadata metadata, string key, out TValue? value)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            if (metadata.TryGetValue(key, out var obj) && obj is TValue typedValue)
            {
                value = typedValue;
                return true;
            }
            value = default!;
            return false;
        }

        #endregion
    }
}
