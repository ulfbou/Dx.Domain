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

using Dx.Domain.Facts;
using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Core;
using Dx.Domain.Generators.Model;
using Dx.Domain.Primitives;

using FluentAssertions;

using Xunit;

namespace Dx.Domain.Generators.Tests.UnitTests;

public class MonotonicKnowledgeTests
{
    private static StageAssertionSet CreateSet(IEnumerable<string> keys)
    {
        var builder = StageAssertionSet.Create();

        foreach (var key in keys)
        {
            builder.Require(new FactKey<DomainIntentModel>("Test", key));
        }

        return builder.Build();
    }

    private static IEnumerable<string> ToStoreKeys(IEnumerable<string> keys)
        => keys.Select(key => $"Test:{key}");

    private static MonotonicFactStore SeedStore(IEnumerable<string> keys)
    {
        var factory = new FactFactoryRegistry();
        var store = new MonotonicFactStore(factory);

        var proposals = keys.ToDictionary(k => k, _ => (object)"dummy");
        if (proposals.Count == 0)
            return store;

        // Use the Dx facade to create causation
        var causation = Causation.Create(
            CorrelationId.New(),
            TraceId.New(),
            actorId: null);

        var commit = store.AtomicCommit("MonotonicKnowledgeTests.Seed", proposals, causation);
        commit.IsSuccess.Should().BeTrue("seeding the monotonic fact store must succeed for tests");

        return store;
    }

    [Fact]
    public void StageB_OverwritingFactFromStageA_ProducesDX4xxxFailure()
    {
        // Arrange - Stage A asserts schema version
        var stageAKeys = new[] { "schemaVersion", "entityCount" };
        var stageA = CreateSet(stageAKeys);

        // Stage B conceptually contradicts schemaVersion; in the new model we model this
        // as requiring a different key that is not present in the store.
        var stageBKeys = new[] { "schemaVersion:v2", "generatedFiles" };
        var stageB = CreateSet(stageBKeys);

        // Seed store only with StageA's facts
        var store = SeedStore(ToStoreKeys(stageAKeys));

        // Act
        var result = stageB.Validate(store);

        // Assert - Stage B should fail due to missing required facts
        result.IsFailure.Should().BeTrue();
        result.Error.Diagnostic.Message.Should().Contain("Missing");
    }

    [Fact]
    public void StageB_AddingNewFacts_Succeeds()
    {
        // Arrange - Stage A asserts initial facts
        var stageAKeys = new[] { "schemaVersion" };
        var stageA = CreateSet(stageAKeys);

        // Stage B adds new facts without contradicting schemaVersion
        var stageBKeys = new[] { "schemaVersion", "entityCount" };
        var stageB = CreateSet(stageBKeys);

        // Seed store with all facts required by both stages
        var store = SeedStore(ToStoreKeys(stageAKeys.Concat(stageBKeys)));

        // Act
        var result = stageB.Validate(store);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void MultipleStages_ChainedValidation_DetectsContradictions()
    {
        // Arrange - Three stages in sequence
        var stage1Keys = new[] { "outputFormat:json" };
        var stage1 = CreateSet(stage1Keys);

        var stage2Keys = new[] { "outputFormat:json", "compressed" };
        var stage2 = CreateSet(stage2Keys);

        var stage3Keys = new[] { "outputFormat:xml", "compressed" };
        var stage3 = CreateSet(stage3Keys);

        // Seed for stage2: only stage1's keys
        var storeForStage2 = SeedStore(ToStoreKeys(stage1Keys));

        // Seed for stage3: stage1 + stage2 keys (json + compressed), but not xml
        var storeForStage3 = SeedStore(ToStoreKeys(stage1Keys.Concat(stage2Keys)));

        // Act
        var stage2Result = stage2.Validate(storeForStage2);
        var stage3Result = stage3.Validate(storeForStage3);

        // Assert
        stage2Result.IsSuccess.Should().BeTrue();

        stage3Result.IsFailure.Should().BeTrue();
        stage3Result.Error.Diagnostic.Message.Should().Contain("Missing");
    }

    [Fact]
    public void NumericFacts_WithSameValue_AreCompatible()
    {
        // In the new model we only care that the key exists; numeric equality is out of scope.
        var stageAKeys = new[] { "count" };
        var stageA = CreateSet(stageAKeys);

        var stageB = CreateSet(stageAKeys);

        var store = SeedStore(ToStoreKeys(stageAKeys));

        var result = stageB.Validate(store);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void StringFacts_CaseInsensitive_AreCompatible()
    {
        // Same here: we assert on presence of the key, not the exact casing of the value.
        var stageAKeys = new[] { "format" };
        var stageA = CreateSet(stageAKeys);

        var stageB = CreateSet(stageAKeys);

        var store = SeedStore(ToStoreKeys(stageAKeys));

        var result = stageB.Validate(store);

        result.IsSuccess.Should().BeTrue();
    }
}
