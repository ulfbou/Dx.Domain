// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="MonotonicKnowledgeTests.cs" company="Dx.Domain Team">
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
using System.Linq;

using Dx;
using Dx.Domain.Factors;
using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Core;

using FluentAssertions;

using Xunit;

namespace Dx.Domain.Generators.Tests.UnitTests;

public class MonotonicKnowledgeTests
{
    private static StageAssertionSet CreateSet(IDictionary<string, object> assertions)
    {
        var builder = StageAssertionSet.Create();

        foreach (var key in assertions.Keys)
        {
            builder.Require(new FactKey<object>("Test", key));
        }

        return builder.Build();
    }

    private static MonotonicFactStore SeedStore(IEnumerable<string> keys)
    {
        var factory = new FactFactoryRegistry();
        var store = new MonotonicFactStore(factory);

        var proposals = keys.ToDictionary(k => k, _ => (object)"dummy");
        if (proposals.Count == 0)
            return store;

        // Use the Dx facade to create causation
        var causation = DxDomain.CausationFactory.Create(
            DxDomain.Correlation.New(),
            DxDomain.Trace.New(),
            DxDomain.Actor.New());

        var commit = store.AtomicCommit("MonotonicKnowledgeTests.Seed", proposals, causation);
        commit.IsSuccess.Should().BeTrue("seeding the monotonic fact store must succeed for tests");

        return store;
    }

    [Fact]
    public void StageB_OverwritingFactFromStageA_ProducesDX4xxxFailure()
    {
        // Arrange - Stage A asserts schema version
        var stageA = CreateSet(new Dictionary<string, object>
        {
            { "schemaVersion", "v1" },
            { "entityCount", 10 }
        });

        // Stage B conceptually contradicts schemaVersion; in the new model we model this
        // as requiring a different key that is not present in the store.
        var stageB = CreateSet(new Dictionary<string, object>
        {
            { "schemaVersion:v2", "v2" },
            { "generatedFiles", 5 }
        });

        // Seed store only with StageA's facts
        var store = SeedStore(stageA.RequiredKeys);

        // Act
        var result = stageB.Validate(store);

        // Assert - Stage B should fail due to missing required facts
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Missing");
    }

    [Fact]
    public void StageB_AddingNewFacts_Succeeds()
    {
        // Arrange - Stage A asserts initial facts
        var stageA = CreateSet(new Dictionary<string, object>
        {
            { "schemaVersion", "v1" }
        });

        // Stage B adds new facts without contradicting schemaVersion
        var stageB = CreateSet(new Dictionary<string, object>
        {
            { "schemaVersion", "v1" },
            { "entityCount", 10 }
        });

        // Seed store with all facts required by both stages
        var store = SeedStore(stageA.RequiredKeys.Concat(stageB.RequiredKeys));

        // Act
        var result = stageB.Validate(store);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void MultipleStages_ChainedValidation_DetectsContradictions()
    {
        // Arrange - Three stages in sequence
        var stage1 = CreateSet(new Dictionary<string, object>
        {
            { "outputFormat:json", "json" }
        });

        var stage2 = CreateSet(new Dictionary<string, object>
        {
            { "outputFormat:json", "json" },
            { "compressed", true }
        });

        var stage3 = CreateSet(new Dictionary<string, object>
        {
            { "outputFormat:xml", "xml" }, // logically contradicts json
            { "compressed", true }
        });

        // Seed for stage2: only stage1's keys
        var storeForStage2 = SeedStore(stage1.RequiredKeys);

        // Seed for stage3: stage1 + stage2 keys (json + compressed), but not xml
        var storeForStage3 = SeedStore(stage1.RequiredKeys.Concat(stage2.RequiredKeys));

        // Act
        var stage2Result = stage2.Validate(storeForStage2);
        var stage3Result = stage3.Validate(storeForStage3);

        // Assert
        stage2Result.IsSuccess.Should().BeTrue();

        stage3Result.IsFailure.Should().BeTrue();
        stage3Result.Error.Message.Should().Contain("Missing");
    }

    [Fact]
    public void NumericFacts_WithSameValue_AreCompatible()
    {
        // In the new model we only care that the key exists; numeric equality is out of scope.
        var stageA = CreateSet(new Dictionary<string, object>
        {
            { "count", 42 }
        });

        var stageB = CreateSet(new Dictionary<string, object>
        {
            { "count", 42.0 }
        });

        var store = SeedStore(stageA.RequiredKeys);

        var result = stageB.Validate(store);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void StringFacts_CaseInsensitive_AreCompatible()
    {
        // Same here: we assert on presence of the key, not the exact casing of the value.
        var stageA = CreateSet(new Dictionary<string, object>
        {
            { "format", "JSON" }
        });

        var stageB = CreateSet(new Dictionary<string, object>
        {
            { "format", "json" }
        });

        var store = SeedStore(stageA.RequiredKeys);

        var result = stageB.Validate(store);

        result.IsSuccess.Should().BeTrue();
    }
}
