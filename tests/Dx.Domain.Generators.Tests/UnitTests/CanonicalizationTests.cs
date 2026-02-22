// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="CanonicalizationTests.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

using Dx.Domain.Generators.Core;

using FluentAssertions;

using Xunit;

namespace Dx.Domain.Generators.Tests.UnitTests;

public class CanonicalizationTests
{
    [Fact]
    public void CanonicalizeJson_SortsKeysAndRemovesNonDeterministicProperties()
    {
        // Arrange
        var json = "{\"b\": 2, \"a\": 1}";

        // Act
        var result = Core.Canonicalization.CanonicalizeJson(json);

        // Assert
        result.Should().Contain("\"a\":1");
        result.Should().Contain("\"b\":2");
    }

    [Fact]
    public void CanonicalizeJson_WithValidJson_ReturnsCanonicalForm()
    {
        // Arrange
        var json = "{\"b\": 2, \"a\": 1}";

        // Act
        var result = Core.Canonicalization.CanonicalizeJson(json);

        // Assert
        result.Should().Contain("\"a\"");
        result.Should().Contain("\"b\"");
    }

    [Fact]
    public void CanonicalizeJson_WithNonDeterministicProperties_RemovesThem()
    {
        // Arrange
        var json = "{\"value\": 1, \"timestamp\": \"2024-01-01T00:00:00Z\", \"guid\": \"123\"}";

        // Act
        var result = Core.Canonicalization.CanonicalizeJson(json);

        // Assert
        result.Should().Contain("value");
        result.Should().NotContain("timestamp");
        result.Should().NotContain("guid");
    }

    [Fact]
    public void CanonicalizeDictionary_SortsByKeys()
    {
        // Arrange
        var dict = new Dictionary<string, string>
        {
            { "z", "26" },
            { "a", "1" },
            { "m", "13" }
        };

        // Act
        var result = Core.Canonicalization.CanonicalizeDictionary(dict);

        // Assert
        result.Should().Be("a=1;m=13;z=26");
    }

    [Fact]
    public void CanonicalizePath_RemovesMachineSpecificPrefixes()
    {
        // Arrange
        var path = "/home/user/project/src/file.cs";

        // Act
        var result = Core.Canonicalization.CanonicalizePath(path);

        // Assert
        result.Should().NotContain("/home/user/");
        result.Should().Contain("project/src/file.cs");
    }

    [Fact]
    public void CanonicalizePath_NormalizesPathSeparators()
    {
        // Arrange
        var path = @"C:\\project\\src\\file.cs";

        // Act
        var result = Core.Canonicalization.CanonicalizePath(path);

        // Assert
        result.Should().Contain("/");
        result.Should().NotContain("\\");
    }

    [Fact]
    public void RemoveTimestamps_ReplacesIso8601Timestamps()
    {
        // Arrange
        var input = "Generated at 2024-01-01T12:00:00Z";

        // Act
        var result = Core.Canonicalization.RemoveTimestamps(input);

        // Assert
        result.Should().Contain("<TIMESTAMP>");
        result.Should().NotContain("2024-01-01");
    }

    [Fact]
    public void RemoveGuids_ReplacesGuidPatterns()
    {
        // Arrange
        var input = "ID: 12345678-1234-1234-1234-123456789abc";

        // Act
        var result = Core.Canonicalization.RemoveGuids(input);

        // Assert
        result.Should().Contain("<GUID>");
        result.Should().NotContain("12345678-1234-1234-1234-123456789abc");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CanonicalizeJson_WithEmptyInput_ReturnsEmptyString(string? input)
    {
        // Act
        var result = Core.Canonicalization.CanonicalizeJson(input!);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CanonicalizeJson_WithInvalidJson_ThrowsArgumentException()
    {
        // Arrange
        var invalidJson = "{not valid json";

        // Act & Assert
        var act = () => Core.Canonicalization.CanonicalizeJson(invalidJson);
        act.Should().Throw<ArgumentException>();
    }
}
