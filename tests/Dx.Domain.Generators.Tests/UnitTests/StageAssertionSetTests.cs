// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageAssertionSetTests.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>

using Dx.Domain.Generators.Core;

namespace Dx.Domain.Generators.Tests.UnitTests;

public class StageAssertionSetTests
{
    [Fact]
    public void IsCompatibleWith_WithNoContradictions_ReturnsTrue()
    {
        // Arrange
        var priorAssertions = new Dictionary<string, object>
        {
            { "schema", "v1" },
            { "version", 1 }
        };
        var currentAssertions = new Dictionary<string, object>
        {
            { "schema", "v1" },
            { "additionalField", "value" }
        };

        var prior = new StageAssertionSet("stage1", priorAssertions);
        var current = new StageAssertionSet("stage2", currentAssertions);

        // Act
        var result = current.IsCompatibleWith(prior, out var contradictions);

        // Assert
        result.Should().BeTrue();
        contradictions.Should().BeEmpty();
    }

    [Fact]
    public void IsCompatibleWith_WithContradiction_ReturnsFalse()
    {
        // Arrange
        var priorAssertions = new Dictionary<string, object>
        {
            { "schema", "v1" }
        };
        var currentAssertions = new Dictionary<string, object>
        {
            { "schema", "v2" }
        };

        var prior = new StageAssertionSet("stage1", priorAssertions);
        var current = new StageAssertionSet("stage2", currentAssertions);

        // Act
        var result = current.IsCompatibleWith(prior, out var contradictions);

        // Assert
        result.Should().BeFalse();
        contradictions.Should().NotBeEmpty();
        contradictions.Should().Contain(c => c.Contains("schema"));
    }

    [Fact]
    public void IsCompatibleWith_WithNumericValues_ComparesCorrectly()
    {
        // Arrange
        var priorAssertions = new Dictionary<string, object>
        {
            { "count", 42 }
        };
        var currentAssertions = new Dictionary<string, object>
        {
            { "count", 42.0 } // Different numeric type but same value
        };

        var prior = new StageAssertionSet("stage1", priorAssertions);
        var current = new StageAssertionSet("stage2", currentAssertions);

        // Act
        var result = current.IsCompatibleWith(prior, out var contradictions);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsCompatibleWithAll_WithMultiplePriorStages_ValidatesAll()
    {
        // Arrange
        var stage1 = new StageAssertionSet("stage1", new Dictionary<string, object>
        {
            { "field1", "value1" }
        });
        var stage2 = new StageAssertionSet("stage2", new Dictionary<string, object>
        {
            { "field2", "value2" }
        });
        var current = new StageAssertionSet("stage3", new Dictionary<string, object>
        {
            { "field1", "value1" },
            { "field2", "value2" },
            { "field3", "value3" }
        });

        // Act
        var result = current.IsCompatibleWithAll(
            new[] { stage1, stage2 },
            out var contradictions);

        // Assert
        result.Should().BeTrue();
        contradictions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithNullStageName_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new StageAssertionSet(null!, new Dictionary<string, object>());
        act.Should().Throw<ArgumentNullException>();
    }
}
