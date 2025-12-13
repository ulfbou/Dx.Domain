// <copyright file="src/Zentient.Metadata/Builders/MetadataBuilders.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

using Zentient.Abstractions.Metadata.Readers;

namespace Zentient.Metadata
{
    /// <summary>
    /// A concrete, immutable implementation of <see cref="IMetadataReader"/>.
    /// This class encapsulates all read-only metadata operations.
    /// </summary>
    public sealed class MetadataReader : IMetadataReader
    {
        private readonly IReadOnlyDictionary<string, object?> _tags;

        public MetadataReader(IDictionary<string, object?> tags)
        {
            _tags = new Dictionary<string, object?>(tags);
        }

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
    }
}
