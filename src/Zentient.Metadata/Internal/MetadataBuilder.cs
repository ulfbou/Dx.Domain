// <copyright file="src/Zentient.Metadata/Builders/MetadataBuilders.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Builders;
using Zentient.Abstractions.Metadata.Readers;
using Zentient.Metadata;

namespace Zentient.Metadata.Internal
{
    /// <summary>
    /// A concrete, mutable implementation of <see cref="IMetadataBuilder"/>.
    /// It uses a dictionary to build the state of the final metadata object.
    /// </summary>
    internal sealed class MetadataBuilder : IMetadataBuilder
    {
        private readonly Dictionary<string, object?> _tags = new();

        public IMetadataBuilder SetTag(string key, object? value)
        {
            Guard.AgainstNullOrWhitespace(key, nameof(key));
            _tags[key] = value;
            return this;
        }

        public IMetadataBuilder AddTags(IEnumerable<KeyValuePair<string, object?>> tags)
        {
            Guard.AgainstNull(tags, nameof(tags));
            foreach (var tag in tags)
            {
                _tags[tag.Key] = tag.Value;
            }
            return this;
        }

        public IMetadataBuilder RemoveTag(string key)
        {
            Guard.AgainstNullOrWhitespace(key, nameof(key));
            _tags.Remove(key);
            return this;
        }

        public IMetadataBuilder RemoveTags(IEnumerable<string> keys)
        {
            Guard.AgainstNull(keys, nameof(keys));
            var keysToRemove = keys as ICollection<string> ?? keys.ToArray();
            if (keysToRemove.Count == 0)
                return this;

            var hashSet = keysToRemove is HashSet<string> hs ? hs : new HashSet<string>(keysToRemove);
            foreach (var key in hashSet)
            {
                _tags.Remove(key);
            }
            return this;
        }

        public IMetadataBuilder RemoveTags(Func<string, object?, bool> predicate)
        {
            Guard.AgainstNull(predicate, nameof(predicate));
            var keysToRemove = _tags.Where(kvp => predicate(kvp.Key, kvp.Value)).Select(kvp => kvp.Key).ToList();
            foreach (var key in keysToRemove)
            {
                _tags.Remove(key);
            }
            return this;
        }

        public IMetadataBuilder UpdateTags(Func<string, object?, bool> predicate, Func<string, object?, object?> updateFunction)
        {
            Guard.AgainstNull(predicate, nameof(predicate));
            Guard.AgainstNull(updateFunction, nameof(updateFunction));
            foreach (var key in _tags.Keys.ToArray())
            {
                if (predicate(key, _tags[key]))
                {
                    _tags[key] = updateFunction(key, _tags[key]);
                }
            }
            return this;
        }

        public IMetadataBuilder Merge(IMetadataReader metadata)
        {
            Guard.AgainstNull(metadata, nameof(metadata));
            return AddTags(metadata.Tags);
        }

        public IMetadata Build() => new MetadataImpl(_tags);
    }
}
