// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="CanonicalizationOrderTests.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>

using Dx.Domain.Generators.Core;

namespace Dx.Domain.Generators.Tests.UnitTests;

public class CanonicalizationOrderTests
{
    [Fact]
    public void ReorderingJsonKeys_DoesNotChangeInputFingerprint()
    {
        // Arrange - Two JSON strings with same content but different key order
        var json1 = @"{""b"": 2, ""a"": 1, ""c"": 3}";
        var json2 = @"{""a"": 1, ""c"": 3, ""b"": 2}";
        var json3 = @"{""c"": 3, ""b"": 2, ""a"": 1}";

        // Act - Canonicalize all three
        var canonical1 = Canonicalization.CanonicalizeJson(json1);
        var canonical2 = Canonicalization.CanonicalizeJson(json2);
        var canonical3 = Canonicalization.CanonicalizeJson(json3);

        // Assert - All should produce the same canonical form
        canonical1.Should().Be(canonical2);
        canonical2.Should().Be(canonical3);
    }

    [Fact]
    public void ReorderingJsonKeys_ProducesSameFingerprint()
    {
        // Arrange - Create fingerprints with reordered intent
        var intent1 = Canonicalization.CanonicalizeJson(@"{""target"": ""entity"", ""name"": ""User""}");
        var intent2 = Canonicalization.CanonicalizeJson(@"{""name"": ""User"", ""target"": ""entity""}");
        
        var manifest = Canonicalization.CanonicalizeJson(@"{""project"": ""MyApp""}");
        var policies = "v1";
        var version = "1.0.0";

        // Act
        var fingerprint1 = InputFingerprint.Compute(intent1, manifest, policies, version);
        var fingerprint2 = InputFingerprint.Compute(intent2, manifest, policies, version);

        // Assert - Same fingerprint despite reordered keys
        fingerprint1.Should().Be(fingerprint2);
        fingerprint1.Value.Should().Be(fingerprint2.Value);
    }

    [Fact]
    public void DictionaryCanonicalization_SortsKeysAlphabetically()
    {
        // Arrange - Dictionary with unsorted keys
        var dict1 = new Dictionary<string, string>
        {
            { "zebra", "z" },
            { "alpha", "a" },
            { "bravo", "b" }
        };

        var dict2 = new Dictionary<string, string>
        {
            { "alpha", "a" },
            { "bravo", "b" },
            { "zebra", "z" }
        };

        // Act
        var canonical1 = Canonicalization.CanonicalizeDictionary(dict1);
        var canonical2 = Canonicalization.CanonicalizeDictionary(dict2);

        // Assert
        canonical1.Should().Be(canonical2);
        canonical1.Should().StartWith("alpha=a");
        canonical1.Should().EndWith("zebra=z");
    }

    [Fact]
    public void NestedJsonObjects_Canonicalized_ProduceSameOutput()
    {
        // Arrange - Nested objects with different key orders
        var json1 = @"{
            ""outer"": {
                ""z"": 3,
                ""a"": 1,
                ""m"": 2
            }
        }";

        var json2 = @"{
            ""outer"": {
                ""a"": 1,
                ""m"": 2,
                ""z"": 3
            }
        }";

        // Act
        var canonical1 = Canonicalization.CanonicalizeJson(json1);
        var canonical2 = Canonicalization.CanonicalizeJson(json2);

        // Assert
        canonical1.Should().Be(canonical2);
    }
}
