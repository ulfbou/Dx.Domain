// <copyright file="src/Zentient.Metadata/Builders/MetadataBuilders.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Readers;

namespace Zentient.Metadata.Internal
{
    /// <summary>
    /// A concrete, immutable implementation of <see cref="IMetadata"/>.
    /// This is the final, read-only metadata object produced by the builder.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    internal sealed class MetadataImpl : IMetadata
    {
        private static readonly StringComparer KeyComparer = StringComparer.Ordinal;
        private readonly ImmutableDictionary<string, object?> _tags;

        public MetadataImpl(IDictionary<string, object?>? tags = null)
        {
            _tags = tags is ImmutableDictionary<string, object?> imd
                ? imd
                : ImmutableDictionary.CreateRange(KeyComparer, tags ?? new Dictionary<string, object?>());
        }

        // --- IMetadataReader Implementation (via Composition) ---
        public int Count => _tags.Count;
        public IEnumerable<string> Keys => _tags.Keys;
        public IEnumerable<object?> Values => _tags.Values;
        public IEnumerable<KeyValuePair<string, object?>> Tags => _tags;

        public bool ContainsKey(string key)
        {
            Guard.AgainstNullOrWhitespace(key, nameof(key));
            return _tags.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object? value) => _tags.TryGetValue(key, out value);

        public bool TryGetValue<TValue>(string key, [MaybeNullWhen(false)] out TValue value)
        {
            Guard.AgainstNullOrWhitespace(key, nameof(key));
            if (_tags.TryGetValue(key, out var objValue) && objValue is TValue typedValue)
            {
                value = typedValue;
                return true;
            }
            value = default;
            return false;
        }

        public object? GetValueOrDefault(string key, object? defaultValue = null)
        {
            return _tags.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public TValue GetValueOrDefault<TValue>(string key, TValue defaultValue = default!)
        {
            Guard.AgainstNullOrWhitespace(key, nameof(key));
            if (_tags.TryGetValue(key, out var value) && value is TValue typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        // --- IMetadata Implementation (Immutability) ---
        public IMetadata WithTag(string key, object? value)
        {
            Guard.AgainstNullOrWhitespace(key, nameof(key));
            var newTags = _tags.SetItem(key, value);
            return ReferenceEquals(newTags, _tags) ? this : new MetadataImpl(newTags);
        }

        public IMetadata WithoutTag(string key)
        {
            Guard.AgainstNullOrWhitespace(key, nameof(key));
            if (!_tags.ContainsKey(key))
                return this;
            var newTags = _tags.Remove(key);
            return ReferenceEquals(newTags, _tags) ? this : new MetadataImpl(newTags);
        }

        public IMetadata WithTag(Func<KeyValuePair<string, object?>> tagFactory)
        {
            var newTag = tagFactory();
            return WithTag(newTag.Key, newTag.Value);
        }

        public IMetadata WithTags(IEnumerable<KeyValuePair<string, object?>> tags)
        {
            var newTags = _tags;
            bool changed = false;
            foreach (var tag in tags)
            {
                var updated = newTags.SetItem(tag.Key, tag.Value);
                if (!ReferenceEquals(updated, newTags))
                {
                    changed = true;
                }

                newTags = updated;
            }

            return changed ? new MetadataImpl(newTags) : this;
        }

        public IMetadata WithTags(Func<IEnumerable<KeyValuePair<string, object?>>> tagsFactory)
        {
            var tags = tagsFactory();
            return WithTags(tags);
        }

        public IMetadata WithoutTags(IEnumerable<string> keys)
        {
            var keySet = keys as ICollection<string> ?? keys.ToArray();
            if (keySet.Count == 0)
                return this;
            var newTags = _tags;
            bool changed = false;
            foreach (var key in keySet)
            {
                var updated = newTags.Remove(key);
                if (!ReferenceEquals(updated, newTags))
                    changed = true;
                newTags = updated;
            }
            return changed ? new MetadataImpl(newTags) : this;
        }

        public IMetadata WithoutTag(Func<string, bool> keyPredicate)
        {
            var keysToRemove = _tags.Keys.Where(keyPredicate).ToList();
            return WithoutTags(keysToRemove);
        }

        public IMetadata Merge(IMetadataReader other)
        {
            var newTags = _tags;
            bool changed = false;
            foreach (var tag in other.Tags)
            {
                var updated = newTags.SetItem(tag.Key, tag.Value);
                if (!ReferenceEquals(updated, newTags))
                    changed = true;
                newTags = updated;
            }
            return changed ? new MetadataImpl(newTags) : this;
        }

        public override string ToString()
        {
            return $"MetadataImpl[{string.Join(", ", _tags.Select(kv => $"{kv.Key}={kv.Value}"))}]";
        }
    }
}
