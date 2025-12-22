// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Canonicalization.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Dx.Domain.Generators.Core
{
    /// <summary>
    /// Provides canonicalization utilities for generator inputs to ensure deterministic fingerprinting.
    /// Removes non-deterministic elements like timestamps, machine paths, random GUIDs, and environment variables.
    /// </summary>
    public static partial class Canonicalization
    {
        private static readonly JsonSerializerOptions CanonicalJsonOptions = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        [GeneratedRegex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?Z?")]
        private static partial Regex Iso8601TimestampRegex();

        [GeneratedRegex(@"\b\d{10,13}\b")]
        private static partial Regex UnixTimestampRegex();

        [GeneratedRegex(@"\b[0-9a-fA-F]{8}-?[0-9a-fA-F]{4}-?[0-9a-fA-F]{4}-?[0-9a-fA-F]{4}-?[0-9a-fA-F]{12}\b")]
        private static partial Regex GuidRegex();

        /// <summary>
        /// Canonicalizes a JSON object by sorting keys and removing non-deterministic values.
        /// </summary>
        /// <param name="jsonInput">The JSON string to canonicalize.</param>
        /// <returns>A canonicalized JSON string.</returns>
        public static string CanonicalizeJson(string jsonInput)
        {
            if (string.IsNullOrWhiteSpace(jsonInput))
                return string.Empty;

            try
            {
                var element = JsonDocument.Parse(jsonInput).RootElement;
                var canonical = CanonicalizeJsonElement(element);
                return JsonSerializer.Serialize(canonical, CanonicalJsonOptions);
            }
            catch (JsonException ex)
            {
                throw new ArgumentException($"Invalid JSON input: {ex.Message}", nameof(jsonInput), ex);
            }
        }

        /// <summary>
        /// Canonicalizes a dictionary by sorting keys and values.
        /// </summary>
        /// <param name="dict">The dictionary to canonicalize.</param>
        /// <returns>A canonicalized string representation.</returns>
        public static string CanonicalizeDictionary(IDictionary<string, string> dict)
        {
            if (dict == null || dict.Count == 0)
                return string.Empty;

            var sorted = dict
                .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
                .Select(kvp => $"{kvp.Key}={kvp.Value}");

            return string.Join(";", sorted);
        }

        /// <summary>
        /// Canonicalizes a file path by removing machine-specific prefixes and normalizing separators.
        /// </summary>
        /// <param name="path">The file path to canonicalize.</param>
        /// <returns>A canonicalized path string.</returns>
        public static string CanonicalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            // Normalize to forward slashes
            var normalized = path.Replace('\\', '/');

            // Remove common machine-specific prefixes
            var prefixes = new[] { "/home/", "/Users/", "C:/", "D:/", "/tmp/", "/var/" };
            foreach (var prefix in prefixes)
            {
                if (normalized.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    var index = normalized.IndexOf('/', prefix.Length);
                    if (index > 0)
                        normalized = normalized.Substring(index + 1);
                }
            }

            return normalized;
        }

        /// <summary>
        /// Removes timestamps from a string by replacing common timestamp patterns with a canonical value.
        /// </summary>
        /// <param name="input">The input string containing timestamps.</param>
        /// <returns>The string with timestamps removed.</returns>
        public static string RemoveTimestamps(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Replace ISO 8601 timestamps
            input = Iso8601TimestampRegex().Replace(input, "<TIMESTAMP>");

            // Replace Unix timestamps
            input = UnixTimestampRegex().Replace(input, "<TIMESTAMP>");

            return input;
        }

        /// <summary>
        /// Removes GUIDs from a string by replacing GUID patterns with a canonical value.
        /// </summary>
        /// <param name="input">The input string containing GUIDs.</param>
        /// <returns>The string with GUIDs removed.</returns>
        public static string RemoveGuids(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Replace GUID patterns (with or without hyphens)
            input = GuidRegex().Replace(input, "<GUID>");

            return input;
        }

        private static object CanonicalizeJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => CanonicalizJsonObject(element),
                JsonValueKind.Array => element.EnumerateArray()
                    .Select(CanonicalizeJsonElement)
                    .ToArray(),
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number => element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null!,
                _ => null!
            };
        }

        private static Dictionary<string, object> CanonicalizJsonObject(JsonElement element)
        {
            var result = new Dictionary<string, object>(StringComparer.Ordinal);

            foreach (var property in element.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal))
            {
                // Skip non-deterministic properties
                if (IsNonDeterministicProperty(property.Name))
                    continue;

                result[property.Name] = CanonicalizeJsonElement(property.Value);
            }

            return result;
        }

        private static bool IsNonDeterministicProperty(string propertyName)
        {
            var nonDeterministicNames = new[]
            {
                "timestamp", "createdAt", "updatedAt", "lastModified",
                "guid", "id", "uuid", "machineId", "machineName",
                "userName", "userPath", "tempPath", "randomSeed"
            };

            return nonDeterministicNames.Any(name =>
                propertyName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                propertyName.EndsWith(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
