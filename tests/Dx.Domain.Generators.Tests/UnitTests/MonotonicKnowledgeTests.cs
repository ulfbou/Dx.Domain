// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="MonotonicKnowledgeTests.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>

using Dx.Domain.Generators.Core;

namespace Dx.Domain.Generators.Tests.UnitTests;

public class MonotonicKnowledgeTests
{
    [Fact]
    public void StageB_OverwritingFactFromStageA_ProducesDX4xxxFailure()
    {
        // Arrange - Stage A asserts schema version
        var stageA = new StageAssertionSet("StageA", new Dictionary<string, object>
        {
            { "schemaVersion", "v1" },
            { "entityCount", 10 }
        });

        // Act - Stage B tries to contradict Stage A's schema version
        var stageB = new StageAssertionSet("StageB", new Dictionary<string, object>
        {
            { "schemaVersion", "v2" }, // Contradiction!
            { "generatedFiles", 5 }
        });

        var isCompatible = stageB.IsCompatibleWith(stageA, out var contradictions);

        // Assert
        isCompatible.Should().BeFalse();
        contradictions.Should().NotBeEmpty();
        contradictions.Should().Contain(c => c.Contains("schemaVersion"));
        contradictions.Should().Contain(c => c.Contains("StageA"));
        contradictions.Should().Contain(c => c.Contains("v1"));
        contradictions.Should().Contain(c => c.Contains("v2"));
    }

    [Fact]
    public void StageB_AddingNewFacts_Succeeds()
    {
        // Arrange - Stage A asserts initial facts
        var stageA = new StageAssertionSet("StageA", new Dictionary<string, object>
        {
            { "schemaVersion", "v1" }
        });

        // Act - Stage B adds new facts without contradicting
        var stageB = new StageAssertionSet("StageB", new Dictionary<string, object>
        {
            { "schemaVersion", "v1" }, // Same as Stage A
            { "entityCount", 10 } // New fact
        });

        var isCompatible = stageB.IsCompatibleWith(stageA, out var contradictions);

        // Assert
        isCompatible.Should().BeTrue();
        contradictions.Should().BeEmpty();
    }

    [Fact]
    public void MultipleStages_ChainedValidation_DetectsContradictions()
    {
        // Arrange - Three stages in sequence
        var stage1 = new StageAssertionSet("Stage1", new Dictionary<string, object>
        {
            { "outputFormat", "json" }
        });

        var stage2 = new StageAssertionSet("Stage2", new Dictionary<string, object>
        {
            { "outputFormat", "json" },
            { "compressed", true }
        });

        var stage3 = new StageAssertionSet("Stage3", new Dictionary<string, object>
        {
            { "outputFormat", "xml" }, // Contradicts Stage1 and Stage2!
            { "compressed", true }
        });

        // Act
        var stage2Compatible = stage2.IsCompatibleWithAll(new[] { stage1 }, out var contradictions2);
        var stage3Compatible = stage3.IsCompatibleWithAll(new[] { stage1, stage2 }, out var contradictions3);

        // Assert
        stage2Compatible.Should().BeTrue();
        contradictions2.Should().BeEmpty();

        stage3Compatible.Should().BeFalse();
        contradictions3.Should().NotBeEmpty();
        contradictions3.Should().Contain(c => c.Contains("outputFormat"));
    }

    [Fact]
    public void NumericFacts_WithSameValue_AreCompatible()
    {
        // Arrange - Test numeric compatibility
        var stageA = new StageAssertionSet("StageA", new Dictionary<string, object>
        {
            { "count", 42 }
        });

        var stageB = new StageAssertionSet("StageB", new Dictionary<string, object>
        {
            { "count", 42.0 } // Different type but same value
        });

        // Act
        var isCompatible = stageB.IsCompatibleWith(stageA, out var contradictions);

        // Assert
        isCompatible.Should().BeTrue();
        contradictions.Should().BeEmpty();
    }

    [Fact]
    public void StringFacts_CaseInsensitive_AreCompatible()
    {
        // Arrange - Test case-insensitive string compatibility
        var stageA = new StageAssertionSet("StageA", new Dictionary<string, object>
        {
            { "format", "JSON" }
        });

        var stageB = new StageAssertionSet("StageB", new Dictionary<string, object>
        {
            { "format", "json" } // Different case
        });

        // Act
        var isCompatible = stageB.IsCompatibleWith(stageA, out var contradictions);

        // Assert
        isCompatible.Should().BeTrue();
        contradictions.Should().BeEmpty();
    }
}
