using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Zentient.Abstractions.Metadata;

namespace Zentient.Metadata
{
    /// <summary>
    /// Central registry for presets and standard metadata. Supports registration and lookup by type, tag, or preset key.
    /// </summary>
    public static class MetadataRegistry
    {
        private static readonly ConcurrentDictionary<object, IMetadata> _presets = new();

        public static void RegisterPreset(object key, IMetadata metadata)
        {
            _presets[key] = metadata;
        }

        public static bool TryGetPreset(object key, out IMetadata? metadata)
        {
            return _presets.TryGetValue(key, out metadata);
        }

        public static IEnumerable<object> RegisteredKeys => _presets.Keys;
    }
}
